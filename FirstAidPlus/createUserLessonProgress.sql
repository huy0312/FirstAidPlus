-- ==========================================
-- SCRIPT TẠO BẢNG THEO DÕI TIẾN ĐỘ BÀI HỌC
-- Hãy chạy Script này trong DBeaver/pgAdmin
-- ==========================================
CREATE TABLE user_lesson_progress (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    lesson_id INT NOT NULL,
    time_spent_seconds INT NOT NULL DEFAULT 0,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    last_accessed TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_user_lesson_progress_users FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_lesson_progress_course_lessons FOREIGN KEY (lesson_id) REFERENCES course_lessons(id) ON DELETE CASCADE
);

CREATE INDEX ix_user_lesson_progress_user_id ON user_lesson_progress(user_id);
CREATE INDEX ix_user_lesson_progress_lesson_id ON user_lesson_progress(lesson_id);
