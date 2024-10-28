using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.ViewModels;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Models;

[Authorize(Roles = "Administrator")]
public class AdministrationController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly TravelAgencyDbContext _context;

    public AdministrationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, TravelAgencyDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    public IActionResult AddUser()
    {
        return View();
    }

    public async Task<IActionResult> ManageUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var model = new ManageUserRolesViewModel
        {
            UserId = userId,
            UserEmail = user.Email,
            UserRoles = userRoles,
            Roles = _roleManager.Roles.Select(r => r.Name)
        };

        return View(model);
    }
    public async Task<IActionResult> UserLogs()
    {
        var logs = await _context.UserActivityLog.ToListAsync();
        var model = new UserLoggsViewModel
        {
            _userLogs = logs
        };
        return View(model);
    }
    public async Task<IActionResult> ManageUserPassword(string userId)
    {
        var userPasswordSettings = _context.UserPasswordSettings.FirstOrDefault(x => x.UserId == userId);
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ManageUserPasswordViewModel
        {
            UserId = userId,
            UserEmail = user.Email,
            PasswordExpirationDays = userPasswordSettings.PasswordExpirationDays,
            PasswordHistoryLimit = userPasswordSettings.PasswordHistoryLimit,
        };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> ManageUserPassword(ManageUserPasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var userPasswordSettings = _context.UserPasswordSettings.FirstOrDefault(x => x.UserId == model.UserId);
        userPasswordSettings.PasswordExpirationDays = model.PasswordExpirationDays;
        userPasswordSettings.PasswordHistoryLimit = model.PasswordHistoryLimit;
        userPasswordSettings.IsPasswordChangeRequired = model.IsPasswordChangeRequired;
        userPasswordSettings.PasswordLengthRequired = model.PasswordLengthRequired;
        userPasswordSettings.PasswordNumbersRequired = model.PasswordNumbersRequired;

        _context.UserPasswordSettings.Update(userPasswordSettings);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
        {
            return View("NotFound");
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }

        model.SelectedRoles = model.SelectedRoles ?? new List<string>();
        result = await _userManager.AddToRolesAsync(user, model.SelectedRoles);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }
        _context.UserActivityLog.Add(new UserActivityLog(user.Id, user.Email, $"User {user.Email} roles updated by administrator"));

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("ForcePasswordChange", "true"));
                _context.UserActivityLog.Add(new UserActivityLog(user.Id, user.Email, $"User {model.Email} created by administrator"));

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user== null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

}

