// IFriendService.cs
using DAMA.Application.DTOs.FriendDtos;

namespace DAMA.Application.Interfaces
{
    public interface IFriendsServices
    {
        Task SendFriendRequest(int senderId, int receiverId);
        Task AcceptFriendRequest(int requestId, int userId);
        Task RejectFriendRequest(int requestId, int userId);
        Task CancelFriendReuests(int requestId);
        Task RemoveFriend(int userId, int friendId);
        Task<List<PendingFriendsDto>> GetPendingRequests(int userId);
        Task<FriendsResponseDto> GetFriends(int userId);
        Task<int> GetMutualFriendsNumber(int userId1, int userId2);

        Task<List<FriendsDto>> GetRequestsSended(int userID);
        public Task<bool> CheckIsFriend(int userId, int friendId);

    }
}