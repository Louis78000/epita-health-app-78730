using Health3.Data;
using Health3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin, Doctor, Patient")]
public class SeeAppointmentsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public SeeAppointmentsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<AppointmentViewModel> Appointments { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Appointments = new List<AppointmentViewModel>();
            return;
        }

        // Detect what role is connected
        var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");

        // Initial request
        IQueryable<Appointment> query = _context.Appointments.Include(a => a.Doctor);

        if (isDoctor)
        {
            // Filter the appointments to show only the connected doctor's appointments.
            query = query.Where(a => a.Doctor.Email == user.Email);
        }

        // Load appointments with the filter
        Appointments = await query
            .Select(a => new AppointmentViewModel
            {
                Title = $"{a.PatientName} - Dr. {a.Doctor.FirstName} {a.Doctor.LastName}",
                Start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss")
            })
            .ToListAsync();
    }

    public class AppointmentViewModel
    {
        public string Title { get; set; }
        public string Start { get; set; }
    }
}