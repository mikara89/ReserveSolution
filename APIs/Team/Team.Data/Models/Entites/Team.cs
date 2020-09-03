using System;
using System.Collections.Generic;
using System.Text;

namespace Team.Data.Models.Entites
{
    public class TeamEntity
    {
        public string Id { get; set; } 
        public string UserId { get; set; }  
        public string TeamName { get; set; } 
        public int RegNumber { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
