using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Health3.Pages.Doctor;

[Authorize(Roles = "Doctor")] // Only doctors
public class DashboardModel : PageModel
{
    public string DoctorName { get; set; }
    public int AppointmentCount { get; set; }

    public void OnGet()
    {

        // Simulate datas for example
        DoctorName = "Dr. Jean Dupont";
        AppointmentCount = 5;
    }
}