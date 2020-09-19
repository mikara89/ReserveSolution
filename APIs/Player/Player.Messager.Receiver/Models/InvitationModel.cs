using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Messager.Receiver.Models
{
    public class InvitationModel
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsUsed { get; set; }

    }
}
