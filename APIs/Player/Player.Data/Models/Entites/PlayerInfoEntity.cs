using System;

namespace Player.Data.Models.Entites
{
    public class PlayerInfoEntity
    {
        
        public string TeamId { get; set; }
        public string Id { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public bool IsActive { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }
}
