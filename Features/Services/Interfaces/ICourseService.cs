using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseByIdAsync(int id);
    Task<Course> AddCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(int id);
}
