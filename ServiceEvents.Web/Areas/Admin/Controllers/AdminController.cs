using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IEventRegistrationService _registrationService;

        public AdminController(
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
            var events = (await _eventService.GetAllAsync(cancellationToken)).ToList();
            var users = (await _userService.GetAllAsync(cancellationToken)).ToList();

            var registrationsCount = 0;
            foreach (var ev in events)
            {
                var regs = await _registrationService.GetByEventIdAsync(ev.EventId, cancellationToken);
                registrationsCount += regs?.Count ?? 0;
            }

            var recentEvents = events
                .OrderByDescending(e => e.CreatedAt)
                .Take(6)
                .ToList();

            var recentUsers = users
                .OrderByDescending(u => u.UserId)
                .Take(6)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                EventsCount = events.Count,
                UsersCount = users.Count,
                RegistrationsCount = registrationsCount,
                RecentEvents = recentEvents,
                RecentUsers = recentUsers
            };

            return View(model);
        }
    }
}
