using Microsoft.AspNetCore.Identity;
using TravelAgency.Models;

namespace TravelAgency.Services
{
    public class BlockUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TravelAgencyDbContext _context;

        public BlockUserMiddleware(RequestDelegate next, UserManager<IdentityUser> userManager, TravelAgencyDbContext context)
        {
            _next = next;
            _userManager = userManager;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only check for authenticated users
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(context.User);
                var userPasswordSettings = _context.UserPasswordSettings.First(x => x.UserId == user.Id);

                // Check if the user should be blocked (add your custom condition here)
                if (user != null && userPasswordSettings.IsPasswordChangeRequired) // 'IsBlocked' is a flag in your user model
                {
                    var allowedPage = "/Account/ChangePassword"; // Replace with the page you want them to access
                    var path = context.Request.Path;

                    // If the user is trying to access any other page, redirect them to the allowed page
                    if (!path.StartsWithSegments(allowedPage))
                    {
                        context.Response.Redirect(allowedPage);
                        return;
                    }
                }
            }

            // Continue to the next middleware in the pipeline
            await _next(context);
        }
    }
}
