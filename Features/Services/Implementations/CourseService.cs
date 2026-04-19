using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;
using StudentManagementSystem.Features.Services.Interfaces;

namespace StudentManagementSystem.Features.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repository;

    public CourseService(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Course> AddCourseAsync(Course course)
    {
        return await _repository.AddAsync(course);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
