using StudentManagementSystem.Features.Data.Enums;

namespace StudentManagementSystem.Features.Models;

public class RegisterRequest
{
    public UserRole Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public int? YearLevel { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string MajorProfession { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
