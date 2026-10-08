using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceEvents.Application.DTOs.DepartmentDTO;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public sealed class DepartmentsController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _departmentService.GetAllAsync(cancellationToken));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AdminDepartmentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AdminDepartmentViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _departmentService.CreateAsync(
                new DepartmentRequest(model.Name),
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(model.Name), exception.Message);
            return View(model);
        }

        TempData["Message"] = "Департамент создан.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        return department is null
            ? NotFound()
            : View(new AdminDepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        AdminDepartmentViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _departmentService.UpdateAsync(
                model.DepartmentId,
                new DepartmentRequest(model.Name),
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(model.Name), exception.Message);
            return View(model);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["Message"] = "Департамент сохранён.";
        return RedirectToAction(nameof(Index));
    }
}
