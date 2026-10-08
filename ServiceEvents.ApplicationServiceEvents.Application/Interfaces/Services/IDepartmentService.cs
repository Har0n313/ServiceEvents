using ServiceEvents.Application.DTOs.DepartmentDTO;

namespace ServiceEvents.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DepartmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DepartmentResponse> CreateAsync(
        DepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        DepartmentRequest request,
        CancellationToken cancellationToken = default);
}
