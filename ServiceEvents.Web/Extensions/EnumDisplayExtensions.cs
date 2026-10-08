using ServiceEvents.Domain.Enums;

namespace ServiceEvents.Web.Extensions;

public static class EnumDisplayExtensions
{
    public static string ToRussian(this EventStatus status) => status switch
    {
        EventStatus.Draft => "Черновик",
        EventStatus.Published => "Опубликовано",
        EventStatus.RegistrationClosed => "Регистрация закрыта",
        EventStatus.Cancelled => "Отменено",
        EventStatus.Completed => "Завершено",
        _ => status.ToString()
    };

    public static string ToRussian(this RegistrationStatus status) => status switch
    {
        RegistrationStatus.Active => "Активна",
        RegistrationStatus.Cancelled => "Отменена",
        _ => status.ToString()
    };

    public static string ToRussian(this UserRole role) => role switch
    {
        UserRole.Employee => "Сотрудник",
        UserRole.Organizer => "Организатор",
        UserRole.Admin => "Администратор",
        _ => role.ToString()
    };

    public static string ToRussian(this PropertyDataType dataType) => dataType switch
    {
        PropertyDataType.String => "Текст",
        PropertyDataType.Boolean => "Да/Нет",
        PropertyDataType.Integer => "Число",
        _ => dataType.ToString()
    };
}
