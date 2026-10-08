using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Domain.Enums;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Employee.Controllers;

[Area("Employee")]
[Authorize(Roles = "Employee")]
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IEventRegistrationService _registrationService;
    private readonly IUserService _userService;
    private readonly IEventPropertyService _propertyService;

    public EventsController(
        IEventService eventService,
        IEventRegistrationService registrationService,
        IUserService userService,
        IEventPropertyService propertyService)
    {
        _eventService = eventService;
        _registrationService = registrationService;
        _userService = userService;
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!TryGetEmployeeId(out var employeeId))
        {
            return Forbid();
        }

        var employee = await _userService.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return Forbid();
        }

        var events = await _eventService.GetAllAsync(cancellationToken);
        var registrations = await _registrationService.GetByUserIdAsync(employeeId, cancellationToken);
        var registrationStatuses = registrations.ToDictionary(
            registration => registration.EventId,
            registration => registration.Status);
        var propertiesByEvent = await GetEventPropertiesAsync(
            events.Where(eventItem => eventItem.Status == EventStatus.Published
                && eventItem.DepartmentId == employee.DepartmentId)
                .Select(eventItem => eventItem.EventId),
            cancellationToken);

        var model = events
            .Where(eventItem => eventItem.Status == EventStatus.Published)
            .Where(eventItem => eventItem.DepartmentId == employee.DepartmentId)
            .OrderBy(eventItem => eventItem.StartDate)
            .Select(eventItem => new EmployeeEventViewModel(
                eventItem,
                registrationStatuses.TryGetValue(eventItem.EventId, out var status) ? status : null,
                propertiesByEvent.GetValueOrDefault(eventItem.EventId, [])))
            .ToList();

        return View(model);
    }

    public async Task<IActionResult> MyRegistrations(CancellationToken cancellationToken)
    {
        if (!TryGetEmployeeId(out var employeeId))
        {
            return Forbid();
        }

        var registrations = await _registrationService.GetByUserIdAsync(employeeId, cancellationToken);
        var events = await _eventService.GetAllAsync(cancellationToken);
        var eventsById = events.ToDictionary(eventItem => eventItem.EventId);

        var model = registrations
            .Where(registration => registration.Status == RegistrationStatus.Active)
            .Where(registration => eventsById.ContainsKey(registration.EventId))
            .OrderByDescending(registration => registration.RegisteredAt)
            .Select(registration => new EmployeeRegistrationViewModel(
                eventsById[registration.EventId],
                registration.Status,
                registration.RegisteredAt))
            .ToList();

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var eventItem = await _eventService.GetByIdAsync(id, cancellationToken);
        if (eventItem is null || eventItem.Status != EventStatus.Published)
        {
            return NotFound();
        }

        if (!TryGetEmployeeId(out var employeeId))
        {
            return Forbid();
        }

        var employee = await _userService.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null
            || eventItem.DepartmentId != employee.DepartmentId)
        {
            return NotFound();
        }

        var registrations = await _registrationService.GetByUserIdAsync(employeeId, cancellationToken);
        var registration = registrations.FirstOrDefault(item => item.EventId == id);

        var propertiesByEvent = await GetEventPropertiesAsync([id], cancellationToken);
        return View(new EmployeeEventViewModel(
            eventItem,
            registration?.Status,
            propertiesByEvent.GetValueOrDefault(id, [])));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetEmployeeId(out var employeeId))
        {
            return Forbid();
        }

        try
        {
            await _registrationService.RegisterAsync(id, employeeId, cancellationToken);
            TempData["Message"] = "Вы записались на мероприятие.";
        }
        catch (InvalidOperationException)
        {
            TempData["Error"] = "Не удалось записаться: регистрация закрыта, мест больше нет или вы уже записаны.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelRegistration(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetEmployeeId(out var employeeId))
        {
            return Forbid();
        }

        try
        {
            await _registrationService.CancelAsync(id, employeeId, cancellationToken);
            TempData["Message"] = "Регистрация отменена.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(MyRegistrations));
    }

    private bool TryGetEmployeeId(out Guid employeeId)
    {
        return Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out employeeId);
    }

    private async Task<Dictionary<Guid, IReadOnlyCollection<EventPropertyValueViewModel>>> GetEventPropertiesAsync(
        IEnumerable<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, IReadOnlyCollection<EventPropertyValueViewModel>>();
        foreach (var eventId in eventIds)
        {
            var values = await _propertyService.GetValuesByEventIdAsync(eventId, cancellationToken);
            var properties = await _propertyService.GetByEventIdAsync(eventId, cancellationToken);
            var namesById = properties.ToDictionary(property => property.PropertyId, property => property.Name);
            result[eventId] = values
                .Where(value => namesById.ContainsKey(value.PropertyId))
                .Select(value => new EventPropertyValueViewModel(
                    namesById[value.PropertyId],
                    value.Value))
                .ToList();
        }

        return result;
    }
}
