using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityService.Data.Models;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityService.Contollers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AuthController> logger;
        private readonly IUserRepository userRepository;
        private readonly IIdentityServerInteractionService interactionService;

        public AuthController(
            SignInManager<AppUser>  signInManager,
            UserManager<AppUser> userManager,
            ILogger<AuthController> logger,
            IUserRepository userRepository,
            IIdentityServerInteractionService interactionService
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.userRepository = userRepository;
            this.interactionService = interactionService;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl) 
        {
            return View(new LoginViewModel {ReturnUrl=returnUrl });
        }
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId) 
        {
            await signInManager.SignOutAsync();

            var logoutRequest = await interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
                return RedirectToAction("Index", "Home");
            
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            var result= await signInManager.PasswordSignInAsync(vm.Email, vm.Password, false, false);
            if (result.Succeeded)
            {
                return Redirect(vm.ReturnUrl??"/index.html");
            }
            else if (result.IsLockedOut)
            {

            }
            return View();
        }
        public IActionResult Register(string returnUrl) 
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                logger.LogInformation("Registering started");

                var confirmLink = Url.Link("ConfirmEmail", new { });

                await userRepository.RegisterAsync(vm, confirmLink);
                return Redirect(vm.ReturnUrl ?? "/index.html");
            }
            catch (Exception ex)
            {
                logger.LogError("Registering faild");
            }

            return View(vm);
        }

        [HttpGet("[controller]/[action]", Name = "ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                logger.LogInformation("Confirming email");
                await userRepository.ConfirmEmail(userId, code);

                return View();
            }
            catch (Exception ex)
            {
                logger.LogError("Faild to confirm email", ex);
                return BadRequest(ex.Message);
            }
        }

        // reset password
        public async Task<IActionResult> ForgotPassword(RequestResertPasswordModel model) 
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var link = Url.Link("ResetPassword", new { });

                logger.LogInformation("User called forgot password.");
                await userRepository.ForgotPasswordAsync(model,link);
                return Redirect("/index.html");
            }
            catch (Exception ex)
            {
                logger.LogError($"Exeption thrown while generating reset password link: {ex}");
                return View(model);
            }
        }
        [HttpPost("[controller]/[action]", Name = "ResetPassword")]
        public IActionResult ResetPassword(string code, string email)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("message", nameof(code));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("message", nameof(email));
            }

            var model = new PasswordResetModel() { Code = code, Email = email };
            try
            {
                //Maybe check if code is valid / or not
                logger.LogInformation("User sent to fill form for reset password");
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exeption thrown while generating reset password link: {ex}");
                return View(model);
            }

        }


        public async Task<IActionResult> ResetingPassword(PasswordResetModel model) 
        {

            if (!ModelState.IsValid)
                return View(model);
            try
            {
                logger.LogInformation("Reseting password for user");
                await userRepository.ResetPasswordAsync(model);
                return View("Password have been reset successfully");
            }
            catch (Exception ex)
            {
                logger.LogError($"Exeption thrown while generating reset password link: {ex}");
                return View("Faild to reset password.");
            }

        }
    }
}
