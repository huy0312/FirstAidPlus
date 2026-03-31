CREATE TABLE lesson_notes (
    id SERIAL PRIMARY KEY,
    lesson_id INT NOT NULL,
    user_id INT NOT NULL,
    content TEXT NOT NULL,
    video_timestamp DOUBLE PRECISION NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    CONSTRAINT fk_lesson_notes_lessons FOREIGN KEY (lesson_id) REFERENCES course_lessons(id) ON DELETE CASCADE,
    CONSTRAINT fk_lesson_notes_users FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE INDEX idx_lesson_notes_lesson_id ON lesson_notes(lesson_id);
CREATE INDEX idx_lesson_notes_user_id ON lesson_notes(user_id);
