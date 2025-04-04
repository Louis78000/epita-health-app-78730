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
        // Récupérer la liste des docteurs
        Doctors = await _context.Doctors.ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // Trouver le docteur correspondant dans la base de données
        var doctor = await _context.Doctors.FindAsync(id);

        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }

        // Recharger la liste des docteurs après suppression
        return RedirectToPage();
    }
}