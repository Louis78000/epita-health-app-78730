using Health3.Models;

public class Appointment
{
    public int Id { get; set; } // Primary key
    public int DoctorId { get; set; } // Stranger key to the doctor
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }

    public Doctor Doctor { get; set; } // Navigation to the doctor
}