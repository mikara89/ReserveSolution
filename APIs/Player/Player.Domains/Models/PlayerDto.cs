using System;

namespace Player.Domains.Models
{
    public class PlayerDto   
    {
        public string Id { get; set; }
        public string TeamId { get; set; } 
        public string FullName { get; set; }
        public string NickName { get; set; }  
        public bool  IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
    }
}
