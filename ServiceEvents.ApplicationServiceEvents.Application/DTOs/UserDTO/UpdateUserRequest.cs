namespace ServiceEvents.Application.DTOs.UserDTO
{
    public sealed record UpdateUserRequest(
    Guid UserId,
    string FullName,
    string Department,
    string Position);
}
