using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;

namespace StudentManagementSystem.Features.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public StudentRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Students
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Students
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> AddAsync(Student student)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
        if (existingStudent is null)
        {
            throw new KeyNotFoundException($"Student with ID {student.Id} was not found.");
        }

        context.Entry(existingStudent).CurrentValues.SetValues(student);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student is not null)
        {
            context.Students.Remove(student);
            await context.SaveChangesAsync();
        }
    }
}
