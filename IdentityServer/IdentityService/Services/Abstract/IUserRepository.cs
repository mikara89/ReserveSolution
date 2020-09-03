using IdentityService.Data.Models;
using IdentityService.Models;
using System.Threading.Tasks;

namespace IdentityService.Services
{
    public interface IUserRepository
    {
        Task RegisterAsync(RegisterViewModel model, string ConfirmEmailLink);
        Task ConfirmEmail(string userId, string token);
        Task ForgotPasswordAsync(RequestResertPasswordModel model, string resetLink);
        Task ResetPasswordAsync(PasswordResetModel model);
        Task BanUser(string userId, string userName, bool superUser);  
    }
}