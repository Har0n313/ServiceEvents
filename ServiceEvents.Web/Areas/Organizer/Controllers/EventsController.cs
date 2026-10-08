using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Domain.Enums;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Organizer.Controllers;

[Area("Organizer")]
[Authorize(Roles = "Organizer")]
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IEventRegistrationService _registrationService;
    private readonly IUserService _userService;

    public EventsController(
        IEventService eventService,
        IEventRegistrationService registrationService,
        IUserService userService)
    {
        _eventService = eventService;
        _registrationService = registrationService;
        _userService = userService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return Forbid();
        }

        var events = await _eventService.GetByOrganizerIdAsync(
            organizerId,
            cancellationToken);

        return View(events
            .OrderByDescending(eventItem => eventItem.CreatedAt)
            .ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(CreateEventViewModel.FromRequest(new CreateEventRequest(
            string.Empty,
            string.Empty,
            DateTime.Now.AddDays(1),
            null,
            null,
            null,
            null)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEventViewModel model,
        CancellationToken cancellationToken,
        int? startDateOffsetMinutes = null,
        int? endDateOffsetMinutes = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!model.TryParseDates(out var startDate, out var endDate))
        {
            ModelState.AddModelError(
                nameof(model.StartDate),
                "Укажите дату и время в формате дд.мм.гггг чч:мм.");
            if (!string.IsNullOrWhiteSpace(model.EndDate))
            {
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "Укажите дату и время в формате дд.мм.гггг чч:мм.");
            }

            return View(model);
        }

        if (!TryGetOrganizerId(out var organizerId))
        {
            return Forbid();
        }

        var request = model.ToRequest(
            ConvertLocalDateToUtc(startDate, startDateOffsetMinutes),
            endDate is { } localEndDate
                ? ConvertLocalDateToUtc(localEndDate, endDateOffsetMinutes ?? startDateOffsetMinutes)
                : null);

        await _eventService.CreateAsync(request, organizerId, cancellationToken);
        TempData["Message"] = "Мероприятие создано.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var eventItem = await GetOwnedEventAsync(id, cancellationToken);
        return eventItem is null ? NotFound() : View(eventItem);
    }

    public async Task<IActionResult> Participants(Guid id, CancellationToken cancellationToken)
    {
        var eventItem = await GetOwnedEventAsync(id, cancellationToken);
        if (eventItem is null)
        {
            return NotFound();
        }

        var registrations = await _registrationService.GetByEventIdAsync(id, cancellationToken);
        var users = await _userService.GetAllAsync(cancellationToken);
        var usersById = users.ToDictionary(user => user.UserId);

        var participants = registrations
            .OrderByDescending(registration => registration.RegisteredAt)
            .Select(registration => new OrganizerParticipantViewModel(
                usersById.TryGetValue(registration.UserId, out var user)
                    ? user.FullName
                    : "Пользователь не найден",
                registration.RegisteredAt,
                registration.Status))
            .ToList();

        return View(new OrganizerParticipantsViewModel(eventItem, participants));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var eventItem = await GetOwnedEventAsync(id, cancellationToken);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View(new UpdateEventRequest(
            eventItem.EventId,
            eventItem.Title,
            eventItem.Description,
            DateTime.SpecifyKind(eventItem.StartDate, DateTimeKind.Utc).ToLocalTime(),
            eventItem.EndDate is { } endDate
                ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc).ToLocalTime()
                : null,
            eventItem.Location,
            eventItem.MaxParticipants,
            eventItem.ImagePath));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateEventRequest request,
        int? startDateOffsetMinutes,
        int? endDateOffsetMinutes,
        CancellationToken cancellationToken)
    {
        var eventItem = await GetOwnedEventAsync(request.EventId, cancellationToken);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        request = request with
        {
            StartDate = ConvertLocalDateToUtc(request.StartDate, startDateOffsetMinutes),
            EndDate = request.EndDate is { } endDate
                ? ConvertLocalDateToUtc(endDate, endDateOffsetMinutes ?? startDateOffsetMinutes)
                : null
        };

        await _eventService.UpdateAsync(request.EventId, request, cancellationToken);
        TempData["Message"] = "Изменения сохранены.";
        return RedirectToAction(nameof(Details), new { id = request.EventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        return UpdateStatusAsync(id, EventStatus.Draft, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> CloseRegistration(Guid id, CancellationToken cancellationToken)
    {
        return UpdateStatusAsync(id, EventStatus.RegistrationClosed, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        return UpdateStatusAsync(id, EventStatus.Cancelled, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (await GetOwnedEventAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        await _eventService.DeleteAsync(id, cancellationToken);
        TempData["Message"] = "Мероприятие удалено.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> UpdateStatusAsync(
        Guid id,
        EventStatus status,
        CancellationToken cancellationToken)
    {
        var eventItem = await GetOwnedEventAsync(id, cancellationToken);
        if (eventItem is null)
        {
            return NotFound();
        }

        switch (status)
        {
            case EventStatus.Draft:
                await _eventService.PublishAsync(id, cancellationToken);
                TempData["Message"] = "Мероприятие опубликовано.";
                break;
            case EventStatus.RegistrationClosed:
                await _eventService.CloseRegistrationAsync(id, cancellationToken);
                TempData["Message"] = "Регистрация закрыта.";
                break;
            case EventStatus.Cancelled:
                await _eventService.CancelAsync(id, cancellationToken);
                TempData["Message"] = "Мероприятие отменено.";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, "Неподдерживаемый статус мероприятия.");
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<ServiceEvents.Application.DTOs.EventDTO.EventResponse?> GetOwnedEventAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetOrganizerId(out var organizerId))
        {
            return null;
        }

        var eventItem = await _eventService.GetByIdAsync(id, cancellationToken);
        return eventItem?.OrganizerId == organizerId ? eventItem : null;
    }

    private bool TryGetOrganizerId(out Guid organizerId)
    {
        return Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out organizerId);
    }

    private static DateTime ConvertLocalDateToUtc(DateTime date, int? offsetMinutes)
    {
        var localDate = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
        var effectiveOffset = offsetMinutes
            ?? (int)TimeZoneInfo.Local.GetUtcOffset(localDate).TotalMinutes;

        return DateTime.SpecifyKind(
            localDate.AddMinutes(effectiveOffset),
            DateTimeKind.Utc);
    }
}
