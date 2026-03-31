using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstAidPlus.Controllers
{
    [Authorize]
    public class FamilyGameController : Controller
    {
        private readonly AppDbContext _context;

        public FamilyGameController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<bool> UserHasActiveSubscription(int userId)
        {
            return await _context.UserSubscriptions
                .AnyAsync(us => us.UserId == userId && 
                                us.Status == "Active" && 
                                (us.EndDate == null || us.EndDate > DateTime.UtcNow));
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                // Bypass for Experts and Admins
                if (User.IsInRole("Expert") || User.IsInRole("Admin"))
                {
                    // Full access
                }
                else if (!await UserHasActiveSubscription(userId))
                {
                    TempData["WarningMessage"] = "Vui lòng đăng ký gói thành viên để sử dụng tính năng Khóa Học Gia Đình.";
                    return RedirectToAction("Index", "Subscription");
                }
            }

            if (!await _context.FamilyCourseCategories.AnyAsync())
            {
                try
                {
                    string sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "seed_family_game.sql");
                    if (System.IO.File.Exists(sqlPath))
                    {
                        string sql = await System.IO.File.ReadAllTextAsync(sqlPath);
                        await _context.Database.ExecuteSqlRawAsync(sql);
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle if needed
                }
            }

            var categories = await _context.FamilyCourseCategories
                .Include(c => c.Situations)
                .ToListAsync();

            var userProgress = await _context.UserGameProgresses
                .Where(p => p.UserId == userId && p.IsCompleted)
                .Select(p => p.SituationId)
                .ToListAsync();

            ViewBag.CompletedSituations = userProgress;

            return View(categories);
        }

        public async Task<IActionResult> Play(int id) // id is situationId
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                // Bypass for Experts and Admins
                if (User.IsInRole("Expert") || User.IsInRole("Admin"))
                {
                    // Full access
                }
                else if (!await UserHasActiveSubscription(userId))
                {
                    return RedirectToAction("Index", "Subscription");
                }
            }

            var situation = await _context.GameSituations
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (situation == null)
            {
                return NotFound();
            }

            return View(situation);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitProgress([FromBody] ProgressDto rawData)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var situation = await _context.GameSituations.FindAsync(rawData.SituationId);
            if (situation == null)
            {
                return NotFound();
            }

            var progress = await _context.UserGameProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.SituationId == rawData.SituationId);

            if (progress == null)
            {
                progress = new UserGameProgress
                {
                    UserId = userId,
                    SituationId = rawData.SituationId,
                    IsCompleted = true,
                    ScoreEarned = rawData.ScoreEarned,
                    CompletedAt = DateTime.UtcNow
                };
                _context.UserGameProgresses.Add(progress);
            }
            else
            {
                // Update score if higher? Simple approach: just update completed at
                if (rawData.ScoreEarned > progress.ScoreEarned)
                {
                    progress.ScoreEarned = rawData.ScoreEarned;
                }
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                _context.UserGameProgresses.Update(progress);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }

    public class ProgressDto
    {
        public int SituationId { get; set; }
        public int ScoreEarned { get; set; }
    }
}
