using StudentManagementSystem.Features.Data.Models;
using StudentManagementSystem.Features.Repositories.Interfaces;
using StudentManagementSystem.Features.Services.Interfaces;

namespace StudentManagementSystem.Features.Services.Implementations;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _repository;

    public GradeService(IGradeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Grade>> GetAllGradesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Grade?> GetGradeByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Grade> AddGradeAsync(Grade grade)
    {
        return await _repository.AddAsync(grade);
    }

    public async Task UpdateGradeAsync(Grade grade)
    {
        await _repository.UpdateAsync(grade);
    }

    public async Task DeleteGradeAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<Grade?> GetGradeByEnrollmentAsync(int enrollmentId)
    {
        return await _repository.GetByEnrollmentAsync(enrollmentId);
    }

    public async Task<IEnumerable<Grade>> GetGradesByStudentAsync(int studentId)
    {
        return await _repository.GetByStudentAsync(studentId);
    }
}
