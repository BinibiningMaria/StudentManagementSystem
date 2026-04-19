using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;

namespace StudentManagementSystem.Features.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Users
            .AsNoTracking()
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> AddAsync(User user)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Users
            .AsNoTracking()
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByStudentIdAsync(int studentId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Users
            .AsNoTracking()
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.StudentId == studentId);
    }

    public async Task UpdateAsync(User user)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new KeyNotFoundException($"User with ID {user.Id} was not found.");
        }

        // Explicitly copy each property to avoid issues with navigation/computed properties
        existingUser.Username = user.Username;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.Role = user.Role;
        existingUser.StudentId = user.StudentId;
        existingUser.FirstName = user.FirstName;
        existingUser.MiddleName = user.MiddleName;
        existingUser.Surname = user.Surname;
        existingUser.Suffix = user.Suffix;
        existingUser.Gender = user.Gender;
        existingUser.CivilStatus = user.CivilStatus;
        existingUser.Email = user.Email;
        existingUser.Address = user.Address;
        existingUser.MajorProfession = user.MajorProfession;

        await context.SaveChangesAsync();
    }

    public async Task DeleteByStudentIdAsync(int studentId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await context.Users.FirstOrDefaultAsync(existingUser => existingUser.StudentId == studentId);
        if (user is not null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Users
            .AsNoTracking()
            .Include(u => u.Student)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is not null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
