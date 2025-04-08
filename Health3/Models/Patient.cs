namespace Health3.Models;

public class Patient
{
    public int Id { get; set; } // User ID in ASP Net users
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateRegistered { get; set; }
}