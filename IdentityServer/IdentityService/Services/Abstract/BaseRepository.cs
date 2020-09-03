using IdentityService.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityService.Services 
{
    public abstract class BaseRepository
    {
        public readonly SignInManager<AppUser> signInManager;
        public readonly UserManager<AppUser> userManager;
        public readonly AppSettings appSettings;
        public BaseRepository(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IOptions<AppSettings> appSettings)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }
    }
}
