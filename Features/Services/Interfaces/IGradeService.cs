using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Services.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<Grade>> GetAllGradesAsync();
    Task<Grade?> GetGradeByIdAsync(int id);
    Task<Grade> AddGradeAsync(Grade grade);
    Task UpdateGradeAsync(Grade grade);
    Task DeleteGradeAsync(int id);
    Task<Grade?> GetGradeByEnrollmentAsync(int enrollmentId);
    Task<IEnumerable<Grade>> GetGradesByStudentAsync(int studentId);
}
