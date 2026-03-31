using FirstAidPlus.Models;
using FirstAidPlus.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace FirstAidPlus.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly Data.AppDbContext _context;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<Hubs.NotificationHub> _hubContext;

        public CourseController(ICourseRepository courseRepository, Data.AppDbContext context, Microsoft.AspNetCore.SignalR.IHubContext<Hubs.NotificationHub> hubContext)
        {
            _courseRepository = courseRepository;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string searchString, string category, string level, int pageNumber = 1)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                // Bypass for Experts and Admins
                if (User.IsInRole("Expert") || User.IsInRole("Admin"))
                {
                    // Full access
                }
                else
                {
                    var hasActiveSubscription = await _context.UserSubscriptions
                        .AnyAsync(s => s.UserId == userId && s.Status == "Active" && (s.EndDate == null || s.EndDate > DateTime.UtcNow));
                    
                    if (!hasActiveSubscription)
                    {
                        TempData["WarningMessage"] = "Vui lòng đăng ký gói thành viên để xem danh sách khóa học.";
                        return RedirectToAction("Index", "Subscription");
                    }
                }
            }
            else
            {
                TempData["WarningMessage"] = "Vui lòng đăng nhập và đăng ký gói thành viên để sử dụng tính năng này.";
                return RedirectToAction("Index", "Subscription");
            }

            var (courses, totalCount, totalPages) = await _courseRepository.GetCoursesAsync(searchString, category, level, pageNumber);

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentLevel = level;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;
            
            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var isEnrolled = false;
            var hasDirectEnrollment = false;
            if (User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    // Check direct enrollment first
                    hasDirectEnrollment = await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == id);

                    // Bypass for Experts and Admins
                    if (User.IsInRole("Expert") || User.IsInRole("Admin"))
                    {
                        isEnrolled = true;
                        hasDirectEnrollment = true;
                    }
                    else
                    {
                        // Check direct enrollment OR any active subscription
                        isEnrolled = hasDirectEnrollment ||
                                     await _context.UserSubscriptions
                                         .AnyAsync(us => us.UserId == userId && 
                                                         us.Status == "Active" && 
                                                         (us.EndDate == null || us.EndDate > DateTime.UtcNow));
                    }
                }
            }

            if (!isEnrolled)
            {
                TempData["WarningMessage"] = "Vui lòng đăng ký gói thành viên có chứa khóa này để xem nội dung chi tiết.";
                return RedirectToAction("Index", "Subscription");
            }

            // Get Feedbacks
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.CourseId == id)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
            var totalReviews = feedbacks.Count;

            // Get enrollment count for this course
            var enrollmentCount = await _context.Enrollments
                .Where(e => e.CourseId == id)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();
            ViewBag.EnrollmentCount = enrollmentCount;

            // Sort lessons within each syllabus item for the view
            if (course.Syllabus != null)
            {
                foreach (var s in course.Syllabus)
                {
                    if (s.Lessons != null)
                    {
                        s.Lessons = s.Lessons.OrderBy(l => l.OrderIndex).ThenBy(l => l.CreatedAt).ToList();
                    }
                }
            }

            var firstLesson = course.Syllabus?
                .OrderBy(s => s.Id)
                .SelectMany(s => s.Lessons)
                .FirstOrDefault();

            var viewModel = new FirstAidPlus.ViewModels.CourseDetailViewModel
            {
                Course = course,
                IsEnrolled = isEnrolled,
                HasDirectEnrollment = hasDirectEnrollment,
                Objectives = course.Objectives.Select(o => o.Content).ToList(),
                Syllabus = course.Syllabus.OrderBy(s => s.Id).Select(s => new FirstAidPlus.ViewModels.SyllabusItem {
                    Title = s.Title,
                    Duration = s.Duration,
                    LessonCount = s.LessonCount
                }).ToList(),
                RelatedCourses = (await _courseRepository.GetAllCoursesAsync()).Where(c => c.Id != id).Take(2).ToList(),
                Feedbacks = feedbacks,
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                FirstLessonId = firstLesson?.Id
            };

            return View(viewModel);
        }


        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Lesson(int id)
        {
            var lesson = await _context.Set<FirstAidPlus.Models.CourseLesson>()
                .Include(l => l.Syllabus)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(c => c.Syllabus)
                            .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            var courseId = lesson.Syllabus.CourseId;
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Index", "Subscription");

            // Access control
            var hasAccess = User.IsInRole("Expert") || User.IsInRole("Admin") ||
                            await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId) ||
                            await _context.UserSubscriptions.AnyAsync(us => us.UserId == userId && us.Status == "Active" && (us.EndDate == null || us.EndDate > DateTime.UtcNow));

            if (!hasAccess)
            {
                TempData["WarningMessage"] = "Vui lòng đăng ký gói thành viên để xem bài học.";
                return RedirectToAction("Index", "Subscription");
            }

            // Build ordered list of all lessons in this course for sidebar navigation
            var allLessons = lesson.Syllabus.Course.Syllabus
                .OrderBy(s => s.Id)
                .SelectMany(s => s.Lessons.OrderBy(l => l.OrderIndex).ThenBy(l => l.CreatedAt))
                .ToList();

            var currentIndex = allLessons.FindIndex(l => l.Id == id);
            ViewBag.PrevLessonId = currentIndex > 0 ? allLessons[currentIndex - 1].Id : (int?)null;
            ViewBag.NextLessonId = currentIndex < allLessons.Count - 1 ? allLessons[currentIndex + 1].Id : (int?)null;
            ViewBag.Course = lesson.Syllabus.Course;
            ViewBag.AllSyllabus = lesson.Syllabus.Course.Syllabus.OrderBy(s => s.Id).ToList();
            ViewBag.CurrentLessonId = id;

            // Fetch comments (root only) for server-side render
            var comments = await _context.LessonComments
                .Include(c => c.User)
                .Include(c => c.Replies!)
                    .ThenInclude(r => r.User)
                .Where(c => c.LessonId == id && c.ParentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            ViewBag.Comments = comments;

            // Calculate Unlocked and Completed lessons
            var allLessonIds = allLessons.Select(al => al.Id).ToList();
            var progresses = await _context.UserLessonProgresses
                .Where(p => p.UserId == userId && allLessonIds.Contains(p.LessonId))
                .ToListAsync();

            var completedLessons = new HashSet<int>(progresses.Where(p => p.IsCompleted).Select(p => p.LessonId));
            var unlockedLessons = new HashSet<int>();

            if (allLessons.Any())
            {
                // First lesson is always unlocked
                unlockedLessons.Add(allLessons.First().Id);

                for (int i = 1; i < allLessons.Count; i++)
                {
                    // A lesson is unlocked if the previous one is completed
                    if (completedLessons.Contains(allLessons[i - 1].Id))
                    {
                        unlockedLessons.Add(allLessons[i].Id);
                    }
                }
            }

            // Always unlock current lesson if they somehow landed here (or for testing)
            unlockedLessons.Add(id); 

            var currentProgress = progresses.FirstOrDefault(p => p.LessonId == id);
            ViewBag.CurrentProgressSeconds = currentProgress?.TimeSpentSeconds ?? 0;
            ViewBag.IsCurrentLessonCompleted = currentProgress?.IsCompleted ?? false;

            // Global course progress
            var completedCount = progresses.Count(p => p.IsCompleted);
            ViewBag.TotalCourseProgress = allLessons.Count > 0 ? (int)Math.Round((double)completedCount / allLessons.Count * 100) : 0;

            ViewBag.CompletedLessons = completedLessons;
            ViewBag.UnlockedLessons = unlockedLessons;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_LessonContent", lesson);
            }

            return View(lesson);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public async Task<IActionResult> GetLessonQa(int lessonId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);

            var lesson = await _context.CourseLessons
                .Include(l => l.Syllabus)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null) return NotFound();

            var instructorId = lesson.Syllabus.Course.InstructorId;

            var rootComments = await _context.LessonComments
                .Include(c => c.User)
                .Include(c => c.Reactions)
                .Include(c => c.Replies!)
                    .ThenInclude(r => r.User)
                .Include(c => c.Replies!)
                    .ThenInclude(r => r.Reactions)
                .Where(c => c.LessonId == lessonId && c.ParentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = rootComments.Select(c => new
            {
                id = c.Id,
                content = c.Content,
                createdAt = c.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                userName = c.User?.FullName ?? "Người dùng",
                isInstructor = instructorId.HasValue && c.UserId == instructorId.Value,
                reactions = c.Reactions.GroupBy(r => r.ReactionType)
                    .Select(g => new { type = g.Key, count = g.Count() }).ToList(),
                currentUserReaction = c.Reactions.FirstOrDefault(r => r.UserId == userId)?.ReactionType,
                replies = (c.Replies ?? new List<LessonComment>())
                    .OrderBy(r => r.CreatedAt)
                    .Select(r => new
                    {
                        id = r.Id,
                        parentId = c.Id, // Original parent
                        content = r.Content,
                        createdAt = r.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                        userName = r.User?.FullName ?? "Người dùng",
                        isInstructor = instructorId.HasValue && r.UserId == instructorId.Value,
                        reactions = r.Reactions.GroupBy(re => re.ReactionType)
                            .Select(g => new { type = g.Key, count = g.Count() }).ToList(),
                        currentUserReaction = r.Reactions.FirstOrDefault(re => re.UserId == userId)?.ReactionType
                    })
                    .ToList()
            });

            return Json(result);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionRequest req)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var comment = await _context.LessonComments.FindAsync(req.CommentId);
            if (comment == null) return NotFound();

            var existingReaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == req.CommentId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == req.ReactionType)
                {
                    _context.CommentReactions.Remove(existingReaction);
                }
                else
                {
                    existingReaction.ReactionType = req.ReactionType;
                    _context.CommentReactions.Update(existingReaction);
                }
            }
            else
            {
                var reaction = new CommentReaction
                {
                    CommentId = req.CommentId,
                    UserId = userId,
                    ReactionType = req.ReactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CommentReactions.Add(reaction);
            }

            await _context.SaveChangesAsync();

            // Broadcast update to the lesson group
            var reactions = await _context.CommentReactions
                .Where(r => r.CommentId == req.CommentId)
                .GroupBy(r => r.ReactionType)
                .Select(g => new { type = g.Key, count = g.Count() })
                .ToListAsync();

            await _hubContext.Clients.Group($"Lesson_{comment.LessonId}")
                .SendAsync("UpdateReaction", new { commentId = req.CommentId, reactions });

            return Ok(new { success = true, reactions });
        }

        public class ReactionRequest
        {
            public int CommentId { get; set; }
            public string ReactionType { get; set; } = "👍";
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressRequest req)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var lesson = await _context.CourseLessons
                .Include(l => l.Syllabus)
                .FirstOrDefaultAsync(l => l.Id == req.LessonId);
            if (lesson == null) return NotFound();

            var progress = await _context.UserLessonProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == req.LessonId);

            if (progress == null)
            {
                progress = new UserLessonProgress
                {
                    UserId = userId,
                    LessonId = req.LessonId,
                    TimeSpentSeconds = req.TimeSpent,
                    IsCompleted = req.TimeSpent >= lesson.Duration * 60 * 0.8, // 80% threshold
                    LastAccessed = DateTime.UtcNow
                };
                _context.UserLessonProgresses.Add(progress);
            }
            else
            {
                // Ensure progress only increases (High-Water Mark)
                if (req.TimeSpent > progress.TimeSpentSeconds)
                {
                    progress.TimeSpentSeconds = req.TimeSpent;
                }
                
                if (!progress.IsCompleted && progress.TimeSpentSeconds >= lesson.Duration * 60 * 0.8) // 80% threshold
                {
                    progress.IsCompleted = true;
                }
                progress.LastAccessed = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Calculate total course progress
            var courseLessons = await _context.CourseLessons
                .Where(l => l.Syllabus.CourseId == lesson.Syllabus.CourseId)
                .Select(l => l.Id)
                .ToListAsync();
            
            var completedCount = await _context.UserLessonProgresses
                .Where(p => p.UserId == userId && courseLessons.Contains(p.LessonId) && p.IsCompleted)
                .CountAsync();
            
            int totalProgress = courseLessons.Count > 0 ? (int)Math.Round((double)completedCount / courseLessons.Count * 100) : 0;

            return Ok(new { 
                success = true, 
                isCompleted = progress.IsCompleted, 
                timeSpent = progress.TimeSpentSeconds,
                totalCourseProgress = totalProgress 
            });
        }

        public class ProgressRequest
        {
            public int LessonId { get; set; }
            public int TimeSpent { get; set; }
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteCourse(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == id && e.UserId == userId);
            if (enrollment == null)
                return NotFound("Enrollment not found");

            // Verify all lessons are completed
            var totalLessons = await _context.CourseLessons
                .Where(l => l.Syllabus.CourseId == id)
                .CountAsync();

            var completedLessons = await _context.UserLessonProgresses
                .Where(p => p.UserId == userId && p.Lesson.Syllabus.CourseId == id && p.IsCompleted)
                .CountAsync();

            if (completedLessons < totalLessons)
            {
                TempData["ErrorMessage"] = "Bạn chưa hoàn thành tất cả các bài học. Vui lòng học hết các bài trước khi nhận chứng chỉ.";
                return RedirectToAction("Details", new { id = id });
            }

            enrollment.Status = "Completed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Chúc mừng bạn đã hoàn thành khóa học!";
            return RedirectToAction("Details", new { id = id });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int lessonId, string content)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var lesson = await _context.CourseLessons
                .Include(l => l.Syllabus)
                .ThenInclude(s => s.Course)
                    .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null) return NotFound();

            var currentUser = await _context.Users.FindAsync(userId);

            var comment = new LessonComment
            {
                LessonId = lessonId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.LessonComments.Add(comment);

            // Create Notification for Instructor
            var instructorUserId = lesson.Syllabus.Course.InstructorId ?? 0;
            
            var notification = new Notification
            {
                UserId = instructorUserId,
                Title = "Câu hỏi mới",
                Message = $"{currentUser?.FullName} đã đặt câu hỏi trong bài: {lesson.Title}",
                Link = Url.Action("Lesson", "Course", new { id = lessonId }) + "#comments",
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            // Broadcast new comment to the lesson group
            await _hubContext.Clients.Group($"Lesson_{lessonId}")
                .SendAsync("ReceiveComment", new
                {
                    id = comment.Id,
                    userName = currentUser?.FullName,
                    content = content,
                    createdAt = comment.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                    isInstructor = instructorUserId == userId,
                    reactions = new List<object>(),
                    currentUserReaction = (string?)null,
                    replies = new List<object>()
                });

            // Trigger SignalR notification for expert
            await _hubContext.Clients.User(instructorUserId.ToString())
                .SendAsync("ReceiveNotification", new { 
                    title = notification.Title, 
                    message = notification.Message, 
                    link = notification.Link,
                    createdAt = notification.CreatedAt
                });

            return Json(new
            {
                success = true,
                commentId = comment.Id,
                userName = currentUser?.FullName,
                content = content,
                createdAt = comment.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm")
            });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int lessonId, int parentId, string content)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest(new { success = false, message = "Nội dung trả lời không được để trống." });
            }

            var lesson = await _context.CourseLessons
                .Include(l => l.Syllabus)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null) return NotFound();

            var course = lesson.Syllabus.Course;
            bool isInstructor = course.InstructorId == userId;

            // Find the target comment (could be a root or a reply)
            var targetComment = await _context.LessonComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == parentId && c.LessonId == lessonId);

            if (targetComment == null) return NotFound();

            // If target is a reply, we set ParentId to the top-level parent to keep a 2-level UI
            // unless we want to support deeper visual nesting.
            int rootParentId = targetComment.ParentId ?? targetComment.Id;

            // If we are replying to a reply, maybe add "@Username " to content if not already there
            if (targetComment.ParentId != null)
            {
                string mention = $"@{targetComment.User?.FullName} ";
                if (!content.StartsWith(mention))
                {
                    content = mention + content;
                }
            }

            var currentUser = await _context.Users.FindAsync(userId);

            var reply = new LessonComment
            {
                LessonId = lessonId,
                ParentId = rootParentId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.LessonComments.Add(reply);

            // Notify the learner who asked the question (root parent)
            var rootComment = await _context.LessonComments.FindAsync(rootParentId);
            if (rootComment != null)
            {
                var notification = new Notification
                {
                    UserId = rootComment.UserId,
                    Title = "Phản hồi mới",
                    Message = $"{currentUser?.FullName ?? "Chuyên gia"} đã phản hồi trong bài: {lesson.Title}",
                    Link = Url.Action("Lesson", "Course", new { id = lessonId }) + "#qa",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                
                await _hubContext.Clients.User(rootComment.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        title = notification.Title,
                        message = notification.Message,
                        link = notification.Link,
                        createdAt = notification.CreatedAt
                    });
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"Lesson_{lessonId}")
                .SendAsync("ReceiveReply", new
                {
                    id = reply.Id,
                    parentId = rootParentId,
                    userName = currentUser?.FullName ?? "Người dùng",
                    content = content,
                    createdAt = reply.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                    isInstructor = isInstructor,
                    reactions = new List<object>(),
                    currentUserReaction = (string?)null
                });

            return Json(new
            {
                success = true,
                replyId = reply.Id,
                parentId = rootParentId,
                userName = currentUser?.FullName ?? "Người dùng",
                content = content,
                createdAt = reply.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                isInstructor = isInstructor
            });
        }

        // --- Lesson Notes Actions ---

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotes(int lessonId)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var notes = await _context.LessonNotes
                .Where(n => n.LessonId == lessonId && n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Json(notes.Select(n => new {
                id = n.Id,
                content = n.Content,
                videoTimestamp = n.VideoTimestamp,
                createdAt = n.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm")
            }));
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveNote(int lessonId, string content, double? timestamp)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            if (string.IsNullOrWhiteSpace(content)) return BadRequest("Content is empty");

            var note = new LessonNote
            {
                LessonId = lessonId,
                UserId = userId,
                Content = content,
                VideoTimestamp = timestamp,
                CreatedAt = DateTime.UtcNow
            };

            _context.LessonNotes.Add(note);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                id = note.Id, 
                createdAt = note.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm") 
            });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var note = await _context.LessonNotes.FindAsync(id);
            if (note == null) return NotFound();
            if (note.UserId != userId) return Forbid();

            _context.LessonNotes.Remove(note);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int courseId, int rating, string comment)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            // Check if user has already submitted feedback
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.CourseId == courseId && f.UserId == userId);

            if (existingFeedback != null)
            {
                existingFeedback.Rating = rating;
                existingFeedback.Comment = comment;
                existingFeedback.CreatedAt = DateTime.UtcNow;
                _context.Feedbacks.Update(existingFeedback);
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được cập nhật!";
            }
            else
            {
                var feedback = new Feedback
                {
                    UserId = userId,
                    CourseId = courseId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Feedbacks.Add(feedback);
                TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = courseId });
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Expert,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLessonContent(int lessonId, string content)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var lesson = await _context.CourseLessons
                .Include(l => l.Syllabus)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null) return NotFound();

            // Permission check: Must be the instructor or an admin
            if (lesson.Syllabus.Course.InstructorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            lesson.Content = content;
            _context.CourseLessons.Update(lesson);
            await _context.SaveChangesAsync();

            return Json(new { success = true, content = lesson.Content });
        }
    }
}
