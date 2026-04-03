using FirstAidPlus.Data;
using FirstAidPlus.Models;
using FirstAidPlus.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FirstAidPlus.Hubs;

namespace FirstAidPlus.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICourseRepository _courseRepository;
        private readonly FirstAidPlus.Services.IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EnrollmentController(AppDbContext context, ICourseRepository courseRepository, FirstAidPlus.Services.IEmailService emailService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _courseRepository = courseRepository;
            _emailService = emailService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Checkout(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                 // Handle case where user ID is not an int or not found
                 // For now, redirect to login or show error
                 return RedirectToAction("Login", "Account");
            }

            // Check if already enrolled
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                return RedirectToAction("MyCourses"); // Or some page showing purchased courses
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                 return RedirectToAction("Login", "Account");
            }

             // Check if already enrolled to prevent duplicates
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                 return RedirectToAction("Details", "Course", new { id = courseId });
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow,
                Status = "Active",
                Amount = 0 // Course has no individual price, subscription model
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // --- NOTIFICATION LỌGIC ---
            // Trigger Notification to Instructor when a user enrolls
            var instructorUserId = course.InstructorId ?? 0;
            var userName = User.Identity.Name;
            var fullNameObj = User.FindFirstValue("FullName");
            var notificationName = !string.IsNullOrEmpty(fullNameObj) ? fullNameObj : userName;

            var notification = new Notification
            {
                UserId = instructorUserId,
                Title = "Học viên mới",
                Message = $"{notificationName} vừa đăng ký khóa học: {course.Title}",
                Link = Url.Action("Dashboard", "Expert"), // Relative link to expert dashboard
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Trigger SignalR 
            await _hubContext.Clients.User(instructorUserId.ToString())
                .SendAsync("ReceiveNotification", new { 
                    title = notification.Title, 
                    message = notification.Message, 
                    link = notification.Link,
                    createdAt = notification.CreatedAt
                });
            // -------------------------

            // Send Invoice Email (Confirmation of start)
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("Email");

            if (!string.IsNullOrEmpty(userEmail))
            {
                var subject = $"Xác nhận tham gia khóa học {course.Title} - FirstAid+";
                var message = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; max-width: 600px; margin: 0 auto;'>
                        <h2 style='color: #dc3545;'>Chúc mừng bạn đã tham gia khóa học!</h2>
                        <p>Xin chào <strong>{userName}</strong>,</p>
                        <p>Bạn đã đăng ký thành công khóa học. Dưới đây là thông tin:</p>
                        <hr style='border: 0; border-top: 1px solid #eee;' />
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='padding: 8px 0;'><strong>Khóa học:</strong></td>
                                <td style='text-align: right;'>{course.Title}</td>
                            </tr>
                             <tr>
                                <td style='padding: 8px 0;'><strong>Mã tham gia:</strong></td>
                                <td style='text-align: right;'>#{enrollment.Id}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0;'><strong>Ngày bắt đầu:</strong></td>
                                <td style='text-align: right;'>{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</td>
                            </tr>
                        </table>
                        <hr style='border: 0; border-top: 1px solid #eee;' />
                        <p>Bạn có thể bắt đầu học ngay bây giờ bằng cách truy cập vào trang <a href='https://localhost:7054/Enrollment/MyCourses' style='color: #dc3545; text-decoration: none; font-weight: bold;'>Khóa học của tôi</a>.</p>
                        <p style='font-size: 12px; color: #888;'>Nếu bạn cần hỗ trợ, vui lòng liên hệ hotline 1900 1818.</p>
                        <p>Trân trọng,<br>Đội ngũ FirstAid+</p>
                    </div>
                ";

                try 
                {
                    await _emailService.SendEmailAsync(userEmail, subject, message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            var firstLesson = await _context.CourseLessons
                .Where(l => l.Syllabus.CourseId == courseId)
                .OrderBy(l => l.OrderIndex)
                .ThenBy(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            if (firstLesson != null)
            {
                TempData["SuccessMessage"] = $"Đã tham gia khóa học: {course.Title}";
                return RedirectToAction("Lesson", "Course", new { id = firstLesson.Id });
            }

            return RedirectToAction("Success", new { courseId = courseId });
        }

        public async Task<IActionResult> Success(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
             if (course == null) return RedirectToAction("Index", "Course");
            return View(course);
        }
        
        // Quick "My Courses" action placeholder
        public async Task<IActionResult> MyCourses(string searchString, string statusFilter, int pageNumber = 1)
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var query = _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.Course.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(e => e.Status == statusFilter);
            }

            int pageSize = 6;
            var enrollments = await query
                .OrderByDescending(e => e.EnrolledAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseIds = enrollments.Select(e => e.CourseId).ToList();

            var totalLessonsDict = await _context.CourseLessons
                .Where(l => courseIds.Contains(l.Syllabus.CourseId))
                .GroupBy(l => l.Syllabus.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);

            var completedLessonsDict = await _context.UserLessonProgresses
                .Where(p => p.UserId == userId && p.IsCompleted && courseIds.Contains(p.Lesson.Syllabus.CourseId))
                .GroupBy(p => p.Lesson.Syllabus.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);

            var viewModels = enrollments.Select(e => new FirstAidPlus.Models.ViewModels.MyCourseViewModel
            {
                CourseId = e.CourseId,
                Title = e.Course.Title,
                ImageUrl = e.Course.ImageUrl,
                EnrolledAt = e.EnrolledAt,
                Status = e.Status,
                InstructorId = e.Course.InstructorId,
                TotalLessons = totalLessonsDict.ContainsKey(e.CourseId) ? totalLessonsDict[e.CourseId] : 0,
                CompletedLessons = completedLessonsDict.ContainsKey(e.CourseId) ? completedLessonsDict[e.CourseId] : 0
            }).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentFilter = statusFilter;
            ViewBag.CurrentPage = pageNumber;
            // Ceiling rounds up to the next full int block, so elements / 6
            ViewBag.TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

            return View(viewModels);
        }
    }
}
