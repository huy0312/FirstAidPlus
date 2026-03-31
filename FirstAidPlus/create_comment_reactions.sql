-- Create comment_reactions table
CREATE TABLE IF NOT EXISTS comment_reactions (
    id SERIAL PRIMARY KEY,
    comment_id INT NOT NULL REFERENCES lesson_comments(id) ON DELETE CASCADE,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    reaction_type VARCHAR(10) NOT NULL DEFAULT '👍',
    created_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(comment_id, user_id, reaction_type)
);

-- Ensure parent_id column exists in lesson_comments (for threading)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'lesson_comments' AND column_name = 'parent_id'
    ) THEN
        ALTER TABLE lesson_comments ADD COLUMN parent_id INT REFERENCES lesson_comments(id) ON DELETE CASCADE;
    END IF;
END $$;
