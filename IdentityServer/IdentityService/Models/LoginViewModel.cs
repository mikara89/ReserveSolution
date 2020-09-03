using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class LoginViewModel
    {
        [Required]
        public  string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }   
    }
}