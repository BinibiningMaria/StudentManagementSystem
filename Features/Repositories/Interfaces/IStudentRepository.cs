using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
}
