-- Create Feedbacks table
CREATE TABLE IF NOT EXISTS feedbacks (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    course_id INT NOT NULL,
    rating INT NOT NULL,
    comment TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_feedbacks_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_feedbacks_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE
);

-- Note: Adjust constraints based on your specific requirements (e.g., ON DELETE SET NULL instead of CASCADE)
