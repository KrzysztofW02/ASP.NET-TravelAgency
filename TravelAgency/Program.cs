using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using TravelAgency.Models;
using TravelAgency.Data;
using TravelAgency.Repository;
using TravelAgency.Controllers;
using TravelAgency.Services;
using AutoMapper;
using FluentValidation;
using System;
using Microsoft.AspNetCore.Identity;

namespace TravelAgency
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TravelAgencyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TravelAgencyDbContext>()
            .AddPasswordValidator<CustomPasswordValidator>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ITravelAgencyService, TravelAgencyService>();
            builder.Services.AddScoped<ITravelOfferingsRepository, TravelOfferingsRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IValidator<TravelOffering>, TravelOfferingValidator>();



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<TravelAgencyDbContext>();
                    DbInitializer.Initialize(context);

                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var roles = new[] { "Administrator", "Manager", "Member" };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var user = await userManager.FindByEmailAsync("Admin@UBBTravelAgency.pl");
                    if (user != null && !await userManager.IsInRoleAsync(user, "Administrator"))
                    {
                        await userManager.AddToRoleAsync(user, "Administrator");
                    }
                    user = await userManager.FindByEmailAsync("asdadas@interia.pl");
                    if(user != null && !await userManager.IsInRoleAsync(user, "Manager"))
                    {
                        await userManager.AddToRoleAsync(user, "Manager");
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                if (context.User.Identity.IsAuthenticated)
                {
                    var user = await userManager.GetUserAsync(context.User);
                    if (user != null)
                    {
                        var forcePasswordChangeClaim = (await userManager.GetClaimsAsync(user))
                            .FirstOrDefault(c => c.Type == "ForcePasswordChange" && c.Value == "true");

                        if (forcePasswordChangeClaim != null && !context.Request.Path.StartsWithSegments("/Identity/Account/Manage/ChangePassword"))
                        {
                            context.Response.Redirect("/Identity/Account/Manage/ChangePassword");
                            return;
                        }
                    }
                }

                await next();
            });

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
