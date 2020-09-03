using System;
using System.Collections.Generic;
using System.Text;

namespace SendMailService.Service.Options
{
    public class AppEmailSettings
    {
        public string Host { get; set; }
        public string Email { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
