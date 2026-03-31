using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FirstAidPlus.Hubs;

namespace FirstAidPlus.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Services.IAIService _aiService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<FirstAidPlus.Hubs.ChatHub> _hubContext;

        public ChatController(AppDbContext context, Services.IAIService aiService, Microsoft.AspNetCore.SignalR.IHubContext<FirstAidPlus.Hubs.ChatHub> hubContext)
        {
            _context = context;
            _aiService = aiService;
            _hubContext = hubContext;
        }

        // GET: Chat/With/5 (Chat with User ID 5)
        public async Task<IActionResult> With(int partnerId)
        {
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId)) return RedirectToAction("Login", "Account");

            var partner = await _context.Users.FindAsync(partnerId);
            if (partner == null) return NotFound("User not found");

            // Load history
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == partnerId) || 
                            (m.SenderId == partnerId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            ViewBag.Partner = partner;
            ViewBag.CurrentUserId = currentUserId;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send(int receiverId, string content)
        {
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdStr, out int currentUserId) && !string.IsNullOrEmpty(content))
            {
                var msg = new Message
                {
                    SenderId = currentUserId,
                    ReceiverId = receiverId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Messages.Add(msg);
                await _context.SaveChangesAsync();

                // Broadcast via SignalR
                var user1 = Math.Min(currentUserId, receiverId);
                var user2 = Math.Max(currentUserId, receiverId);
                var groupName = $"Chat_{user1}_{user2}";

                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveChatMessage", new
                {
                    senderId = currentUserId,
                    content = content,
                    createdAt = msg.CreatedAt.ToLocalTime().ToString("HH:mm")
                });

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }
            return RedirectToAction("With", new { partnerId = receiverId });
        }

        [HttpPost]
        [AllowAnonymous] // Allow guests to chat with AI
        public IActionResult GetAIResponse([FromBody] AIDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                 return BadRequest("Message is required.");
            }
            
            var reply = _aiService.GetReply(request.Message);
            return Json(new { reply, timestamp = DateTime.Now.ToString("h:mm tt") });
        }

        public class AIDto { public string Message { get; set; } }
    }
}
