using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByStudentIdAsync(int studentId);
    Task UpdateAsync(User user);
    Task DeleteByStudentIdAsync(int studentId);
}
