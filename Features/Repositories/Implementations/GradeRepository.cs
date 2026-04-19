using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;

namespace StudentManagementSystem.Features.Repositories.Implementations;

public class GradeRepository : IGradeRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GradeRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Grades
            .AsNoTracking()
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Course)
            .ToListAsync();
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Grades
            .AsNoTracking()
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Course)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Grade> AddAsync(Grade grade)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Grades.Add(grade);
        await context.SaveChangesAsync();
        return grade;
    }

    public async Task UpdateAsync(Grade grade)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingGrade = await context.Grades.FirstOrDefaultAsync(g => g.Id == grade.Id);
        if (existingGrade is null)
        {
            throw new KeyNotFoundException($"Grade with ID {grade.Id} was not found.");
        }

        context.Entry(existingGrade).CurrentValues.SetValues(grade);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var grade = await context.Grades.FirstOrDefaultAsync(g => g.Id == id);
        if (grade is not null)
        {
            context.Grades.Remove(grade);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Grade?> GetByEnrollmentAsync(int enrollmentId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Grades
            .AsNoTracking()
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Course)
            .FirstOrDefaultAsync(g => g.EnrollmentId == enrollmentId);
    }

    public async Task<IEnumerable<Grade>> GetByStudentAsync(int studentId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Grades
            .AsNoTracking()
            .Include(g => g.Enrollment)
                .ThenInclude(e => e!.Course)
            .Where(g => g.Enrollment!.StudentId == studentId)
            .ToListAsync();
    }
}
