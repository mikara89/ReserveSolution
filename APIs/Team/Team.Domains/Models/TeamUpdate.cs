using System.ComponentModel.DataAnnotations;

namespace Team.Domains.Models
{
    public class TeamUpdate 
    {

        [MaxLength(20), MinLength(4)]
        public string TeamName { get; set; }
        public int RegNumber { get; set; }
    }
}
