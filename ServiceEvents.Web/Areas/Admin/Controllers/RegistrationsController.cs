using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RegistrationsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IUserService _userService;
    private readonly IEventRegistrationService _registrationService;

    public RegistrationsController(
        IEventService eventService,
        IUserService userService,
        IEventRegistrationService registrationService)
    {
        _eventService = eventService;
        _userService = userService;
        _registrationService = registrationService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var events = await _eventService.GetAllAsync(cancellationToken);
        var users = await _userService.GetAllAsync(cancellationToken);
        var usersById = users.ToDictionary(user => user.UserId);
        var registrations = new List<AdminRegistrationViewModel>();

        foreach (var ev in events)
        {
            var eventRegistrations = await _registrationService.GetByEventIdAsync(
                ev.EventId,
                cancellationToken);

            registrations.AddRange(eventRegistrations.Select(registration =>
                new AdminRegistrationViewModel(
                    ev.Title,
                    usersById.TryGetValue(registration.UserId, out var user)
                        ? user.FullName
                        : registration.UserId.ToString(),
                    registration.RegisteredAt,
                    registration.Status)));
        }

        return View(registrations
            .OrderByDescending(registration => registration.RegisteredAt)
            .ToList());
    }
}
