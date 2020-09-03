using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class PasswordResetModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComformPassword { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
