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
    public class ExpertsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FirstAidPlus.Services.IEmailService _emailService;

        public ExpertsController(AppDbContext context, FirstAidPlus.Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Admin/Experts - List all experts
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 5;
            var query = _context.Users
                .Where(u => u.RoleId == 2) // Assuming 2 is Expert role
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || u.Email.Contains(search) || (u.FullName != null && u.FullName.Contains(search)));
            }

            var totalExperts = await query.CountAsync();
            var experts = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ExpertListViewModel
            {
                Experts = experts,
                SearchString = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalExperts / (double)pageSize)
            };

            return View(viewModel);
        }
        // GET: Admin/Experts/Qualifications - List all qualifications for review
        public async Task<IActionResult> Qualifications()
        {
            var qualifications = await _context.Qualifications
                .Include(q => q.User)
                .OrderByDescending(q => q.Status == Qualification.StatusPending)
                .ThenByDescending(q => q.IssuedAt)
                .ToListAsync();

            return View(qualifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveQualification(int id)
        {
            var qual = await _context.Qualifications.Include(q => q.User).FirstOrDefaultAsync(q => q.Id == id);
            if (qual == null) return NotFound();

            qual.Status = Qualification.StatusApproved;
            
            var notification = new Notification
            {
                UserId = qual.UserId,
                Title = "Chứng chỉ đã được duyệt",
                Message = $"Chứng chỉ \"{qual.Title}\" của bạn đã được quản trị viên phê duyệt.",
                Link = "/Expert/Qualifications",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã duyệt chứng chỉ thành công.";
            return RedirectToAction(nameof(Qualifications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectQualification(int id, string reason)
        {
            var qual = await _context.Qualifications.Include(q => q.User).FirstOrDefaultAsync(q => q.Id == id);
            if (qual == null) return NotFound();

            qual.Status = Qualification.StatusRejected;
            qual.AdminComment = reason;

            var notification = new Notification
            {
                UserId = qual.UserId,
                Title = "Chứng chỉ bị từ chối",
                Message = $"Chứng chỉ \"{qual.Title}\" của bạn không được duyệt. Lý do: {reason}",
                Link = "/Expert/Qualifications",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã từ chối chứng chỉ.";
            return RedirectToAction(nameof(Qualifications));
        }

        // GET: Admin/Experts/AssignGame/5
        public async Task<IActionResult> AssignGame(int id)
        {
            var expert = await _context.Users.FindAsync(id);
            if (expert == null || expert.RoleId != 2) return NotFound();

            var allCategories = await _context.FamilyCourseCategories.ToListAsync();
            var assignedIds = await _context.GameCategoryExperts
                .Where(x => x.ExpertId == id)
                .Select(x => x.CategoryId)
                .ToListAsync();

            ViewBag.Expert = expert;
            ViewBag.AssignedIds = assignedIds;
            return View(allCategories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignGame(int id, List<int> categoryIds)
        {
            var expert = await _context.Users.FindAsync(id);
            if (expert == null || expert.RoleId != 2) return NotFound();

            // Clear existing assignments for this expert
            var existing = _context.GameCategoryExperts.Where(x => x.ExpertId == id);
            _context.GameCategoryExperts.RemoveRange(existing);

            // Add newly selected ones
            if (categoryIds != null)
            {
                foreach (var catId in categoryIds)
                {
                    _context.GameCategoryExperts.Add(new GameCategoryExpert
                    {
                        ExpertId = id,
                        CategoryId = catId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật phân công trò chơi thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
