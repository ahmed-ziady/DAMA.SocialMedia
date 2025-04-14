using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAMA.Infrastructure.Services
{
    public class FriendService(DamaContext context) : IFriendService
    {
        private readonly DamaContext _context = context;

        // Send Friend Request
        public async Task<bool> SendFriendRequest(int senderId, int receiverId)
        {
            // Check if a friend request already exists
            if (await _context.FriendRequests.AnyAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId))
                return false;

            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);
            if (sender == null || receiver == null)
                return false;

            // Create friend request (assumes your FriendRequest class has a constructor accepting sender and receiver)
            var friendRequest = new FriendRequest(sender, receiver);
            await _context.FriendRequests.AddAsync(friendRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        // Accept Friend Request (only receiver can accept)
        public async Task<bool> AcceptFriendRequest(int requestId, int userId)
        {
            var request = await _context.FriendRequests.FindAsync(requestId);
            if (request == null || request.ReceiverId != userId)
                return false; // Either not found or unauthorized

            // Create a Friendship record using new property names
            var friendship = new Friendship
            {
                RequesterId = request.SenderId,
                ReceiverId = request.ReceiverId
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Friendships.Add(friendship);
                _context.FriendRequests.Remove(request);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Reject Friend Request (only receiver can reject)
        public async Task<bool> RejectFriendRequest(int requestId, int userId)
        {
            var request = await _context.FriendRequests.FindAsync(requestId);
            if (request == null || request.ReceiverId != userId)
                return false;

            _context.FriendRequests.Remove(request);
            return await _context.SaveChangesAsync() > 0;
        }

        // Remove Friend (Unfriend)
        public async Task<bool> RemoveFriend(int userId, int friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.RequesterId == userId && f.ReceiverId == friendId) ||
                (f.RequesterId == friendId && f.ReceiverId == userId));

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            return await _context.SaveChangesAsync() > 0;
        }

        // Get Friends List
        public async Task<List<User>> GetFriendsList(int userId)
        {
            return await _context.Friendships
                .Where(f => f.RequesterId == userId || f.ReceiverId == userId)
                .Select(f => f.RequesterId == userId ? f.Receiver! : f.Requester!)
                .ToListAsync();
        }

        // Get Friend Requests List (for the receiver)
        public async Task<List<FriendRequest>> GetFriendRequests(int userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId)
                .ToListAsync();
        }
    }
}
