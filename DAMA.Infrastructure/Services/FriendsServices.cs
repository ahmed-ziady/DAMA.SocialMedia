using DAMA.Application.DTOs.FriendDtos;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class FriendsServices(DamaContext context, IConnectionTracker _connectionTracker, IHubContext<NotificationHub> _hubContext) : IFriendsServices
    {
        public async Task AcceptFriendRequest(int requestId, int userId)
        {
            var request = await context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId && fr.ReceiverId == userId)
                ?? throw new ArgumentNullException(nameof(requestId), "Friend request not found or you are not the receiver.");

            if (request.Status != FriendRequestStatus.Pending)
                throw new InvalidOperationException("Friend request is not pending.");

            // Check if friendship already exists
            bool friendshipExists = await context.Friendships.AnyAsync(f =>
                (f.RequesterId == request.SenderId && f.ReceiverId == request.ReceiverId) ||
                (f.RequesterId == request.ReceiverId && f.ReceiverId == request.SenderId));

            if (friendshipExists)
                throw new InvalidOperationException("Friendship already exists.");

            request.Accept();

            Friendship friendships =

                new()
                { RequesterId = request.SenderId, ReceiverId = request.ReceiverId };


            await context.Friendships.AddAsync(friendships);
            context.FriendRequests.Remove(request);
            await context.SaveChangesAsync();

            await SendNotification(request.SenderId, "FriendRequestAccepted", request);
        }


        public async Task<FriendsResponseDto> GetFriends(int userId)
        {
            var friends = await context.Friendships
                .AsNoTracking()
                .Where(f =>
                    f.RequesterId == userId || f.ReceiverId == userId)
                .Select(f => new
                {
                    Friend = f.RequesterId == userId ? f.Receiver : f.Requester,
                    f.FriendshipId,
                    f.CreatedAt
                })
                .Select(f => new FriendsDto
                {
                    Id = f.FriendshipId,
                    FirstName = f.Friend.FirstName,
                    LastName = f.Friend.LastName,
                    ProfileImageUrl = f.Friend.ProfileImageUrl ?? String.Empty,
                    CreatedAt = f.CreatedAt,
                    FriendId = f.Friend.Id
                })
                .ToListAsync();

            return new FriendsResponseDto
            {
                Friends = friends,
                TotalCount = friends.Count
            };
        }
        public async Task<int> GetMutualFriendsNumber(int userId1, int userId2)
        {
            var friendsOf1 = await context.Friendships
                .Where(f => f.RequesterId == userId1 || f.ReceiverId == userId1)
                .Select(f => f.RequesterId == userId1 ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            var friendsOf2 = await context.Friendships
                .Where(f => f.RequesterId == userId2 || f.ReceiverId == userId2)
                .Select(f => f.RequesterId == userId2 ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            var mutualFriends = friendsOf1.Intersect(friendsOf2).Count();

            return mutualFriends;
        }


        public async Task<List<PendingFriendsDto>> GetPendingRequests(int userId)
        {


            return await context.FriendRequests
                .AsNoTracking()
                .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
                .Select(fr => new PendingFriendsDto
                {
                    RequestId = fr.FriendRequestId,
                    RequestDate = fr.RequestDate,
                    SenderId = fr.Sender.Id,
                    SenderFirstName = fr.Sender.FirstName,
                    SenderLastName = fr.Sender.LastName,
                    SenderProfileImageUrl = fr.Sender.ProfileImageUrl ?? String.Empty
                })
                .ToListAsync();
        }


        public async Task RejectFriendRequest(int requestId, int userId)
        {
            var request = await context.FriendRequests
                  .Include(fr => fr.Sender)
                  .Include(fr => fr.Receiver)
                  .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId && fr.ReceiverId == userId) ?? throw new ArgumentException("Friend request not found or you are not the receiver.");
            if (request.Status != FriendRequestStatus.Pending)
            {
                throw new InvalidOperationException("Friend request is not pending.");
            }

            request.Reject();
            context.FriendRequests.Remove(request);
            await context.SaveChangesAsync();

            await SendNotification(request.SenderId, "FriendRequestRejected", request);

        }

        public async Task RemoveFriend(int userId, int friendId)
        {
            var friendship = await context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == userId && f.ReceiverId == friendId) ||
                    (f.RequesterId == friendId && f.ReceiverId == userId))
                ?? throw new ArgumentException("Friendship not found.");

            context.Friendships.Remove(friendship);
            await context.SaveChangesAsync();

            await SendNotification(friendId, "FriendshipRemoved", userId);
        }

        public async Task SendFriendRequest(int senderId, int receiverId)
        {
            if (senderId == receiverId)
                throw new ArgumentException("You cannot send a friend request to yourself.");

            var isFriend = await context.Friendships

            .AsNoTracking()
            .AnyAsync(f =>
                (f.RequesterId == senderId && f.ReceiverId == receiverId) ||
                (f.RequesterId == receiverId && f.ReceiverId == senderId));

            if (isFriend)
                throw new InvalidOperationException("You are already friends.");

            var requestExists = await context.FriendRequests
                .AsNoTracking()
                .AnyAsync(fr =>
                    (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                    (fr.SenderId == receiverId && fr.ReceiverId == senderId));

            if (requestExists)
                throw new InvalidOperationException("Friend request already exists.");

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                RequestDate = DateTime.UtcNow,
                Status = FriendRequestStatus.Pending
            };

            context.FriendRequests.Add(friendRequest);
            await context.SaveChangesAsync();

            await SendNotification(receiverId, "NewFriendRequest", friendRequest);
        }

        public async Task<List<FriendsDto>> GetRequestsSended(int id)
        {
            return await context.FriendRequests
                .AsNoTracking()
                .Where(fr => fr.SenderId == id && fr.Status == FriendRequestStatus.Pending)
                .Select(fr => new FriendsDto
                {
                    Id = fr.FriendRequestId,
                    FirstName = fr.Receiver.FirstName,
                    LastName = fr.Receiver.LastName,
                    ProfileImageUrl = fr.Receiver.ProfileImageUrl ?? String.Empty,
                    CreatedAt = fr.RequestDate,
                    FriendId = fr.Receiver.Id
                })
                .ToListAsync();
        }

        public async Task CancelFriendReuests(int requestId)
        {
            var request = await context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId) ?? throw new ArgumentException("Friend request not found.");
            if (request.Status != FriendRequestStatus.Pending)
            {
                throw new InvalidOperationException("Friend request is not pending.");
            }
            request.Cancel();
            context.FriendRequests.Remove(request);
            await context.SaveChangesAsync();
            await SendNotification(request.ReceiverId, "FriendRequestCanceled", request);
        }

        public async Task<bool> CheckIsFriend(int userId, int friendId)
        {
            if (userId == friendId)
                throw new ArgumentException("You cannot check friendship with yourself.");
            if (userId <= 0 || friendId <= 0)
                throw new ArgumentException("User IDs must be greater than zero.");
            var isFriend = await context.Friendships
                .AsNoTracking()
                .AnyAsync(f =>
                    (f.RequesterId == userId && f.ReceiverId == friendId) ||
                    (f.RequesterId == friendId && f.ReceiverId == userId));
            return isFriend;
        }

        private async Task SendNotification(int userId, string method, object payload)
        {
            var connections = await _connectionTracker.GetConnections(userId.ToString());
            if (connections.Any())
            {
                await _hubContext.Clients.Clients(connections).SendAsync(method, payload);
            }
        }
    }
}
