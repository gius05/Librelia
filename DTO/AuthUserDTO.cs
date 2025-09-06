using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Librelia.DTO
{
    public class AuthUserDTO
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("rememberMe")] 
        public bool RememberMe { get; set; } = false;
    }
}
