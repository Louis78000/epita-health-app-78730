using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Health3.Data; // Remplace avec ton namespace
using Health3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("GetAppointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(); // If the user is not connected
        }

        var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");

        IQueryable<Appointment> query = _context.Appointments.Include(a => a.Doctor);

        if (isDoctor)
        {
            //Only print appointments of the doctor connected
            query = query.Where(a => a.Doctor.Email == user.Email);
        }

        var appointments = await query
            .Select(a => new
            {
                id = a.Id,
                title = $"{a.PatientName} - Dr. {a.Doctor.FirstName} {a.Doctor.LastName}",
                start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = "green" //Default color
            })
            .ToListAsync();

        return Ok(appointments); // Return the filtered appointments
    }

    [HttpPost("AddAppointment")]
    public IActionResult AddAppointment([FromBody] Appointment model)
    {
        if (model == null || string.IsNullOrEmpty(model.PatientName) || model.AppointmentDate == DateTime.MinValue)
            return BadRequest("Données invalides");

        _context.Appointments.Add(model);
        _context.SaveChanges();

        return Ok(new { id = model.Id, title = model.PatientName, start = model.AppointmentDate });
    }

    [HttpDelete("DeleteAppointment/{id}")]
    public IActionResult DeleteAppointment(int id)
    {
        var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return NotFound("Rendez-vous non trouvé.");

        _context.Appointments.Remove(appointment);
        _context.SaveChanges();

        return Ok(new { message = "Rendez-vous supprimé avec succès." });
    }
}
