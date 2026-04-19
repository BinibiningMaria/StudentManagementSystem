namespace StudentManagementSystem.Features.Data.Models;

public class Course
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Units { get; set; }

    // Navigation properties
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
