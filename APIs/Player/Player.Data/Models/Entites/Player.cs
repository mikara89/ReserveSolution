using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Player.Data.Models.Entites
{
    public class PlayerEntity
    {
        public PlayerEntity()
        {
            Infos = new List<PlayerInfoEntity>();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public ICollection<PlayerInfoEntity> Infos { get; set; }

        public PlayerInfoEntity LastInfo()
        {
            return Infos.OrderBy(x => x.UpdatedAt).FirstOrDefault();
        }
    }
}
