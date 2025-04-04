using Health3.Data;
using Health3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health3.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // Redirect based on role
                    if (roles.Contains("Doctor"))
                    {
                        return RedirectToAction("Index", "DoctorDashboard"); // Redirect to Doctor's dashboard
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "PatientDashboard");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Log out the user
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}