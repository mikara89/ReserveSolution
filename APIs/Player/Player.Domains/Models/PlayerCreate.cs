using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Player.Domains.Models
{
    public class PlayerCreate
    {
        [Required]
        [MaxLength(20), MinLength(4)]
        public string NickName { get; set; }
        [Required]
        [MaxLength(50), MinLength(4)]
        public string FullName { get; set; }
    }
}
