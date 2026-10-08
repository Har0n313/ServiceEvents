using ServiceEvents.Application.DTOs.DepartmentDTO;
using ServiceEvents.Application.Interfaces.Repositories;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Domain.Entities;

namespace ServiceEvents.Application.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<DepartmentResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return departments.Select(ToResponse).ToList();
    }

    public async Task<DepartmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        return department is null ? null : ToResponse(department);
    }

    public async Task<DepartmentResponse> CreateAsync(
        DepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);
        var department = new Department(name);
        await _departmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(department);
    }

    public async Task UpdateAsync(
        Guid id,
        DepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with id '{id}' was not found.");
        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(name, id, cancellationToken);
        department.Rename(name);
        await _departmentRepository.UpdateAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureNameIsUniqueAsync(
        string name,
        Guid? exceptId,
        CancellationToken cancellationToken)
    {
        var existing = await _departmentRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null && existing.Id != exceptId)
        {
            throw new InvalidOperationException("Департамент с таким названием уже существует.");
        }
    }

    private static DepartmentResponse ToResponse(Department department)
    {
        return new DepartmentResponse(department.Id, department.Name);
    }
}
