using BeanScene.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BeanScene.Data;

namespace BeanScene.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;


    public UsersController(
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting yourself
            if (user.Id == _userManager.GetUserId(User))
            {
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            var existingUser =
                await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            string? photoPath = null;

            if (model.Photo != null)
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "users");

                Directory.CreateDirectory(uploadsFolder);

                var fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.Photo.FileName);

                var filePath =
                    Path.Combine(uploadsFolder, fileName);

                using (var stream =
                    new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }

                photoPath =
                    "/uploads/users/" + fileName;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                PhotoPath = photoPath
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(
                    user,
                    model.Role);

                TempData["SuccessMessage"] =
                    "User created successfully.";

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var currentRole =
                    roles.FirstOrDefault() ?? "Member";

                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    CurrentRole = currentRole,
                    IsEmployee =
                        currentRole == "Manager" ||
                        currentRole == "Staff",
                        PhotoPath = user.PhotoPath
                });
            }

            return View(model);
        }


// details function
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Role = roles.FirstOrDefault() ?? "Member";

            return View(user);
        }

        // Edit Action


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // Edit POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    string id,
    ApplicationUser model,
    IFormFile? photo)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            if (photo != null)
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "users");

                Directory.CreateDirectory(uploadsFolder);

                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(photo.FileName);

                var filePath =
                    Path.Combine(uploadsFolder, fileName);

                using var stream =
                    new FileStream(filePath, FileMode.Create);

                await photo.CopyToAsync(stream);

                user.PhotoPath =
                    "/uploads/users/" + fileName;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

    }


}
