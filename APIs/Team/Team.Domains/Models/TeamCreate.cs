using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Team.Domains.Models
{
    public class TeamCreate
    {
        [Required]
        [MaxLength(20), MinLength(4)]
        public string TeamName { get; set; }
        [Required]
        public int RegNumber { get; set; }
    }
}
