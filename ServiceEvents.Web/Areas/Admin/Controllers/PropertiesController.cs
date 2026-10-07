using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ServiceEvents.Application.DTOs.EventPropertyDTO;
using ServiceEvents.Application.Interfaces.Services;

namespace ServiceEvents.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PropertiesController : Controller
    {
        private readonly IEventPropertyService _propertyService;

        public PropertiesController(IEventPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var props = await _propertyService.GetGlobalAsync(cancellationToken);
            return View(props);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventPropertyRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);
            await _propertyService.CreateAsync(request, cancellationToken);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var props = await _propertyService.GetGlobalAsync(cancellationToken);
            var prop = props.FirstOrDefault(p => p.PropertyId == id);
            if (prop == null) return NotFound();
            return View(prop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEventPropertyRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);
            await _propertyService.UpdateAsync(request.PropertyId, request, cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _propertyService.DeleteAsync(id, cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
