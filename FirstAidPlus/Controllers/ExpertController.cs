using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using FirstAidPlus.Hubs;

namespace FirstAidPlus.Controllers
{
    // [Authorize(Roles = "Expert,Admin")] // Updated Role
    public class ExpertController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly FirstAidPlus.Services.ICloudinaryService _cloudinaryService;

        public ExpertController(AppDbContext context, IHubContext<ChatHub> hubContext, FirstAidPlus.Services.ICloudinaryService cloudinaryService)
        {
            _context = context;
            _hubContext = hubContext;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                 return RedirectToAction("Login", "Account");
            }

            var last7Days = Enumerable.Range(0, 7).Select(i => DateTime.UtcNow.Date.AddDays(-i)).Reverse().ToList();

            var courseCount = await _context.Courses.CountAsync(c => c.InstructorId == userId);
            
            var studentCount = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Course.InstructorId == userId)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            // Calculate Average Rating
            var expertCourseIds = await _context.Courses.Where(c => c.InstructorId == userId).Select(c => c.Id).ToListAsync();
            var feedbacks = await _context.Feedbacks.Where(f => expertCourseIds.Contains(f.CourseId)).ToListAsync();
            var avgRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;

            // Calculate New Students This Month
            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var studentsThisMonth = await _context.Enrollments
                .Where(e => expertCourseIds.Contains(e.CourseId) && e.EnrolledAt >= firstDayOfMonth)
                .CountAsync();

            ViewBag.CourseCount = courseCount;
            ViewBag.StudentCount = studentCount;
            ViewBag.AverageRating = avgRating.ToString("F1");
            ViewBag.StudentsThisMonth = studentsThisMonth;

            ViewBag.ChartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
            
            var enrollmentStats = await _context.Enrollments
                .Where(e => expertCourseIds.Contains(e.CourseId) && e.EnrolledAt >= last7Days.First())
                .GroupBy(e => e.EnrolledAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);

            ViewBag.ChartData = last7Days.Select(d => enrollmentStats.ContainsKey(d) ? enrollmentStats[d] : 0).ToList();

            // Students Today
            var studentsToday = await _context.Enrollments
                .Where(e => expertCourseIds.Contains(e.CourseId) && e.EnrolledAt >= DateTime.UtcNow.Date)
                .CountAsync();
            ViewBag.StudentsToday = studentsToday;

            // Completion Rate
            var totalEnrollments = await _context.Enrollments.CountAsync(e => expertCourseIds.Contains(e.CourseId));
            var totalLessonsInExpertCourses = await _context.CourseLessons.CountAsync(l => expertCourseIds.Contains(l.Syllabus.CourseId));
            var completedLessonsCount = await _context.UserLessonProgresses
                .CountAsync(p => p.IsCompleted && expertCourseIds.Contains(p.Lesson.Syllabus.CourseId));
            
            int completionRate = 0;
            if (totalEnrollments > 0 && totalLessonsInExpertCourses > 0)
            {
                // Simple avg completion rate: total completed / (total students * total lessons)
                completionRate = (int)((double)completedLessonsCount / (totalEnrollments * totalLessonsInExpertCourses) * 100);
            }
            ViewBag.CompletionRate = completionRate;

            // Recent Reviews
            ViewBag.RecentReviews = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Course)
                .Where(f => expertCourseIds.Contains(f.CourseId))
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Popular Courses
            var popularCourseIds = await _context.Enrollments
                .Where(e => expertCourseIds.Contains(e.CourseId))
                .GroupBy(e => e.CourseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(5)
                .ToListAsync();

            ViewBag.PopularCourses = await _context.Courses
                .Include(c => c.Syllabus)
                .Where(c => popularCourseIds.Contains(c.Id))
                .ToListAsync();

            var studentCountsDictDashboard = await _context.Enrollments
                .Where(e => popularCourseIds.Contains(e.CourseId))
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Select(e => e.UserId).Distinct().Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);
            
            ViewBag.StudentCounts = studentCountsDictDashboard;

            return View();
        }

        public async Task<IActionResult> Enrollments(int pageNumber = 1)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var expertCourseIds = await _context.Courses.Where(c => c.InstructorId == userId).Select(c => c.Id).ToListAsync();

            var query = _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => expertCourseIds.Contains(e.CourseId))
                .OrderByDescending(e => e.EnrolledAt);

            int pageSize = 10;
            var enrollments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

            return View(enrollments);
        }

        public async Task<IActionResult> MyCourses(string searchString, int pageNumber = 1)
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId))
            {
                 return RedirectToAction("Login", "Account");
            }

            var query = _context.Courses
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .Where(c => c.InstructorId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Title.Contains(searchString));
            }

            // Default to newest first (assuming lower ID or just OrderByDescending)
            query = query.OrderByDescending(c => c.Id);

            int pageSize = 6;
            var courses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseIds = courses.Select(c => c.Id).ToList();

            // Get enrollment counts for each course
            var studentCountsDict = await _context.Enrollments
                .Where(e => courseIds.Contains(e.CourseId))
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Select(e => e.UserId).Distinct().Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);

            var viewModels = courses.Select(c => new 
            {
                Course = c,
                StudentCount = studentCountsDict.ContainsKey(c.Id) ? studentCountsDict[c.Id] : 0
            }).ToList();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentPage = pageNumber;
            double totalItems = await query.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / pageSize);

            // Pass the anonymous object list to the view using ViewBag, as we don't have a specific ViewModel 
            // Or we can just use dynamic view model if it's simpler. Let's pass the list of Course, and put dictionary in ViewBag
            ViewBag.StudentCounts = studentCountsDict;

            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

             var course = await _context.Courses.FindAsync(id);
             if (course == null) return NotFound();
             
             // Expert can only delete their own course
             if (course.InstructorId != userId) return Forbid();

             _context.Courses.Remove(course);
             await _context.SaveChangesAsync();

             TempData["SuccessMessage"] = "Đã xóa khóa học thành công.";
             return RedirectToAction(nameof(MyCourses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCourseStatus(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            if (course.InstructorId != userId) return Forbid();

            course.IsActive = !course.IsActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(course.IsActive ? "bật" : "tắt")} khóa học thành công.";
            return RedirectToAction(nameof(MyCourses));
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) 
                {
                     ModelState.AddModelError("", "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");
                     return View(course);
                }

                course.InstructorId = userId;

                if (imageFile != null && imageFile.Length > 0)
                {
                    try 
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "courses");
                        course.ImageUrl = imageUrl;
                    }
                    catch (Exception ex)
                    {
                         ModelState.AddModelError("", "Lỗi khi upload ảnh: " + ex.Message);
                         return View(course);
                    }
                }

                try
                {
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                
                    // Redirect to Edit to add details immediately
                    TempData["SuccessMessage"] = "Khóa học đã được tạo thành công! Hãy thêm nội dung chi tiết.";
                    return RedirectToAction("EditCourse", new { id = course.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu vào database: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                         ModelState.AddModelError("", "Chi tiết lỗi DB: " + ex.InnerException.Message);
                    }
                }
            }
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var course = await _context.Courses
                .Include(c => c.Objectives)
                .Include(c => c.Syllabus)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();
            if (course.InstructorId != userId) return Forbid();

            // Sort lessons within each syllabus item
            if (course.Syllabus != null)
            {
                foreach (var syllabus in course.Syllabus)
                {
                    if (syllabus.Lessons != null)
                    {
                        syllabus.Lessons = syllabus.Lessons.OrderBy(l => l.OrderIndex).ThenBy(l => l.CreatedAt).ToList();
                    }
                }
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, Course course, IFormFile? imageFile)
        {
             if (id != course.Id) return NotFound();

             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");
            
             // Verify ownership again
             var existingCourse = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
             if (existingCourse == null) return NotFound();
             if (existingCourse.InstructorId != userId) return Forbid();

             if (ModelState.IsValid)
             {
                 try
                 {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "courses");
                        course.ImageUrl = imageUrl;
                    }
                    else
                    {
                        course.ImageUrl = existingCourse.ImageUrl; // Keep existing if not changed
                    }

                     course.InstructorId = userId; // Ensure ID remains
                     _context.Update(course);
                     await _context.SaveChangesAsync();

                     // Recalculate duration to be safe
                     // await UpdateCourseTotalDuration(id); // Removed: dynamic calculation
                     TempData["SuccessMessage"] = "Cập nhật thông tin khóa học thành công!";
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                    if (!_context.Courses.Any(e => e.Id == course.Id)) return NotFound();
                    else throw;
                 }
                 return RedirectToAction("EditCourse", new { id = course.Id });
             }
             return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> AddObjective(int courseId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return RedirectToAction("EditCourse", new { id = courseId });

            var course = await _context.Courses.FindAsync(courseId);
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (course.InstructorId != userId) return Forbid();

            var obj = new CourseObjective { CourseId = courseId, Content = content };
            _context.CourseObjectives.Add(obj);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã thêm mục tiêu." });

            return RedirectToAction("EditCourse", new { id = courseId });
        }

         [HttpPost]
        public async Task<IActionResult> DeleteObjective(int id)
        {
            var obj = await _context.CourseObjectives.Include(o => o.Course).FirstOrDefaultAsync(o => o.Id == id);
            if (obj == null) return NotFound();

             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (obj.Course.InstructorId != userId) return Forbid();

            _context.CourseObjectives.Remove(obj);
            int courseId = obj.CourseId;
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã xóa mục tiêu." });

            return RedirectToAction("EditCourse", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> AddSyllabus(int courseId, string title, string duration, int lessonCount)
        {
             if (string.IsNullOrWhiteSpace(title)) return RedirectToAction("EditCourse", new { id = courseId });

            var course = await _context.Courses.FindAsync(courseId);
             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (course.InstructorId != userId) return Forbid();

            var syl = new CourseSyllabus { 
                CourseId = courseId, 
                Title = title, 
                Duration = duration, 
                LessonCount = lessonCount 
            };
            _context.CourseSyllabus.Add(syl);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã thêm chương mới." });

            return RedirectToAction("EditCourse", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSyllabus(int id)
        {
            var syl = await _context.CourseSyllabus.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == id);
             if (syl == null) return NotFound();

             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (syl.Course.InstructorId != userId) return Forbid();

            _context.CourseSyllabus.Remove(syl);
            int courseId = syl.CourseId;
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã xóa chương." });

            return RedirectToAction("EditCourse", new { id = courseId });
        }


            
        

        [HttpPost]
        public async Task<IActionResult> AddLesson(int syllabusId, string title, string type, string description, string content, string videoUrl, int duration)
        {
             if (string.IsNullOrWhiteSpace(title)) 
             {
                 var s = await _context.CourseSyllabus.FindAsync(syllabusId);
                 return RedirectToAction("EditCourse", new { id = s?.CourseId ?? 0 });
             }

             var syllabus = await _context.CourseSyllabus
                 .Include(s => s.Course)
                 .Include(s => s.Lessons)
                 .FirstOrDefaultAsync(s => s.Id == syllabusId);
             if (syllabus == null) return NotFound();

             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (syllabus.Course.InstructorId != userId) return Forbid();

            // Find max OrderIndex in this syllabus
            int maxOrder = 0;
            if (syllabus.Lessons != null && syllabus.Lessons.Any())
            {
                maxOrder = syllabus.Lessons.Max(l => l.OrderIndex);
            }

            var lesson = new CourseLesson
            {
                SyllabusId = syllabusId,
                Title = title,
                Type = type,
                Description = description,
                Content = content,
                VideoUrl = videoUrl,
                Duration = duration,
                OrderIndex = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.CourseLessons.Add(lesson);
            syllabus.LessonCount += 1;
            await _context.SaveChangesAsync();

            // await UpdateCourseTotalDuration(syllabus.CourseId); // Removed: dynamic calculation

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã thêm bài học." });

            return RedirectToAction("EditCourse", new { id = syllabus.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLesson(int id, string title, string type, string description, string content, string videoUrl, int duration)
        {
            var lesson = await _context.CourseLessons.Include(l => l.Syllabus).FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null) return NotFound();

            var syllabus = lesson.Syllabus;
            var courseId = syllabus.CourseId;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Forbid();
            
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null || course.InstructorId != userId) return Forbid();

            lesson.Title = title;
            lesson.Type = type;
            lesson.Description = description;
            lesson.Content = content;
            lesson.VideoUrl = videoUrl;
            lesson.Duration = duration;

            _context.CourseLessons.Update(lesson);
            await _context.SaveChangesAsync();

            // await UpdateCourseTotalDuration(courseId); // Removed: dynamic calculation

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã cập nhật bài học." });

            return RedirectToAction("EditCourse", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLesson(int id)
        {
             var lesson = await _context.CourseLessons.Include(l => l.Syllabus).ThenInclude(s => s.Course).FirstOrDefaultAsync(l => l.Id == id);
             if (lesson == null) return NotFound();

             var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(userIdStr, out int userId)) return Forbid();
             if (lesson.Syllabus.Course.InstructorId != userId) return Forbid();

             int courseId = lesson.Syllabus.CourseId;
             _context.CourseLessons.Remove(lesson);
             if (lesson.Syllabus.LessonCount > 0) lesson.Syllabus.LessonCount -= 1;
             await _context.SaveChangesAsync();

             // await UpdateCourseTotalDuration(courseId); // Removed: dynamic calculation

             if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Đã xóa bài học." });

             return RedirectToAction("EditCourse", new { id = courseId });
        }



        // CHAT FUNCTIONALITY - LIST CONVERSATIONS
        public async Task<IActionResult> Chat()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId)) return RedirectToAction("Login", "Account");

            // Get users who have chatted with me OR I have chatted with
            // Distinct user IDs
            var contactIds = await _context.Messages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            var contacts = await _context.Users
                .Where(u => contactIds.Contains(u.Id))
                .ToListAsync();
            
            // If contact list is empty, maybe show all users (for demo) or just empty
            if (!contacts.Any())
            {
                 // Demo: Show first 10 users to start chat
                 contacts = await _context.Users.Where(u => u.Id != currentUserId).Take(10).ToListAsync();
            }

            return View(contacts);
        }

        public async Task<IActionResult> Conversation(int userId)
        {
             var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
             if (!int.TryParse(currentUserIdStr, out int currentUserId)) return RedirectToAction("Login", "Account");

             var partner = await _context.Users.FindAsync(userId);
             if (partner == null) return NotFound();

             var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) || 
                            (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            ViewBag.Partner = partner;
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int receiverId, string content)
        {
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdStr, out int currentUserId) && !string.IsNullOrEmpty(content))
            {
                var msg = new Message
                {
                    SenderId = currentUserId,
                    ReceiverId = receiverId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Messages.Add(msg);
                await _context.SaveChangesAsync();

                // Broadcast via SignalR
                var user1 = Math.Min(currentUserId, receiverId);
                var user2 = Math.Max(currentUserId, receiverId);
                var groupName = $"Chat_{user1}_{user2}";

                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveChatMessage", new
                {
                    senderId = currentUserId,
                    content = content,
                    createdAt = msg.CreatedAt.ToLocalTime().ToString("HH:mm")
                });

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }
            return RedirectToAction("Conversation", new { userId = receiverId });
        }



        [HttpPost]
        public async Task<IActionResult> UpdateLessonOrder([FromBody] LessonReorderRequest request)
        {
            if (request == null || request.LessonIds == null || !request.LessonIds.Any())
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Json(new { success = false, message = "Chưa đăng nhập." });

            var syllabus = await _context.CourseSyllabus
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == request.SyllabusId);

            if (syllabus == null || syllabus.Course.InstructorId != userId)
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            var lessons = await _context.CourseLessons
                .Where(l => l.SyllabusId == request.SyllabusId)
                .ToListAsync();

            // Update OrderIndex based on the received sequence
            for (int i = 0; i < request.LessonIds.Count; i++)
            {
                var lesson = lessons.FirstOrDefault(l => l.Id == request.LessonIds[i]);
                if (lesson != null)
                {
                    lesson.OrderIndex = i + 1;
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> Qualifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var qualifications = await _context.Qualifications
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.IssuedAt)
                .ToListAsync();

            return View(qualifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQualification(Qualification qual, IFormFile? certFile)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            qual.UserId = userId;

            if (certFile != null && certFile.Length > 0)
            {
                var certUrl = await _cloudinaryService.UploadImageAsync(certFile, "certs");
                qual.CertificateUrl = certUrl;
            }

            _context.Qualifications.Add(qual);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm chứng chỉ mới thành công.";
            return RedirectToAction(nameof(Qualifications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var qual = await _context.Qualifications.FindAsync(id);
            if (qual == null) return NotFound();
            if (qual.UserId != userId) return Forbid();

            _context.Qualifications.Remove(qual);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa chứng chỉ thành công.";
            return RedirectToAction(nameof(Qualifications));
        }

        // Family Game Management
        [HttpGet]
        public async Task<IActionResult> ManageGame()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var assignedCategories = await _context.GameCategoryExperts
                .Include(gce => gce.Category)
                .ThenInclude(c => c.Situations)
                .Where(gce => gce.ExpertId == userId && gce.Category != null)
                .Select(gce => gce.Category!)
                .ToListAsync();

            return View(assignedCategories);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var isAssigned = await _context.GameCategoryExperts.AnyAsync(gce => gce.ExpertId == userId && gce.CategoryId == id);
            if (!isAssigned) return Forbid();

            var category = await _context.FamilyCourseCategories
                .Include(c => c.Situations)
                .ThenInclude(s => s.Options)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSituation(int categoryId, string title, string? characterContext, string? situationDescription, string question)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var isAssigned = await _context.GameCategoryExperts.AnyAsync(gce => gce.ExpertId == userId && gce.CategoryId == categoryId);
            if (!isAssigned) return Forbid();

            var sit = new GameSituation
            {
                CategoryId = categoryId,
                Title = title,
                CharacterContext = characterContext,
                SituationDescription = situationDescription,
                Question = question
            };
            _context.GameSituations.Add(sit);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm tình huống thành công";
            return RedirectToAction(nameof(EditCategory), new { id = categoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSituation(int id, int categoryId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var isAssigned = await _context.GameCategoryExperts.AnyAsync(gce => gce.ExpertId == userId && gce.CategoryId == categoryId);
            if (!isAssigned) return Forbid();

            var sit = await _context.GameSituations.FindAsync(id);
            if (sit != null && sit.CategoryId == categoryId)
            {
                _context.GameSituations.Remove(sit);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa tình huống";
            }
            return RedirectToAction(nameof(EditCategory), new { id = categoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOption(int situationId, int categoryId, string optionText, bool isCorrect, string? explanation, int points)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var isAssigned = await _context.GameCategoryExperts.AnyAsync(gce => gce.ExpertId == userId && gce.CategoryId == categoryId);
            if (!isAssigned) return Forbid();

            var opt = new GameOption
            {
                SituationId = situationId,
                OptionText = optionText,
                IsCorrect = isCorrect,
                Explanation = explanation,
                Points = points
            };
            _context.GameOptions.Add(opt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditCategory), new { id = categoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOption(int id, int categoryId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var isAssigned = await _context.GameCategoryExperts.AnyAsync(gce => gce.ExpertId == userId && gce.CategoryId == categoryId);
            if (!isAssigned) return Forbid();

            var opt = await _context.GameOptions.FindAsync(id);
            if (opt != null)
            {
                _context.GameOptions.Remove(opt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(EditCategory), new { id = categoryId });
        }
    }

    public class LessonReorderRequest
    {
        public int SyllabusId { get; set; }
        public List<int> LessonIds { get; set; }
    }
}
