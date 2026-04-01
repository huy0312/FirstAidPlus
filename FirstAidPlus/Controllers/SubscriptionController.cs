using FirstAidPlus.Data;
using FirstAidPlus.Models;
using FirstAidPlus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transaction = FirstAidPlus.Models.Transaction;

namespace FirstAidPlus.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly IEmailService _emailService;
        private readonly IPayOSService _payOSService;

        public SubscriptionController(AppDbContext context, IVnPayService vnPayService, IMoMoService moMoService, IEmailService emailService, IPayOSService payOSService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _emailService = emailService;
            _payOSService = payOSService;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _context.Plans.ToListAsync();
            return View(plans);
        }

        public async Task<IActionResult> Checkout(int planId)
        {
            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int planId, string paymentMethod = "PayOS")
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null) return NotFound();

            var transaction = new Transaction
            {
                UserId = userId,
                PlanId = planId,
                Amount = plan.Price,
                Status = "Pending",
                OrderDescription = $"Mua goi {plan.Name} cho user {User.Identity.Name}",
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            if (paymentMethod == "VnPay")
            {
                transaction.VnPayTxnRef = DateTime.Now.Ticks.ToString();
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, transaction);
                return Redirect(paymentUrl);
            }
            else if (paymentMethod == "Momo")
            {
                transaction.MomoOrderId = Guid.NewGuid().ToString();
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                var paymentUrl = await _moMoService.CreatePaymentUrl(HttpContext, transaction);
                if (string.IsNullOrEmpty(paymentUrl) || paymentUrl.StartsWith("Error:")) {
                    TempData["ErrorMessage"] = "Lỗi tạo link thanh toán MoMo: " + paymentUrl;
                    return RedirectToAction("Index");
                }
                return Redirect(paymentUrl);
            }
            else if (paymentMethod == "PayOS")
            {
                try 
                {
                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    
                    var result = await _payOSService.CreatePaymentLink(HttpContext, transaction);
                    return Redirect(result.CheckoutUrl);
                }
                catch (Exception ex)
                {
                    // Catch the detailed error from PayOSService
                    TempData["ErrorMessage"] = "Lỗi tạo link PayOS: " + (ex.InnerException?.Message ?? ex.Message);
                    return RedirectToAction("Index");
                }
            }
            else if (paymentMethod == "BankTransfer")
            {
                transaction.VnPayTxnRef = DateTime.Now.Ticks.ToString();
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("BankTransferCheckout", new { transactionId = transaction.Id });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.ValidateCallback(Request.Query);

            if (response)
            {
                var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();
                var vnp_TransactionNo = Request.Query["vnp_TransactionNo"].ToString();
                
                var transaction = await _context.Transactions
                    .Include(t=>t.Plan)
                    .Include(t=>t.User)
                    .FirstOrDefaultAsync(t => t.VnPayTxnRef == vnp_TxnRef);

                if (transaction != null && transaction.Status != "Success")
                {
                    transaction.VnPayTransactionNo = vnp_TransactionNo;
                    transaction.Status = "Success";
                    _context.Transactions.Update(transaction);

                    await ProcessSuccessfulSubscription(transaction);
                    await _context.SaveChangesAsync();
                    
                    // Send Email
                    try {
                        string userEmail = transaction.User?.Email ?? User.FindFirstValue(ClaimTypes.Email);
                        if (!string.IsNullOrEmpty(userEmail)) {
                            await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công", 
                                $"<h3>Cảm ơn bạn đã thanh toán!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p><p>Mã giao dịch: {vnp_TransactionNo}</p>");
                        }
                    } catch (Exception) { /* log error */ }
                }

                TempData["SuccessMessage"] = "Thanh toán thành công! Gói đăng ký của bạn đã được kích hoạt.";
                return RedirectToAction("PaymentSuccess");
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán thất bại hoặc bị hủy.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> MomoCallback()
        {
            var response = _moMoService.ValidateCallback(Request.Query);
            if (response.success)
            {
                var transaction = await _context.Transactions
                   .Include(t => t.Plan)
                   .Include(t => t.User)
                   .FirstOrDefaultAsync(t => t.MomoOrderId == response.orderId);

                if (transaction != null && transaction.Status != "Success")
                {
                    transaction.MomoTransId = response.transId;
                    transaction.Status = "Success";
                    _context.Transactions.Update(transaction);

                    await ProcessSuccessfulSubscription(transaction);
                    await _context.SaveChangesAsync();

                    // Send Email
                    try
                    {
                        string userEmail = transaction.User?.Email ?? User.FindFirstValue(ClaimTypes.Email);
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công",
                                $"<h3>Cảm ơn bạn đã thanh toán qua MoMo!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p><p>Mã giao dịch: {response.transId}</p>");
                        }
                    }
                    catch (Exception) { /* log error */ }
                }
                TempData["SuccessMessage"] = "Thanh toán MoMo thành công!";
                return RedirectToAction("PaymentSuccess");
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán MoMo thất bại.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> PayOSCallback([FromQuery] string code, [FromQuery] string id, [FromQuery] bool cancel, [FromQuery] string status, [FromQuery] int orderCode)
        {
            if (cancel)
            {
                TempData["ErrorMessage"] = "Bạn đã hủy thanh toán PayOS.";
                return RedirectToAction("Index");
            }

            if (status == "PAID")
            {
                // Verify order and wait for webhook to update status, but we can optimistically show success or just verify here
                var transaction = await _context.Transactions.Include(t => t.Plan).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == orderCode);
                
                if (transaction != null)
                {
                    if (transaction.Status != "Success")
                    {
                         // Actually the Webhook will handle this better, but we can update here if it hasn't been handled yet
                         transaction.Status = "Success";
                         _context.Transactions.Update(transaction);
                         await ProcessSuccessfulSubscription(transaction);
                         await _context.SaveChangesAsync();
                         
                         try
                         {
                             string userEmail = transaction.User?.Email ?? User.FindFirstValue(ClaimTypes.Email);
                             if (!string.IsNullOrEmpty(userEmail))
                             {
                                 await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công",
                                     $"<h3>Cảm ơn bạn đã thanh toán qua PayOS!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p><p>Mã đơn hàng: {orderCode}</p>");
                             }
                         }
                         catch (Exception) { /* log error */ }
                    }
                    TempData["SuccessMessage"] = "Thanh toán PayOS thành công!";
                    return RedirectToAction("PaymentSuccess");
                }
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình thanh toán PayOS.";
            return RedirectToAction("Index");
        }

        [HttpPost("payos/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOS.Models.Webhooks.Webhook webhookBody)
        {
            try 
            {
                var webhookData = await _payOSService.VerifyPaymentWebhookData(webhookBody);
                if (webhookData == null) return Ok(new { success = false, message = "Invalid webhook data" });

                if (webhookData.Code == "00")
                {
                    var orderCode = webhookData.OrderCode;
                    var transaction = await _context.Transactions.Include(t => t.Plan).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == orderCode);

                    if (transaction != null && transaction.Status != "Success")
                    {
                        transaction.Status = "Success";
                        _context.Transactions.Update(transaction);
                        await ProcessSuccessfulSubscription(transaction);
                        await _context.SaveChangesAsync();
                        
                        try
                        {
                            string userEmail = transaction.User?.Email;
                            if (!string.IsNullOrEmpty(userEmail))
                            {
                                await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công",
                                    $"<h3>Cảm ơn bạn đã thanh toán qua PayOS!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p><p>Mã đơn hàng: {orderCode}</p>");
                            }
                        }
                        catch (Exception) { /* log error */ }
                    }
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        public async Task<IActionResult> BankTransferCheckout(int transactionId)
        {
            var transaction = await _context.Transactions.Include(t => t.Plan).FirstOrDefaultAsync(t => t.Id == transactionId);
            if (transaction == null || transaction.Status == "Success") return RedirectToAction("Index");
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> BankTransferConfirm(int transactionId)
        {
            var transaction = await _context.Transactions.Include(t => t.Plan).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == transactionId);
            if (transaction != null && transaction.Status != "Success")
            {
                transaction.Status = "Success";
                _context.Transactions.Update(transaction);
                await ProcessSuccessfulSubscription(transaction);
                await _context.SaveChangesAsync();

                try
                {
                    string userEmail = transaction.User?.Email ?? User.FindFirstValue(ClaimTypes.Email);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công", $"<h3>Cảm ơn bạn đã thanh toán qua Chuyển Khoản!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p>");
                    }
                }
                catch (Exception) { /* log error */ }
            }
            TempData["SuccessMessage"] = "Xác nhận chuyển khoản thành công!";
            return RedirectToAction("PaymentSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null) return Json(new { success = false, status = "NotFound" });

            if (transaction.Status == "Success")
            {
                return Json(new { success = true, status = "Success" });
            }
            return Json(new { success = true, status = transaction.Status });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SepayWebhook([FromBody] SePayWebhookData data)
        {
            // This endpoint handles SePay webhooks which can be configured for TPBank/other banks
            try
            {
                if (data == null || string.IsNullOrEmpty(data.content) || data.transferType != "in")
                    return Ok(new { success = true });

                string content = data.content.ToLower();
                int transactionId = 0;

                // Support various formats: "don hang 123", "DH123", or just "123"
                var match = System.Text.RegularExpressions.Regex.Match(content, @"don hang (\d+)");
                if (match.Success)
                {
                    transactionId = int.Parse(match.Groups[1].Value);
                }
                else
                {
                    var dhMatch = System.Text.RegularExpressions.Regex.Match(content, @"dh(\d+)");
                    if (dhMatch.Success) 
                    {
                        transactionId = int.Parse(dhMatch.Groups[1].Value);
                    }
                    else 
                    {
                        var anyMatch = System.Text.RegularExpressions.Regex.Match(content, @"(\d+)");
                        if (anyMatch.Success) transactionId = int.Parse(anyMatch.Groups[1].Value);
                    }
                }

                if (transactionId > 0)
                {
                    var transaction = await _context.Transactions.Include(t => t.Plan).Include(t => t.User).FirstOrDefaultAsync(t => t.Id == transactionId);
                    
                    if (transaction != null && transaction.Status != "Success" && data.transferAmount >= transaction.Amount)
                    {
                        transaction.Status = "Success";
                        transaction.VnPayTransactionNo = $"BANK_{data.id}"; // Use SePay ID as reference
                        _context.Transactions.Update(transaction);
                        await ProcessSuccessfulSubscription(transaction);
                        await _context.SaveChangesAsync();

                        try
                        {
                            string userEmail = transaction.User?.Email;
                            if (!string.IsNullOrEmpty(userEmail))
                            {
                                await _emailService.SendEmailAsync(userEmail, "Xác nhận thanh toán thành công", 
                                    $"<h3>Cảm ơn bạn đã thanh toán qua Chuyển Khoản {data.gateway}!</h3><p>Gói: {transaction.Plan?.Name}</p><p>Giá: {transaction.Amount:N0} VND</p><p>Mã giao dịch: {data.id}</p>");
                            }
                        }
                        catch (Exception) { /* log error */ }
                    }
                }
                
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        private async Task ProcessSuccessfulSubscription(Transaction transaction)
        {
            // Activate/Extend Subscription
            var existingSub = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == transaction.UserId && s.PlanId == transaction.PlanId && s.EndDate > DateTime.UtcNow);

            if (existingSub != null)
            {
                // Extend
                var currentEndDate = existingSub.EndDate ?? DateTime.UtcNow;
                if (currentEndDate < DateTime.UtcNow) currentEndDate = DateTime.UtcNow;

                existingSub.EndDate = currentEndDate.AddMonths(transaction.Plan?.DurationValue ?? 1);
                _context.UserSubscriptions.Update(existingSub);
            }
            else
            {
                // Create New
                var newSub = new UserSubscription
                {
                    UserId = transaction.UserId,
                    PlanId = transaction.PlanId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(transaction.Plan?.DurationValue ?? 1),
                    Status = "Active"
                };
                _context.UserSubscriptions.Add(newSub);
            }
        }
    }
}
