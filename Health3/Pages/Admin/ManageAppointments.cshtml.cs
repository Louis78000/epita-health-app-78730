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
    private readonly UserManager<IdentityUser> _userManager;

    public ManageAppointmentsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public AppointmentInputModel Appointment { get; set; }

    public List<Doctor> Doctors { get; set; }
    public List<IdentityUser> Patients { get; set; }
    public IdentityUser LoggedInPatient { get; set; }

    public class AppointmentInputModel
    {
        public int DoctorId { get; set; }
        public string PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }

    public async Task OnGetAsync()
    {
        // Initialiser Appointment si null
        if (Appointment == null)
        {
            Appointment = new AppointmentInputModel();
        }

        // Charger la liste des docteurs
        Doctors = await _context.Doctors.ToListAsync();

        // Identifier si l'utilisateur connecté est un patient
        if (User.IsInRole("Admin"))
        {
            Patients = await _context.Users.ToListAsync();
        }
        else if (User.IsInRole("Patient"))
        {
            var userId = _userManager.GetUserId(User);
            LoggedInPatient = await _userManager.FindByIdAsync(userId);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Doctors = await _context.Doctors.ToListAsync();
            if (User.IsInRole("Admin"))
            {
                Patients = await _context.Users.ToListAsync();
            }
            return Page();
        }

        // Vérification des horaires pris
        var conflictingAppointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == Appointment.DoctorId &&
                                      a.AppointmentDate == Appointment.AppointmentDate);

        if (conflictingAppointment != null)
        {
            // Ajouter un message d'erreur
            ModelState.AddModelError(string.Empty, "This appointment slot is already taken by the selected doctor.");
            Doctors = await _context.Doctors.ToListAsync();
            if (User.IsInRole("Admin"))
            {
                Patients = await _context.Users.ToListAsync();
            }
            return Page(); // Renvoie la page avec le message d'erreur
        }

        // Définir le patient selon le rôle de l'utilisateur connecté
        var patientId = User.IsInRole("Patient")
            ? _userManager.GetUserId(User)
            : Appointment.PatientId;

        var newAppointment = new Appointment
        {
            DoctorId = Appointment.DoctorId,
            PatientName = patientId,
            AppointmentDate = Appointment.AppointmentDate
        };

        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();

        return RedirectToPage("SeeAppointments");
    }
}
