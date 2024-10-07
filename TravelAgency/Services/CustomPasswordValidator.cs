using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace TravelAgency.Services
{
    public class CustomPasswordValidator : IPasswordValidator<IdentityUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user, string password)
        {
            int digitCount = password.Count(char.IsDigit);

            if (digitCount < 2)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordRequiresTwoDigits",
                    Description = "Passwords must have at least two digits."
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
