using Health3.Data;
using Health3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class AddDoctorModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AddDoctorModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public DoctorInputModel Doctor { get; set; } // Bind property for Razor Page

    public string SuccessMessage { get; set; } // Message for success notification

    public class DoctorInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public void OnGet()
    {
        // Handle GET request logic (initial page render)
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Map the input to the Doctor entity
        var newDoctor = new Doctor
        {
            FirstName = Doctor.FirstName,
            LastName = Doctor.LastName,
            Email = Doctor.Email
        };

        _context.Doctors.Add(newDoctor);
        await _context.SaveChangesAsync();

        // Create a user for the doctor
        var doctorUser = new IdentityUser
        {
            UserName = Doctor.Email,
            Email = Doctor.Email
        };

        string defaultPassword = "Doctor@123"; // Default password

        var result = await _userManager.CreateAsync(doctorUser, defaultPassword);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(doctorUser, "Doctor");
        }

        SuccessMessage = "Doctor has been saved and linked to a 'Doctor' account.";

        return Page(); // Stay on the page and show the success message
    }
}
