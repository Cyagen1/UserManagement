using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserManagement.Contracts
{
    [method: JsonConstructor]
    public class PermissionDto(string code, string description)
    {
        [Required]
        [StringLength(20)]
        public string Code { get; } = code;
        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; } = description;
    }
}
