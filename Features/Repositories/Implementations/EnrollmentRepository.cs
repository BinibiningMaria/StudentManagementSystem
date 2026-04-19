using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;

namespace StudentManagementSystem.Features.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment> AddAsync(Enrollment enrollment)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();
        return enrollment;
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingEnrollment = await context.Enrollments.FirstOrDefaultAsync(e => e.Id == enrollment.Id);
        if (existingEnrollment is null)
        {
            throw new KeyNotFoundException($"Enrollment with ID {enrollment.Id} was not found.");
        }

        context.Entry(existingEnrollment).CurrentValues.SetValues(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var enrollment = await context.Enrollments
            .Include(e => e.Grade)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (enrollment is not null)
        {
            context.Enrollments.Remove(enrollment);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseAsync(int courseId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Grade)
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
    }
}
