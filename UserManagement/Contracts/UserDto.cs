using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UserManagement.Attributes;

namespace UserManagement.Contracts
{
    [method: JsonConstructor]
    public class UserDto(string username, string password, bool status)
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; } = username;
        [Required]
        [Password(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; } = password;
        [Required]
        public bool Status { get; } = status;
    }
}
