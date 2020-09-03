using System;

namespace Team.Api.Models
{
    public class TeamDto 
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public int RegNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
