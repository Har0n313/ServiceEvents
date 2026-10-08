using System.Globalization;
using ServiceEvents.Application.DTOs.EventPropertyDTO;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ServiceEvents.Web.Models;

public static class EventPropertyValues
{
    public static async Task<IReadOnlyCollection<EventPropertyValueViewModel>> GetDisplayAsync(
        Guid eventId,
        IEventPropertyService propertyService,
        CancellationToken cancellationToken)
    {
        var properties = await propertyService.GetByEventIdAsync(eventId, cancellationToken);
        var values = await propertyService.GetValuesByEventIdAsync(eventId, cancellationToken);
        var propertyNames = properties.ToDictionary(property => property.PropertyId, property => property.Name);

        return values
            .Where(value => propertyNames.ContainsKey(value.PropertyId))
            .Select(value => new EventPropertyValueViewModel(
                propertyNames[value.PropertyId],
                value.Value))
            .ToList();
    }

    public static bool Validate(
        IDictionary<Guid, string> values,
        IReadOnlyCollection<EventPropertyResponse> properties,
        ModelStateDictionary modelState)
    {
        var propertiesById = properties.ToDictionary(property => property.PropertyId);
        var isValid = true;

        foreach (var (propertyId, value) in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (!propertiesById.TryGetValue(propertyId, out var property))
            {
                modelState.AddModelError(
                    nameof(CreateEventViewModel.PropertyValues),
                    "Список характеристик мероприятия изменился. Обновите страницу.");
                isValid = false;
                continue;
            }

            if (property.DataType == PropertyDataType.Integer
                && !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                modelState.AddModelError(
                    $"{nameof(CreateEventViewModel.PropertyValues)}[{propertyId}]",
                    $"Для характеристики «{property.Name}» укажите целое число.");
                isValid = false;
            }
            else if (property.DataType == PropertyDataType.Boolean
                     && value is not ("Да" or "Нет"))
            {
                modelState.AddModelError(
                    $"{nameof(CreateEventViewModel.PropertyValues)}[{propertyId}]",
                    $"Для характеристики «{property.Name}» выберите «Да» или «Нет».");
                isValid = false;
            }
        }

        return isValid;
    }

    public static async Task SaveAsync(
        Guid eventId,
        IDictionary<Guid, string> values,
        IReadOnlyCollection<EventPropertyResponse> properties,
        IEventPropertyService propertyService,
        CancellationToken cancellationToken)
    {
        var validPropertyIds = properties
            .Select(property => property.PropertyId)
            .ToHashSet();

        foreach (var (propertyId, value) in values)
        {
            if (!validPropertyIds.Contains(propertyId))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                await propertyService.RemoveValueAsync(eventId, propertyId, cancellationToken);
                continue;
            }

            await propertyService.SetValueAsync(eventId, propertyId, value.Trim(), cancellationToken);
        }
    }
}
