using Health3.Data;
using Health3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin, Patient")]
public class ManageAppointmentsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager; // Ajout pour gérer l'utilisateur connecté

    public ManageAppointmentsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public AppointmentInputModel Appointment { get; set; }

    public List<Doctor> Doctors { get; set; }
    public List<IdentityUser> Patients { get; set; } // Liste des patients (pour l'admin)
    public IdentityUser LoggedInPatient { get; set; } // Patient connecté (si applicable)

    public class AppointmentInputModel
    {
        public int DoctorId { get; set; }
        public string PatientId { get; set; } // Stocke l'ID du patient
        public DateTime AppointmentDate { get; set; }
    }

    public async Task OnGetAsync()
    {
        // Charger la liste des docteurs
        Doctors = await _context.Doctors.ToListAsync();

        // Identifier si l'utilisateur connecté est un patient
        if (User.IsInRole("Admin"))
        {
            Patients = await _context.Users.ToListAsync(); // Charger tous les utilisateurs pour l'admin
        }
        else if (User.IsInRole("Patient"))
        {
            var userId = _userManager.GetUserId(User); // Obtenir l'ID de l'utilisateur connecté
            LoggedInPatient = await _userManager.FindByIdAsync(userId); // Charger les détails du patient
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Doctors = await _context.Doctors.ToListAsync();
            if (User.IsInRole("Admin"))
            {
                Patients = await _context.Users.ToListAsync(); // Charger tous les utilisateurs pour l'admin
            }
            return Page();
        }

        // Définir le patient selon le rôle de l'utilisateur connecté
        var patientId = User.IsInRole("Patient")
            ? _userManager.GetUserId(User) // L'utilisateur connecté est un patient
            : Appointment.PatientId; // L'utilisateur connecté est un admin et choisit le patient

        var newAppointment = new Appointment
        {
            DoctorId = Appointment.DoctorId,
            PatientName = patientId, // Associer le patient
            AppointmentDate = Appointment.AppointmentDate
        };

        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();

        return RedirectToPage("SeeAppointments"); // Redirige vers la page des rendez-vous
    }
}
