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
    public Doctor Doctor { get; set; } // Propriété pour les données du docteur

    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Récupérer le docteur à partir de la base de données
        Doctor = await _context.Doctors.FindAsync(id);
        if (Doctor == null)
        {
            return NotFound(); // Retourne une erreur 404 si le docteur n'existe pas
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        
        if (!ModelState.IsValid)
        {
            return Page(); // Si le modèle est invalide, rester sur la page
        }

        // Récupérer le docteur dans la base de données
        var doctorInDb = await _context.Doctors.FindAsync(Doctor.Id);
        if (doctorInDb == null)
        {
            return NotFound(); // Retourne une erreur si le docteur n'existe pas
        }

        // Mettre à jour les informations du docteur
        doctorInDb.FirstName = Doctor.FirstName;
        doctorInDb.LastName = Doctor.LastName;
        doctorInDb.Email = Doctor.Email;
        await _context.SaveChangesAsync();

        // Rediriger vers la page SeeDoctors après la sauvegarde
        return RedirectToPage("/Admin/SeeDoctors");
    }
}