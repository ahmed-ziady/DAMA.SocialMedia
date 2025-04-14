namespace DAMA.Domain.Entities
{
    public class FriendRequest
    {
        public int FriendRequestId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public FriendRequestStatus RequestStatus { get; set; } = FriendRequestStatus.Pending; 
        public DateTime DateSent { get; set; } = DateTime.UtcNow; 

        public User? Sender { get; set; } 
        public User? Receiver { get; set; } 

        public FriendRequest() { }

        public FriendRequest(User sender, User receiver)
        {
            Sender = sender;
            Receiver = receiver;
            SenderId = sender.Id;
            ReceiverId = receiver.Id;
            RequestStatus = FriendRequestStatus.Pending;
            DateSent = DateTime.UtcNow;
        }
    }

    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Declined
    }
}
