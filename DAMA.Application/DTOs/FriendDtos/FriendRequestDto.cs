using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.FriendDtos
{
    public record FriendRequestDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ReceiverId { get; set; }

    }
}