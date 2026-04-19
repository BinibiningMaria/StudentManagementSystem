using System.Security.Cryptography;
using System.Text;
using StudentManagementSystem.Features.Data.Enums;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Helpers;
using StudentManagementSystem.Features.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;
using StudentManagementSystem.Features.Services.Interfaces;

namespace StudentManagementSystem.Features.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IStudentRepository _studentRepository;

    public UserService(IUserRepository repository, IStudentRepository studentRepository)
    {
        _repository = repository;
        _studentRepository = studentRepository;
    }

    public async Task<User?> ValidateLoginAsync(string username, string password, UserRole role)
    {
        var user = await _repository.GetByUsernameAsync(username);
        if (user is null) return null;
        if (user.Role != role) return null;

        var hash = HashPassword(password);
        return user.PasswordHash == hash ? user : null;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var existing = await _repository.GetByUsernameAsync(request.Username);
        if (existing is not null)
            throw new InvalidOperationException("Username already exists.");

        Student? student = null;
        if (request.Role == UserRole.Student)
        {
            student = await _studentRepository.AddAsync(new Student
            {
                StudentNumber = GenerateStudentNumber(),
                FullName = PersonNameHelper.BuildFullName(request.FirstName, request.MiddleName, request.Surname, request.Suffix),
                FirstName = request.FirstName.Trim(),
                MiddleName = request.MiddleName.Trim(),
                Surname = request.Surname.Trim(),
                Suffix = request.Suffix.Trim(),
                Gender = request.Gender.Trim(),
                CivilStatus = request.CivilStatus.Trim(),
                Email = request.Email.Trim(),
                Address = request.Address.Trim(),
                Department = request.Department.Trim(),
                Program = request.Program.Trim(),
                YearLevel = request.YearLevel ?? 1,
                InstructorName = request.InstructorName.Trim()
            });
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            PasswordHash = HashPassword(request.Password),
            Role = request.Role,
            StudentId = student?.Id,
            FirstName = request.FirstName.Trim(),
            MiddleName = request.MiddleName.Trim(),
            Surname = request.Surname.Trim(),
            Suffix = request.Suffix.Trim(),
            Gender = request.Gender.Trim(),
            CivilStatus = request.CivilStatus.Trim(),
            Email = request.Role == UserRole.Admin ? string.Empty : request.Email.Trim(),
            Address = request.Role == UserRole.Admin ? string.Empty : request.Address.Trim(),
            MajorProfession = request.Role == UserRole.Admin ? string.Empty : request.MajorProfession.Trim()
        };

        return await _repository.AddAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        await _repository.UpdateAsync(user);
    }

    public async Task UpdateUserWithPasswordAsync(User user, string newPassword)
    {
        user.PasswordHash = HashPassword(newPassword);
        await _repository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {id} was not found.");

        // If the user has a linked student, delete the student record too
        if (user.StudentId.HasValue)
        {
            await _studentRepository.DeleteAsync(user.StudentId.Value);
        }

        await _repository.DeleteAsync(id);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateStudentNumber()
    {
        return $"STU-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}
