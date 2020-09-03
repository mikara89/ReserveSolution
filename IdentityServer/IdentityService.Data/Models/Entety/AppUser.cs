using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Data.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsActive { get; set; }

    }
}
