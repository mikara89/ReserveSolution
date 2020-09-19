using System;

namespace Player.Data.Models.Entites
{
    public class InvetationEntity 
    {
        public string Id { get; set; }
        public string TeamId { get; set; } 
        public DateTime Expiration { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsUsed { get; set; } 
    }
}
