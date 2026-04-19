using System.ComponentModel.DataAnnotations.Schema;
using StudentManagementSystem.Features.Data.Enums;

namespace StudentManagementSystem.Features.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string MajorProfession { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Student? Student { get; set; }

    [NotMapped]
    public string FullName =>
        string.Join(" ", new[] { FirstName, MiddleName, Surname, Suffix }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
