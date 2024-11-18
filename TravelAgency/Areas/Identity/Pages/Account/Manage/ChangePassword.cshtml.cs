// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TravelAgency.Models;

namespace TravelAgency.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly TravelAgencyDbContext _context;
        private readonly string _captchaSecret = "6Lf4ZnwqAAAAAB7QzyXQQKD9ZEQLdEk2lmzax-WP";

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            TravelAgencyDbContext context)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            new CreateAssessmentSample().createAssessment();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var passwordSettings = _context.UserPasswordSettings.FirstOrDefault(x => x.UserId == user.Id);
            var UserPasswordHistory = _context.PasswordHistories.Where(p => p.UserId == user.Id).OrderBy(x => x.DateChanged).Take(passwordSettings.PasswordHistoryLimit);
            foreach (var password in UserPasswordHistory)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(user, password.PasswordHash, Input.NewPassword);
                if (result == PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "You cannot use a password that you have used before.");
                    return Page();
                }
            }
            if (passwordSettings.PasswordLengthRequired > Input.NewPassword.Length)
            {
                ModelState.AddModelError(string.Empty, $"The new password must be at least {passwordSettings.PasswordLengthRequired} characters long.");
                return Page();
            }
            if (passwordSettings.PasswordNumbersRequired > Input.NewPassword.Count(char.IsDigit))
            {
                ModelState.AddModelError(string.Empty, $"The new password must have at least {passwordSettings.PasswordNumbersRequired} digits.");
                return Page();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var forcePasswordChangeClaim = claims.FirstOrDefault(c => c.Type == "ForcePasswordChange" && c.Value == "true");
            if (forcePasswordChangeClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, forcePasswordChangeClaim);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            _context.PasswordHistories.Add(new PasswordHistory
            {
                PasswordHash = _userManager.PasswordHasher.HashPassword(user, Input.NewPassword),
                DateChanged = DateTime.Now,
                UserId = user.Id
            });
            passwordSettings.IsPasswordChangeRequired = false;
            _context.UserPasswordSettings.Update(passwordSettings);
            _context.UserActivityLog.Add(new UserActivityLog(user.Id, user.Email, "User logged in"));
            _context.SaveChanges();

            return RedirectToPage();
        }
        private async Task<bool> ValidateReCaptcha(string recaptchaResponse)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={_captchaSecret}&response={recaptchaResponse}",
                    null);

                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                return result.success == "true";
            }
        }

        public class CreateAssessmentSample
        {
            // Utwórz ocenę, aby przeanalizować ryzyko związane z działaniem w interfejsie użytkownika.
            // projectID: Identyfikator Twojego projektu Google Cloud.
            // recaptchaKey: Klucz reCAPTCHA powiązany z witryną lub aplikacją
            // token: Wygenerowany token uzyskany od klienta.
            // recaptchaAction: Nazwa działania odpowiadająca tokenowi.
            public void createAssessment(string projectID = "cyberbezpieczens-1731409320128", string recaptchaKey = "6Lf4ZnwqAAAAAPqMtIpOiqADtQlzfpU8scHGq0DX", string token = "action-token", string recaptchaAction = "action-name")
            {
                // Utwórz klienta reCAPTCHA.
                // DO ZROBIENIA: zapisz kod klienta w pamięci podręcznej (zalecane) lub wywołaj client.close() przed wyjściem z tej metody.
                RecaptchaEnterpriseServiceClient client = RecaptchaEnterpriseServiceClient.Create();

                ProjectName projectName = new ProjectName(projectID);

                // Utwórz żądanie oceny.
                CreateAssessmentRequest createAssessmentRequest = new CreateAssessmentRequest()
                {
                    Assessment = new Assessment()
                    {
                        // Ustaw właściwości zdarzenia do śledzenia.
                        Event = new Event()
                        {
                            SiteKey = recaptchaKey,
                            Token = token,
                            ExpectedAction = recaptchaAction
                        },
                    },
                    ParentAsProjectName = projectName
                };

                Assessment response = client.CreateAssessment(createAssessmentRequest);

                // Sprawdź, czy token jest prawidłowy.
                if (response.TokenProperties.Valid == false)
                {
                    System.Console.WriteLine("The CreateAssessment call failed because the token was: " +
                        response.TokenProperties.InvalidReason.ToString());
                    return;
                }

                // Sprawdź, czy oczekiwane działanie zostało wykonane.
                if (response.TokenProperties.Action != recaptchaAction)
                {
                    System.Console.WriteLine("The action attribute in reCAPTCHA tag is: " +
                        response.TokenProperties.Action.ToString());
                    System.Console.WriteLine("The action attribute in the reCAPTCHA tag does not " +
                        "match the action you are expecting to score");
                    return;
                }

                // Uzyskaj ocenę ryzyka i jego przyczyny.
                // Więcej informacji o interpretowaniu testu znajdziesz tutaj:
                // https://cloud.google.com/recaptcha-enterprise/docs/interpret-assessment
                System.Console.WriteLine("The reCAPTCHA score is: " + ((decimal)response.RiskAnalysis.Score));

                foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
                {
                    System.Console.WriteLine(reason.ToString());
                }
            }
        }

    }
}
