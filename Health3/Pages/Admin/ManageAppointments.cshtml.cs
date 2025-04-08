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
        // Initialize Appointment if null
        if (Appointment == null)
        {
            Appointment = new AppointmentInputModel();
        }

        // Loading the doctor list
        Doctors = await _context.Doctors.ToListAsync();

        // Identify if the connected is a a patient
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

        // Check the date
        var conflictingAppointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == Appointment.DoctorId &&
                                      a.AppointmentDate == Appointment.AppointmentDate);

        if (conflictingAppointment != null)
        {
            // Add an error message
            ModelState.AddModelError(string.Empty, "This appointment slot is already taken by the selected doctor.");
            Doctors = await _context.Doctors.ToListAsync();
            if (User.IsInRole("Admin"))
            {
                Patients = await _context.Users.ToListAsync();
            }
            return Page(); // Redirect to the page with the message
        }

        // Define the patient with the connected
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
