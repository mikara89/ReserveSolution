using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityService.Messaging.Sender.Options
{
    public class MailModel
    {
        [Required]
        [EmailAddress]
        public string To { get; set; }
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }
    }
}
