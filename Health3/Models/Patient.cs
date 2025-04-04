namespace Health3.Models;

public class Patient
{
    public int Id { get; set; } // Correspond à l'ID de l'utilisateur dans AspNetUsers
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateRegistered { get; set; }
}