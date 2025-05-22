using DAMA.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DAMAWebApi.Controllers
{
    [Authorize]
    public class NotificationHubController(IConnectionTracker connectionTracker) : Hub
    {
        public const string HubUrl = NotificationHub.HubUrl;
        private readonly IConnectionTracker _connectionTracker = connectionTracker;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionTracker.AddConnection(userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionTracker.RemoveConnection(userId, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}