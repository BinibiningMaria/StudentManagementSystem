using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;

namespace StudentManagementSystem.Features.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CourseRepository(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Courses
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == id);
    }

    public async Task<Course> AddAsync(Course course)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Courses.Add(course);
        await context.SaveChangesAsync();
        return course;
    }

    public async Task UpdateAsync(Course course)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingCourse = await context.Courses.FirstOrDefaultAsync(c => c.Id == course.Id);
        if (existingCourse is null)
        {
            throw new KeyNotFoundException($"Course with ID {course.Id} was not found.");
        }

        context.Entry(existingCourse).CurrentValues.SetValues(course);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is not null)
        {
            context.Courses.Remove(course);
            await context.SaveChangesAsync();
        }
    }
}
