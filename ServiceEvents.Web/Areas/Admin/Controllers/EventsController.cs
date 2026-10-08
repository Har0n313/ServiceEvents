using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiceEvents.Application.DTOs.EventDTO;
using ServiceEvents.Application.Interfaces.Services;
using System.Security.Claims;
using ServiceEvents.Web.Models;

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
            if (!ModelState.IsValid) return View(model);

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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var organizerId))
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
            var ev = await _eventService.GetByIdAsync(id, cancellationToken);
            if (ev == null) return NotFound();
            return View(ev);
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var ev = await _eventService.GetByIdAsync(id, cancellationToken);
            if (ev == null) return NotFound();
            return View(new UpdateEventRequest(
                ev.EventId,
                ev.Title,
                ev.Description,
                DateTime.SpecifyKind(ev.StartDate, DateTimeKind.Utc).ToLocalTime(),
                ev.EndDate is { } endDate
                    ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc).ToLocalTime()
                    : null,
                ev.Location,
                ev.MaxParticipants,
                ev.ImagePath));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            UpdateEventRequest request,
            int startDateOffsetMinutes,
            int? endDateOffsetMinutes,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);

            request = request with
            {
                StartDate = ConvertLocalDateToUtc(request.StartDate, startDateOffsetMinutes),
                EndDate = request.EndDate is { } endDate
                    ? ConvertLocalDateToUtc(endDate, endDateOffsetMinutes ?? startDateOffsetMinutes)
                    : null
            };

            await _eventService.UpdateAsync(request.EventId, request, cancellationToken);
            TempData["Message"] = "Событие сохранено.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _eventService.DeleteAsync(id, cancellationToken);
            TempData["Message"] = "Событие удалено.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
        {
            await _eventService.PublishAsync(id, cancellationToken);
            TempData["Message"] = "Событие опубликовано.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseRegistration(Guid id, CancellationToken cancellationToken)
        {
            await _eventService.CloseRegistrationAsync(id, cancellationToken);
            TempData["Message"] = "Регистрация закрыта.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            await _eventService.CancelAsync(id, cancellationToken);
            TempData["Message"] = "Событие отменено.";
            return RedirectToAction("Index");
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
}
