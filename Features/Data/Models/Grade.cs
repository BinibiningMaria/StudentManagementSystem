namespace StudentManagementSystem.Features.Data.Models;

public class Grade
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public decimal GradeValue { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime GradedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public Enrollment? Enrollment { get; set; }
}
