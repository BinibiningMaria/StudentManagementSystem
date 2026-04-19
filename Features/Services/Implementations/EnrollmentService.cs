using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;
using StudentManagementSystem.Features.Services.Interfaces;

namespace StudentManagementSystem.Features.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;

    public EnrollmentService(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment)
    {
        return await _repository.AddAsync(enrollment);
    }

    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        await _repository.UpdateAsync(enrollment);
    }

    public async Task DeleteEnrollmentAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentAsync(int studentId)
    {
        return await _repository.GetByStudentAsync(studentId);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _repository.GetByCourseAsync(courseId);
    }
}
