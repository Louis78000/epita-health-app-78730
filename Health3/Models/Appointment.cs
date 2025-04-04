using Health3.Models;

public class Appointment
{
    public int Id { get; set; } // Clé primaire
    public int DoctorId { get; set; } // Clé étrangère vers le docteur
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }

    public Doctor Doctor { get; set; } // Navigation vers le docteur
}