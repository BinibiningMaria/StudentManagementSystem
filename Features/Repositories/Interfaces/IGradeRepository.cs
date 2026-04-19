using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Repositories.Interfaces;

public interface IGradeRepository
{
    Task<IEnumerable<Grade>> GetAllAsync();
    Task<Grade?> GetByIdAsync(int id);
    Task<Grade> AddAsync(Grade grade);
    Task UpdateAsync(Grade grade);
    Task DeleteAsync(int id);
    Task<Grade?> GetByEnrollmentAsync(int enrollmentId);
    Task<IEnumerable<Grade>> GetByStudentAsync(int studentId);
}
