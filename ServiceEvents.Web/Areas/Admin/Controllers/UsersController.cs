using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServiceEvents.Application.DTOs.UserDTO;
using ServiceEvents.Application.Interfaces.Services;
using ServiceEvents.Domain.Enums;
using ServiceEvents.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ServiceEvents.Web.Models;

namespace ServiceEvents.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(
            IUserService userService,
            IPasswordHasher<User> passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminCreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            AdminCreateUserViewModel request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);

            var user = new User(
                request.FullName,
                request.Department,
                request.Position,
                request.Email,
                request.Role);
            var passwordHash = _passwordHasher.HashPassword(user, request.Password);

            try
            {
                await _userService.CreateAsync(
                    new CreateUserRequest(
                        request.Email,
                        request.FullName,
                        request.Department,
                        request.Position,
                        request.Role),
                    passwordHash,
                    cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError(nameof(request.Email), exception.Message);
                return View(request);
            }

            TempData["Message"] = "Пользователь создан.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(new UpdateUserRequest(
                user.UserId,
                user.FullName,
                user.Department,
                user.Position));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(request);
            await _userService.UpdateAsync(request.UserId, request, cancellationToken);
            TempData["Message"] = "Пользователь сохранён.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid id, UserRole role, CancellationToken cancellationToken)
        {
            await _userService.ChangeRoleAsync(id, role, cancellationToken);
            TempData["Message"] = "Роль пользователя изменена.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            Guid id,
            string? returnUrl,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteAsync(id, cancellationToken);
                TempData["Message"] = "Пользователь удалён.";
            }
            catch (InvalidOperationException exception)
            {
                TempData["Error"] = exception.Message;
            }

            return Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl!)
                : RedirectToAction(nameof(Index));
        }
    }
}
