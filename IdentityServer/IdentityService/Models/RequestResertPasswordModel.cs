using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models 
{
    public class RequestResertPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
