using DAMA.Domain.Entities;

public interface INotificationHubClient
{
    Task ReceiveFriendRequest(FriendRequest request);
    Task FriendRequestAccepted(Friendship friendship);
    Task FriendRequestRejected(int requestId);
}