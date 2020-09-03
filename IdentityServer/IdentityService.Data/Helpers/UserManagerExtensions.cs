using IdentityService.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Data.Helpers
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindActiveByIdAsync(this UserManager<AppUser> userManager, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return user.IsActive ? user : null;
        }
        public static async Task<AppUser> FindActiveByEmailAsync(this UserManager<AppUser> userManager, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user.IsActive ? user : null;
        }
        public static async Task<AppUser> FindActiveByNameAsync(this UserManager<AppUser> userManager, string name)
        {
            var user = await userManager.FindByNameAsync(name);
            if(user!=null)
                return user.IsActive ? user : null;
            return null;
        }
    }
}
