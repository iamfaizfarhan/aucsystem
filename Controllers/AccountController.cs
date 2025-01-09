using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Aucsystem.Models;
using System.Threading.Tasks;

namespace Aucsystem.Controllers
{
    public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)  : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string FullName, string Email, string Password, string Phone, string UserType)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = FullName,
                    Email = Email,
                    UserName = Email,
                    PhoneNumber = Phone,
                    UserType = UserType
                };

                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    // Ensure the role exists
                    if (!await _roleManager.RoleExistsAsync(UserType))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserType));
                    }

                    // Assign the role to the user
                    await _userManager.AddToRoleAsync(user, UserType);
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", user.FullName));

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect based on role
                    if (UserType == "Seller")
                        return RedirectToAction("SellerDashboard", "Dashboard");
                    else
                        return RedirectToAction("BuyerDashboard", "Dashboard");
                }

             
            }

            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

         public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user?.UserType == "Seller")
                        return RedirectToAction("SellerDashboard", "Dashboard");
                    else
                        return RedirectToAction("BuyerDashboard", "Dashboard");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "User account is locked out.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            #pragma warning disable CS8601 
            var model = new SettingsViewModel
            {
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Settings");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SettingsViewModel model, IFormFile ProfilePicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (ProfilePicture != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                user.ProfilePicture = "/uploads/" + fileName;
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "Failed to update password.";
                    return RedirectToAction("Settings");
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update settings.";
            }

            return RedirectToAction("Settings");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (profilePicture != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePicture = "/uploads/" + fileName; 
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Profile picture updated.";
            }

            return RedirectToAction("Settings");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Sign out the user
                await _signInManager.SignOutAsync();

                // Delete the user account
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Your account has been deleted successfully.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "There was an error deleting your account.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction("Settings");
        }
    


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

    public class RegisterViewModel
    {
            public string ? FullName { get; set; }
            public string ? Email { get; set; }
            public string ? Phone { get; set; }

            public string Password { get; set; } = String.Empty;
            public string ? UserType { get; set; }

    }
     public class LoginViewModel
    {
       
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
        public class SettingsViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ? ProfilePicture { get; set; }
        public string ? Password { get; set; }
    }
}


