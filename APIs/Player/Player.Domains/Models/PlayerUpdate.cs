using System.ComponentModel.DataAnnotations;

namespace Player.Domains.Models
{
    public class PlayerUpdate  
    {

        [MaxLength(50), MinLength(4)]
        public string FullName { get; set; }
        [MaxLength(20), MinLength(4)]
        public string NickName { get; set; }
    }
}
