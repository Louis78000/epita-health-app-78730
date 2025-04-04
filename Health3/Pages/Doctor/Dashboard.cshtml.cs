using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Health3.Pages.Doctor;

[Authorize(Roles = "Doctor")] // Restreint l'accès uniquement aux docteurs
public class DashboardModel : PageModel
{
    public string DoctorName { get; set; }
    public int AppointmentCount { get; set; }

    public void OnGet()
    {
        
        // Simuler les données pour l'exemple
        DoctorName = "Dr. Jean Dupont"; // Vous pouvez récupérer cela dynamiquement via User.Identity.Name
        AppointmentCount = 5; // À remplacer par une requête réelle pour compter les rendez-vous
    }
}