using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Health3.Data;
using Health3.Models;

[Authorize(Roles = "Admin")]
public class SeeDoctorsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SeeDoctorsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Doctor> Doctors { get; set; }

    public async Task OnGetAsync()
    {
        // Pick the doctor's list
        Doctors = await _context.Doctors.ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // Find the doctor
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }

        // Reload the list after deleting
        return RedirectToPage();
    }
}