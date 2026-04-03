using FirstAidPlus.Data;
using FirstAidPlus.Hubs;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FirstAidPlus.Services
{
    public class SubscriptionExpirationWorker : BackgroundService
    {
        private readonly ILogger<SubscriptionExpirationWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public SubscriptionExpirationWorker(ILogger<SubscriptionExpirationWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiration Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpirationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking subscription expirations.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Subscription Expiration Worker is stopping.");
        }

        private async Task CheckExpirationsAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                var now = DateTime.Now;

                var expiredSubscriptions = await context.UserSubscriptions
                    .Include(s => s.User)
                    .Include(s => s.Plan)
                    .Where(s => s.Status == "Active" && s.EndDate < now)
                    .ToListAsync();

                if (expiredSubscriptions.Any())
                {
                    _logger.LogInformation($"Found {expiredSubscriptions.Count} expired subscriptions.");

                    foreach (var sub in expiredSubscriptions)
                    {
                        sub.Status = "Expired";

                        // 1. Create Web Notification
                        var notification = new Notification
                        {
                            UserId = sub.UserId,
                            Title = "Gói đăng ký hết hạn",
                            Message = $"Gói '{sub.Plan?.Name}' của bạn đã hết hạn vào ngày {sub.EndDate?.ToLocalTime():dd/MM/yyyy}. Hãy gia hạn để tiếp tục sử dụng!",
                            Link = "/Subscription",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false
                        };
                        context.Notifications.Add(notification);

                        // 2. Send Email
                        if (!string.IsNullOrEmpty(sub.User?.Email))
                        {
                            try
                            {
                                string emailBody = $@"
                                    <h3>Thông báo hết hạn gói dịch vụ</h3>
                                    <p>Xin chào <strong>{sub.User.FullName}</strong>,</p>
                                    <p>Gói đăng ký <strong>{sub.Plan?.Name}</strong> của bạn đã hết hạn vào ngày {sub.EndDate?.ToLocalTime():dd/MM/yyyy HH:mm}.</p>
                                    <p>Để tiếp tục truy cập vào các bài giảng và nhận chứng chỉ quốc tế, vui lòng thực hiện gia hạn gói dịch vụ.</p>
                                    <p><a href='https://firstaidplus.vn/Subscription' style='background-color: #d71920; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Gia hạn ngay</a></p>
                                    <br/>
                                    <p>Trân trọng,<br/>Đội ngũ FirstAid+</p>";

                                await emailService.SendEmailAsync(sub.User.Email, "Thông báo hết hạn gói đăng ký - FirstAid+", emailBody);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to send expiration email to {sub.User.Email}");
                            }
                        }

                        // 3. Push real-time notification
                        await hubContext.Clients.User(sub.UserId.ToString()).SendAsync("ReceiveNotification", new
                        {
                            Title = notification.Title,
                            Message = notification.Message,
                            Link = notification.Link,
                            CreatedAt = notification.CreatedAt
                        });
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Successfully processed expired subscriptions.");
                }
            }
        }
    }
}
