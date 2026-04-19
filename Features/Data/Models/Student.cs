namespace StudentManagementSystem.Features.Data.Models;

public class Student
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public int YearLevel { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Course? Course { get; set; }
    public User? User { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
