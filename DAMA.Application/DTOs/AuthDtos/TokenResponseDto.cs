using System.Text.Json.Serialization;

namespace DAMA.Application.DTOs.AuthDtos
{
    public record TokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }
        public int Id { get; set; }
    }
}
