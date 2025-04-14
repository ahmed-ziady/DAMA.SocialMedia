using DAMA.Domain.Entities;

namespace DAMA.Application.Interfaces
{
    public interface IFriendService
    {
        Task<bool> SendFriendRequest(int senderId, int receiverId);
        Task<bool> AcceptFriendRequest(int requestId, int userId);
        Task<bool> RejectFriendRequest(int requestId, int userId);
        Task<bool> RemoveFriend(int userId, int friendId);
        Task<List<User>> GetFriendsList(int userId);
        Task<List<FriendRequest>> GetFriendRequests(int userId);
    }
}
