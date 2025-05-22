public record PendingFriendsDto
{
    public int RequestId { get; set; }
    public DateTime RequestDate { get; set; }

    public int SenderId { get; set; }
    public required string SenderFirstName { get; set; }
    public required string SenderLastName { get; set; }
    public required string SenderProfileImageUrl { get; set; }
}
