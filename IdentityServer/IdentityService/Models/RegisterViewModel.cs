using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComformPassword { get; set; }
        public string ReturnUrl { get; set; } 
    }
}