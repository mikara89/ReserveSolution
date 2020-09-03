using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Team.Api.Models
{
    public class TeamCreate
    { 
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(20), MinLength(4)]
        public string TeamName { get; set; }
        [Required]
        public int RegNumber { get; set; }
    }
}
