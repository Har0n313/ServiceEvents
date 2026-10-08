namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record UpdateUserRequest(
    Guid UserId,
    string FullName,
    Guid DepartmentId,
    string Position);
}
