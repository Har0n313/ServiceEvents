using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Application.Interfaces.Services;

namespace ServiceEvents.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllAsync(cancellationToken);
            return View(events);
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var ev = await _eventService.GetByIdAsync(id, cancellationToken);
            if (ev == null) return NotFound();
            return View(ev);
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var ev = await _eventService.GetByIdAsync(id, cancellationToken);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEventRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);
            await _eventService.UpdateAsync(request.EventId, request, cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _eventService.DeleteAsync(id, cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
