using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Health3.Data;
using Health3.Models;

[Authorize(Roles = "Admin")]
public class EditDoctorModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditDoctorModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Doctor Doctor { get; set; } // Doctor's data

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Pick the doctor from the database
        Doctor = await _context.Doctors.FindAsync(id);
        if (Doctor == null)
        {
            return NotFound(); // return 404 error if the doctor do not exist
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        
        if (!ModelState.IsValid)
        {
            return Page(); // if the model is invalid, stay on the page
        }

        // Pick the doctor from the database
        var doctorInDb = await _context.Doctors.FindAsync(Doctor.Id);
        if (doctorInDb == null)
        {
            return NotFound(); // return 404 error if the doctor do not exist
        }

        // Update doctor information
        doctorInDb.FirstName = Doctor.FirstName;
        doctorInDb.LastName = Doctor.LastName;
        doctorInDb.Email = Doctor.Email;
        await _context.SaveChangesAsync();

        //Redirect to the SeeDoctors page after the save
        return RedirectToPage("/Admin/SeeDoctors");
    }
}