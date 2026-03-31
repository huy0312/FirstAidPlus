using FirstAidPlus.Data;
using FirstAidPlus.Models;
using FirstAidPlus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstAidPlus.Controllers
{
    [Authorize]
    public class MedicalProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAIService _aiService;

        public MedicalProfileController(AppDbContext context, IAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecords()
        {
            try 
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

                var records = await _context.MedicalRecords
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.YearDiagnosed)
                    .ToListAsync();
                return Json(records);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetRecords error: {ex.Message}");
                return Json(new { success = false, message = "Không thể tải hồ sơ y tế." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRecord([FromBody] MedicalRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.ConditionName)) return BadRequest("Tên bệnh không được để trống");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            record.UserId = userId;
            record.CreatedAt = DateTime.Now;

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return Json(new { success = true, record });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var record = await _context.MedicalRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            
            if (record != null)
            {
                _context.MedicalRecords.Remove(record);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var records = await _context.MedicalRecords.Where(r => r.UserId == userId).ToListAsync();

            if (!records.Any())
            {
                return Json(new { suggestion = "Bạn chưa có hồ sơ bệnh án nào. Hãy thêm thông tin để AI có thể tư vấn chính xác hơn." });
            }

            // Ensure we are using the Gemini service that has the analysis method
            // Casting interface to concrete to access specific method if not in interface yet
            // Or extending interface. For now, assuming standard GetReply usage or specific method.
            
            // To make it clean, we'll format a prompt here if we don't change the Interface
            var prompt = $"Dựa trên hồ sơ y tế sau của người dùng: {string.Join(", ", records.Select(r => r.ConditionName + " (" + r.Description + ")"))}. " +
                         "Hãy đề xuất 3 khóa học sơ cấp cứu phù hợp nhất có trong hệ thống FirstAidPlus (như CPR, Sơ cứu cơ bản, Sơ cứu trẻ em, v.v.) và giải thích ngắn gọn tại sao. Trả lời bằng tiếng Việt, định dạng HTML list.";

            var suggestion = _aiService.GetReply(prompt);
            return Json(new { suggestion });
        }
    }
}
