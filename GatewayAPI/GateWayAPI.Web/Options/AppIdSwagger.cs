using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWayAPI.Web.Options
{
    public class AppIdSwagger
    {
        public string ClientId { get; set; }
        public string AppName { get; set; }
        public string[] ApiNames { get; set; }
        public string[] AuthKeys { get; set; } 
    }
}
