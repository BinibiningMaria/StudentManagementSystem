using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Services.Interfaces;

public interface IEnrollmentService
{
    Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
    Task<Enrollment?> GetEnrollmentByIdAsync(int id);
    Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
    Task DeleteEnrollmentAsync(int id);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentAsync(int studentId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId);
}
