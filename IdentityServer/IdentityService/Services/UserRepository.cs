using IdentityService.Data.Helpers;
using IdentityService.Data.Models;
using IdentityService.Messaging.Sender.Sender;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityService.Services
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly MailSendSender mailSendSender;
        private readonly ILogger<UserRepository> logger;
        public UserRepository(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IOptions<AppSettings> appSettings,
            MailSendSender mailSendSender,
            ILogger<UserRepository> logger)
            : base(signInManager, userManager, appSettings)
        {
            this.mailSendSender = mailSendSender;
            this.logger = logger;
        }
        public async Task RegisterAsync(RegisterViewModel model, string ConfirmEmailLink)
        {

            var user = new AppUser {
                UserName = model.Email,
                Email = model.Email,
                IsActive=true 
            };
            var resultCreate = await userManager.CreateAsync(user, model.Password);

            if (resultCreate.Succeeded)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                string codeHtmlVersion = HttpUtility.UrlEncode(code);
                var link = ConfirmEmailLink + $"?userId={user.Id}&code={codeHtmlVersion}";

                mailSendSender.Send(new Messaging.Sender.Options.MailModel
                {
                    To = user.Email,
                    Message = $"Please confirm your account by clicking this link: {link}",
                    Subject = "Confirm your account"
                });

                await AddClaimsToNewUser(user);
                await AddRolesToNewUser(user);

            }
            else
                throw new RegistrationFaildException
                    ("Faild to create user with given username and password");

        }
        public async Task ConfirmEmail(string userId,string token)
        {
            if (userId == null || token == null)
                throw new TokenOrUserIdNullException();

            var user = await userManager
                .FindByIdAsync(userId); 

            if (user == null)
                throw new UserNullException();

            if (user.EmailConfirmed)
                throw new EmailAlreadyConfirmedException();

            var result = await userManager
                .ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new EmailFaildToConfirmException(String.Join("; ",result.Errors));


        }
        public async Task ForgotPasswordAsync(RequestResertPasswordModel model, string resetLink)
        {
            var user = await userManager.FindActiveByEmailAsync(model.Email);

            if (user == null)
                throw new UserNullException();

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var email = user.Email;
            var link = $"{resetLink}?code={code}?email={email}";

            mailSendSender.Send(new Messaging.Sender.Options.MailModel
            {
                To = user.Email,
                Message = @$"On your request we are sending you link for Password Reset: {link}
If you didn't request, please ignore this mail.",
                Subject = "Reset Password for account",
            });
        }

        public async Task ResetPasswordAsync(PasswordResetModel model)
        {
            var user = await userManager
                .FindActiveByEmailAsync(model.Email);

            if (user == null)
                throw new UserNullException();

            var result = await userManager
                .ResetPasswordAsync(user, model.Code, model.Password);

            if (result != IdentityResult.Success)
                throw new ResetingPasswordFaildException();
        }

        public async Task BanUser(string userId,string userName,bool superUser)
        {
            var user = await userManager.FindActiveByIdAsync(userId);

            if (user == null || !user.IsActive)
                throw new UserNullException();

            if (userName != user.UserName)
                if (!superUser)
                    throw new UnauthorizedException(); 

            user.IsActive = false;

            await UpdatingValueOfClaimsForBannedUser(user);
        }
        private async Task AddClaimsToNewUser(AppUser user)
        {

            if ((await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(UserClaims.User, "True"))) == IdentityResult.Success)
                logger.LogInformation($"Added claim {UserClaims.User} to new user.");
            else
                logger.LogWarning($"Claim {UserClaims.User} not added to new user.");

            if ((await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(UserClaims.IsBanned, "False"))) == IdentityResult.Success)
                logger.LogInformation($"Added claim {UserClaims.IsBanned} to new user.");
            else
                logger.LogWarning($"Claim {UserClaims.IsBanned} not added to new user.");
        }
        private async Task AddRolesToNewUser(AppUser user)
        {
            if ((await userManager.AddToRoleAsync(user, UserRole.User)) == IdentityResult.Success)
                logger.LogInformation($"Added role {UserRole.User} to new user.");
            else
                logger.LogWarning($"Role {UserRole.User} not added to new user.");
        }
        private async Task UpdatingValueOfClaimsForBannedUser(AppUser user)
        {
            var claim = (await userManager.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == UserClaims.IsBanned);
            if (claim == null)
            {

                if ((await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(UserClaims.IsBanned, "True"))) == IdentityResult.Success)
                    logger.LogInformation($"Added claim {UserClaims.IsBanned} with value true to user.");
            }
            else
                if ((await userManager.ReplaceClaimAsync(user, claim, new System.Security.Claims.Claim(UserClaims.IsBanned, "True"))) == IdentityResult.Success)
                logger.LogInformation($"Updated claim {UserClaims.IsBanned} with value true to user.");
        }


        #region Exceptions
        public class TokenOrUserIdNullException : Exception
        {
            public TokenOrUserIdNullException() : base("Token or UserId are null")
            {
            }
        }
        public class UserNullException : Exception
        {
            public UserNullException() : base("Can't find user with given id.")
            {
            }
        }
        public class EmailAlreadyConfirmedException : Exception
        {
            public EmailAlreadyConfirmedException()
                : base("Email already confirmed.")
            {
            }
        }
        public class EmailFaildToConfirmException : Exception
        {
            public EmailFaildToConfirmException(string errors)
                : base($"Email faild to confirm. {errors}")
            {
            }
        }
        public class ResetingPasswordFaildException : Exception
        {
            public ResetingPasswordFaildException()
                : base("Faild to reset password.")
            {
            }
        }
        public class UnauthorizedException : Exception
        {
            public UnauthorizedException()
                : base()
            {
            } 
        }

        public class RegistrationFaildException : Exception 
        {
            public RegistrationFaildException(Exception exception) 
                : base($"Exeption thrown while tryng to register user: {exception}")
            {
            }
            public RegistrationFaildException(string message)
                : base(message)
            {
            }
        }
        #endregion

    }
}
