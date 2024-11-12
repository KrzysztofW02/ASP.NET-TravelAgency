// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TravelAgency.Models;

namespace TravelAgency.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly TravelAgencyDbContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, TravelAgencyDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public string MathQuestion { get; set; }

        [BindProperty]
        public string CaptchaAnswer { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            [Display(Name = "One-Time Password")]
            public double? OneTimePassword { get; set; }
        }

        public double X { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Random random = new Random();
            int xValue = random.Next(1, 101); 
            HttpContext.Session.SetString("XValue", xValue.ToString());
            X = xValue;

            int aValue = 14; 
            int calculatedOtp = (int)Math.Round(aValue * Math.Log(xValue));

            int a = random.Next(1, 10);
            int b = random.Next(1, 10);
            MathQuestion = $"{a} + {b} = ?";
            HttpContext.Session.SetInt32("CaptchaResult", a + b);

            ViewData["CalculatedOtp"] = calculatedOtp;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var signInUser = _signInManager.UserManager.Users.FirstOrDefault(x => x.Email == Input.Email);
            var user = _context.UserPasswordSettings.First(x => x.UserId == signInUser.Id);

            if (ModelState.IsValid)
            {
                int? expectedCaptchaResult = HttpContext.Session.GetInt32("CaptchaResult");
                if (!int.TryParse(CaptchaAnswer, out int providedCaptchaResult) || providedCaptchaResult != expectedCaptchaResult)
                {
                    ModelState.AddModelError(string.Empty, "Invalid CAPTCHA answer.");
                    return Page();
                }

                if (signInUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. User does not exist.");
                    return Page();
                }
                if (user.OneTimePasswordActive == true)
                {
                    if(Input.OneTimePassword == 0 || Input.OneTimePassword == null)
                    {
                        ModelState.AddModelError(string.Empty, "One-Time Password is required.");
                        return Page();
                    }

                    if (!int.TryParse(HttpContext.Session.GetString("XValue"), out int xValue) || xValue == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Session error or invalid X value. Please try logging in again.");
                        return Page();
                    }

                    int aValue = Input.Email.Length;

                    int correctOneTimePassword = (int)Math.Round(aValue * Math.Log(xValue));

                    _logger.LogInformation($"Calculated OTP for x = {xValue} and email length = {aValue}: {correctOneTimePassword}");
                    _logger.LogInformation($"Received OTP: {Input.OneTimePassword}");

                    if (Math.Abs((double)(Input.OneTimePassword - correctOneTimePassword)) > 1)
                    {
                        await _signInManager.UserManager.AccessFailedAsync(signInUser);

                        if (await _signInManager.UserManager.IsLockedOutAsync(signInUser))
                        {
                            _logger.LogWarning("User account locked out due to OTP failures.");
                            return RedirectToPage("./Lockout");
                        }
                        ModelState.AddModelError(string.Empty, "Invalid login attempt. Login or password is inncorrect.");
                        return Page();
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (user.IsPasswordChangeRequired)
                {
                    return RedirectToPage("/Account/Manage/ChangePassword", new { userId = user.Id });
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    _context.UserActivityLog.Add(new UserActivityLog(signInUser.Id, signInUser.Email, "User logged in"));
                    _context.SaveChanges();
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Your account has been locked for 15 minutes due to too many failed login attempts.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _context.UserActivityLog.Add(new UserActivityLog(signInUser.Id, signInUser.Email, "Invalid login attempt"));
                    _context.SaveChanges();
                    return Page();
                }
            }

            return Page();
        }
    }
}
