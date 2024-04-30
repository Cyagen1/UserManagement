using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagement.Attributes;

namespace UserManagement.DataAccess.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [Password(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, and one digit.")]
        public string Password { get; set; }
        [Required]
        public bool Status { get; set; }
        public List<UserPermission> UserPermissions { get; set; }
    }
}
