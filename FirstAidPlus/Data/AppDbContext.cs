using Microsoft.EntityFrameworkCore;
using FirstAidPlus.Models;

namespace FirstAidPlus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<CourseObjective> CourseObjectives { get; set; }
        public DbSet<CourseSyllabus> CourseSyllabus { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<CourseLesson> CourseLessons { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<PlanCourse> PlanCourses { get; set; }
        public DbSet<FamilyCourseCategory> FamilyCourseCategories { get; set; }
        public DbSet<GameSituation> GameSituations { get; set; }
        public DbSet<GameOption> GameOptions { get; set; }
        public DbSet<UserGameProgress> UserGameProgresses { get; set; }
        public DbSet<UserLessonProgress> UserLessonProgresses { get; set; }
        public DbSet<LessonComment> LessonComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<GameCategoryExpert> GameCategoryExperts { get; set; }


        public DbSet<LessonNote> LessonNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Manual Snake_case mapping for LessonNote
            modelBuilder.Entity<LessonNote>(entity => {
                entity.ToTable("lesson_notes");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.VideoTimestamp).HasColumnName("video_timestamp");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            // Manual Snake_case mapping for User
            modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Bio).HasColumnName("bio");
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.ResetToken).HasColumnName("reset_token");
                entity.Property(e => e.ResetTokenExpiry).HasColumnName("reset_token_expiry");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.IsEmailConfirmed).HasColumnName("is_email_confirmed");
                entity.Property(e => e.EmailConfirmationToken).HasColumnName("email_confirmation_token");
            });

            // Manual Snake_case mapping for Role
            modelBuilder.Entity<Role>(entity => {
                entity.ToTable("roles");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleName).HasColumnName("role_name");
            });

            // Manual Snake_case mapping for Course
            modelBuilder.Entity<Course>(entity => {
                entity.ToTable("courses");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CertificateName).HasColumnName("certificate_name");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url");
                entity.Property(e => e.VideoUrl).HasColumnName("video_url");
                entity.Property(e => e.TrainingImageUrl).HasColumnName("training_image_url");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.IsPopular).HasColumnName("is_popular");
                entity.Property(e => e.InstructorId).HasColumnName("instructor_id");
                entity.Property(e => e.Category).HasColumnName("category");
            });

            // Removed Instructors and Instructor_Qualifications maps

            // Manual Snake_case mapping for Testimonial
            modelBuilder.Entity<Testimonial>(entity => {
                entity.ToTable("testimonials");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StudentName).HasColumnName("student_name");
                entity.Property(e => e.StudentRole).HasColumnName("student_role");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.Rating).HasColumnName("rating");
            });

            // Manual Snake_case mapping for CourseObjective
            modelBuilder.Entity<CourseObjective>(entity => {
                entity.ToTable("course_objectives");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.Content).HasColumnName("content");
            });

            // Manual Snake_case mapping for CourseSyllabus
            modelBuilder.Entity<CourseSyllabus>(entity => {
                entity.ToTable("course_syllabus");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.LessonCount).HasColumnName("lesson_count");
            });

            // Manual Snake_case mapping for Enrollment
            modelBuilder.Entity<Enrollment>(entity => {
                entity.ToTable("enrollments");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.EnrolledAt).HasColumnName("enrolled_at");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Amount).HasColumnName("amount");
            });

            // Manual Snake_case mapping for Message
            modelBuilder.Entity<Message>(entity => {
                entity.ToTable("messages");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Manual Snake_case mapping for Subscription Plans
             modelBuilder.Entity<Plan>(entity => {
                entity.ToTable("plans");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Features).HasColumnName("features");
                entity.Property(e => e.DurationValue).HasColumnName("duration_value");
                entity.Property(e => e.DurationUnit).HasColumnName("duration_unit");
            });

             modelBuilder.Entity<UserSubscription>(entity => {
                entity.ToTable("user_subscriptions");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.Status).HasColumnName("status");
            });

             // Manual Snake_case mapping for CourseLesson
            modelBuilder.Entity<CourseLesson>(entity => {
                entity.ToTable("course_lessons");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SyllabusId).HasColumnName("syllabus_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Type).HasColumnName("type");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.VideoUrl).HasColumnName("video_url");
                entity.Property(e => e.Duration).HasColumnName("duration");
                entity.Property(e => e.OrderIndex).HasColumnName("order_index");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Manual Snake_case mapping for Settings
            modelBuilder.Entity<Setting>(entity => {
                entity.ToTable("settings");
                entity.Property(e => e.Key).HasColumnName("key");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Group).HasColumnName("group");
            });

             // Manual Snake_case mapping for Transactions
            modelBuilder.Entity<Transaction>(entity => {
                entity.ToTable("transactions");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.OrderDescription).HasColumnName("order_description");
                entity.Property(e => e.VnPayTxnRef).HasColumnName("vnp_txn_ref");
                entity.Property(e => e.VnPayTransactionNo).HasColumnName("vnp_transaction_no");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.MomoOrderId).HasColumnName("momo_order_id");
                entity.Property(e => e.MomoTransId).HasColumnName("momo_trans_id");
                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            });
            // Manual Snake_case mapping for MedicalRecord
            modelBuilder.Entity<MedicalRecord>(entity => {
                entity.ToTable("medical_records");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ConditionName).HasColumnName("condition_name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.YearDiagnosed).HasColumnName("year_diagnosed");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Manual Snake_case mapping for Feedback
            modelBuilder.Entity<Feedback>(entity => {
                entity.ToTable("feedbacks");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // PlanCourse Many-to-Many Configuration
            modelBuilder.Entity<PlanCourse>(entity => {
                entity.ToTable("plan_courses");
                entity.HasKey(pc => new { pc.PlanId, pc.CourseId });
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.HasOne(pc => pc.Plan)
                      .WithMany(p => p.PlanCourses)
                      .HasForeignKey(pc => pc.PlanId);

                entity.HasOne(pc => pc.Course)
                      .WithMany(c => c.PlanCourses)
                      .HasForeignKey(pc => pc.CourseId);
            });

            // Manual Snake_case mapping for FamilyGame Models
            modelBuilder.Entity<FamilyCourseCategory>(entity => {
                entity.ToTable("family_course_categories");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.IconUrl).HasColumnName("icon_url");
                entity.Property(e => e.ColorHex).HasColumnName("color_hex");
            });

            modelBuilder.Entity<GameSituation>(entity => {
                entity.ToTable("game_situations");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.CharacterContext).HasColumnName("character_context");
                entity.Property(e => e.SituationDescription).HasColumnName("situation_description");
                entity.Property(e => e.Question).HasColumnName("question");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url");

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Situations)
                      .HasForeignKey(e => e.CategoryId);
            });

            modelBuilder.Entity<GameOption>(entity => {
                entity.ToTable("game_options");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SituationId).HasColumnName("situation_id");
                entity.Property(e => e.OptionText).HasColumnName("option_text");
                entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
                entity.Property(e => e.Explanation).HasColumnName("explanation");
                entity.Property(e => e.Points).HasColumnName("points");
                entity.Property(e => e.IconUrl).HasColumnName("icon_url");

                entity.HasOne(e => e.Situation)
                      .WithMany(s => s.Options)
                      .HasForeignKey(e => e.SituationId);
            });

            modelBuilder.Entity<UserGameProgress>(entity => {
                entity.ToTable("user_game_progress");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.SituationId).HasColumnName("situation_id");
                entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
                entity.Property(e => e.ScoreEarned).HasColumnName("score_earned");
                entity.Property(e => e.CompletedAt).HasColumnName("completed_at");

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Situation)
                      .WithMany()
                      .HasForeignKey(e => e.SituationId);
            });

            // Manual Snake_case mapping for UserLessonProgress
            modelBuilder.Entity<UserLessonProgress>(entity => {
                entity.ToTable("user_lesson_progress");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.TimeSpentSeconds).HasColumnName("time_spent_seconds");
                entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
                entity.Property(e => e.LastAccessed).HasColumnName("last_accessed");

                entity.HasOne(e => e.Lesson)
                      .WithMany()
                      .HasForeignKey(e => e.LessonId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Manual Snake_case mapping for LessonComment
            modelBuilder.Entity<LessonComment>(entity => {
                entity.ToTable("lesson_comments");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.LessonId).HasColumnName("lesson_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.ParentId).HasColumnName("parent_id");
            });

            // Manual Snake_case mapping for Notification
            modelBuilder.Entity<Notification>(entity => {
                entity.ToTable("notifications");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.Link).HasColumnName("link");
                entity.Property(e => e.IsRead).HasColumnName("is_read");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Manual Snake_case mapping for CommentReaction
            modelBuilder.Entity<CommentReaction>(entity => {
                entity.ToTable("comment_reactions");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CommentId).HasColumnName("comment_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ReactionType).HasColumnName("reaction_type");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Manual Snake_case mapping for Qualification
            modelBuilder.Entity<Qualification>(entity => {
                entity.ToTable("qualifications");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CertificateUrl).HasColumnName("certificate_url");
                entity.Property(e => e.IssuedAt).HasColumnName("issued_at");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.AdminComment).HasColumnName("admin_comment");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            // Game Category Expert join table
            modelBuilder.Entity<GameCategoryExpert>(entity => {
                entity.ToTable("game_category_experts");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ExpertId).HasColumnName("expert_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");

                entity.HasOne(e => e.Expert)
                      .WithMany()
                      .HasForeignKey(e => e.ExpertId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
