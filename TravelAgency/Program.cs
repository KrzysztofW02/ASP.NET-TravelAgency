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

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
                options.Lockout.MaxFailedAccessAttempts = 5; 
                options.Lockout.AllowedForNewUsers = true;  
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TravelAgencyDbContext>()
            .AddPasswordValidator<CustomPasswordValidator>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15); 
                options.SlidingExpiration = false; 
                options.LoginPath = "/Identity/Account/Login"; 
                options.LogoutPath = "/Identity/Account/Logout"; 
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15); 
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });



            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ITravelAgencyService, TravelAgencyService>();
            builder.Services.AddScoped<ITravelOfferingsRepository, TravelOfferingsRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IValidator<TravelOffering>, TravelOfferingValidator>();
            builder.Services.AddHttpClient();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<TravelAgencyDbContext>();
                    DbInitializer.Initialize(context);

                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var roles = new[] { "Administrator", "Manager", "Member", "TravelAgent" };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var user = await userManager.FindByEmailAsync("Admin@admin.pl");
                    if (user != null && !await userManager.IsInRoleAsync(user, "Administrator"))
                    {
                        await userManager.AddToRoleAsync(user, "Administrator");
                    }
                    user = await userManager.FindByEmailAsync("asdadas@interia.pl");
                    if(user != null && !await userManager.IsInRoleAsync(user, "Administrator"))
                    {
                        await userManager.AddToRoleAsync(user, "Manager");
                    }
                    user = await userManager.FindByEmailAsync("user1@interia.pl");
                    if (user != null && !await userManager.IsInRoleAsync(user, "Member"))
                    {
                        await userManager.AddToRoleAsync(user, "Member");
                    }
                    user = await userManager.FindByEmailAsync("TravelAgent@agent.pl");
                    if (user != null && !await userManager.IsInRoleAsync(user, "TravelAgent"))
                    {
                        await userManager.AddToRoleAsync(user, "TravelAgent");
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
                        using (var scope = app.Services.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<TravelAgencyDbContext>();
                            var userPasswordSettings = dbContext.UserPasswordSettings.FirstOrDefault(x => x.UserId == user.Id);

                            var userPasswordHistory = dbContext.PasswordHistories
                            .Where(p => p.UserId == user.Id)
                            .OrderByDescending(x => x.DateChanged)
                            .FirstOrDefault();
                            var daysWithCurrentPassword = 0.0;
                            var passwordExpirationDays = 30;

                            if (userPasswordHistory != null)
                            {
                                daysWithCurrentPassword = (DateTime.Now - userPasswordHistory.DateChanged ).TotalDays;
                            }
                            if (daysWithCurrentPassword >= userPasswordSettings.PasswordExpirationDays)
                            {
                                context.Response.Redirect("/Identity/Account/Manage/ChangePassword");
                                return;
                            }

                            if (userPasswordSettings.IsPasswordChangeRequired && !context.Request.Path.StartsWithSegments("/Identity/Account/Manage/ChangePassword"))
                            {
                                context.Response.Redirect("/Identity/Account/Manage/ChangePassword");
                                return;
                            }
                        }
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

            app.UseSession();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
