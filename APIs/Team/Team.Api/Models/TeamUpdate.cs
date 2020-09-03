using System.ComponentModel.DataAnnotations;

namespace Team.Api.Models
{
    public class TeamUpdate 
    {
        [Required]
        [MaxLength(20), MinLength(4)]
        public string TeamName { get; set; }
        [Required]
        public int RegNumber { get; set; }
    }
}
