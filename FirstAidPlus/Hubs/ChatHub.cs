using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FirstAidPlus.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinChat(int partnerId)
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var user1 = Math.Min(currentUserId, partnerId);
                var user2 = Math.Max(currentUserId, partnerId);
                var groupName = $"Chat_{user1}_{user2}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task LeaveChat(int partnerId)
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var user1 = Math.Min(currentUserId, partnerId);
                var user2 = Math.Max(currentUserId, partnerId);
                var groupName = $"Chat_{user1}_{user2}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }
    }
}
