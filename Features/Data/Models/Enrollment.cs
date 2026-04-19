namespace StudentManagementSystem.Features.Data.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Enrolled";

    // Navigation properties
    public Student? Student { get; set; }
    public Course? Course { get; set; }
    public Grade? Grade { get; set; }
}
