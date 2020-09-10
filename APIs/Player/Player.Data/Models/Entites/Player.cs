using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Data.Models.Entites 
{
    public class PlayerEntity
    {
        public string Id { get; set; } 
        public string UserId { get; set; }
        public string TeamId { get; set; }
        public string NickName { get; set; } 
        public int RegNumber { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
