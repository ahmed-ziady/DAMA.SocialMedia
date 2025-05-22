namespace DAMA.Application.DTOs.SearchDto
{
    public record SearchResponseDto
    {
       
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string? ProfileImageUrl { get; set; }
             public int MutualFriendsCount { get; set; }

    }
}
