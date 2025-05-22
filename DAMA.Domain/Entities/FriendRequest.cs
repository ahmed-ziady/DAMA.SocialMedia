using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAMA.Domain.Entities
{
    public class FriendRequest
    {
        [Key]
        public int FriendRequestId { get; set; }

        [Required]
        [ForeignKey("Sender")]
        public int SenderId { get; set; }

        [Required]
        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        // Navigation properties
        public virtual User Sender { get; set; } = null!;
        public virtual User Receiver { get; set; } = null!;

        // Status check properties
        public bool IsPending => Status == FriendRequestStatus.Pending;
        public bool IsAccepted => Status == FriendRequestStatus.Accepted;
        public bool IsRejected => Status == FriendRequestStatus.Rejected;
        public bool IsCancelled => Status == FriendRequestStatus.Cancelled;

        // State transition methods
        public void Accept()
        {
            if (Status != FriendRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be accepted");

            Status = FriendRequestStatus.Accepted;
        }

        public void Reject()
        {
            if (Status != FriendRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected");

            Status = FriendRequestStatus.Rejected;
        }

        public void Cancel()
        {
            if (Status != FriendRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be cancelled");

            Status = FriendRequestStatus.Cancelled;
        }
    }

    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Cancelled = 3
    }
}