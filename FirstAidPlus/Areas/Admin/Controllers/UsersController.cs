using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstAidPlus.Areas.Admin.ViewModels;

namespace FirstAidPlus.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? roleId, int page = 1)
        {
            var viewModel = await GetUserListViewModel(search, roleId, page);
            return View(viewModel);
        }

        private async Task<UserListViewModel> GetUserListViewModel(string? search, int? roleId, int page)
        {
            int pageSize = 5;
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || u.Email.Contains(search) || (u.FullName != null && u.FullName.Contains(search)));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            var totalUsers = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var roles = await _context.Roles.ToListAsync();

            return new UserListViewModel
            {
                Users = users,
                SearchString = search,
                RoleFilter = roleId,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                Roles = roles,
                NewUser = new CreateUserViewModel()
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("NewUser.Username", "Tên đăng nhập đã tồn tại.");
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("NewUser.Email", "Email đã tồn tại.");
                }

                if (ModelState.ErrorCount == 0)
                {
                    var user = new User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Address = model.Address,
                        RoleId = model.RoleId,
                        CreatedAt = DateTime.UtcNow,
                        IsEmailConfirmed = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tạo người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // If we're here, there were errors. Prepare the Index view model.
            var viewModel = await GetUserListViewModel(null, null, 1);
            viewModel.NewUser = model; 
            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(int id, int roleId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.RoleId = roleId;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thay đổi vai trò thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
