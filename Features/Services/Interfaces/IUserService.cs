using StudentManagementSystem.Features.Data.Enums;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Models;

namespace StudentManagementSystem.Features.Services.Interfaces;

public interface IUserService
{
    Task<User?> ValidateLoginAsync(string username, string password, UserRole role);
    Task<User> RegisterAsync(RegisterRequest request);
    Task<User?> GetUserByIdAsync(int id);
}
