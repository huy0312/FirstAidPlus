-- Create PlanCourses table
CREATE TABLE IF NOT EXISTS plan_courses (
    plan_id INT NOT NULL,
    course_id INT NOT NULL,
    
    PRIMARY KEY (plan_id, course_id),
    CONSTRAINT fk_plancourses_plan FOREIGN KEY (plan_id) REFERENCES plans(id) ON DELETE CASCADE,
    CONSTRAINT fk_plancourses_course FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE
);

-- Seed Data (Adjust IDs as needed based on your actual data)
-- Assuming:
-- Plan 1: Basic (Access to Course 1, 2)
-- Plan 2: Pro (Access to Course 1, 2, 3, 4)

-- Clear existing mappings if needed
-- DELETE FROM plan_courses;

-- Insert mappings
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (1, 1) ON CONFLICT DO NOTHING;
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (1, 2) ON CONFLICT DO NOTHING;
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (2, 1) ON CONFLICT DO NOTHING;
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (2, 2) ON CONFLICT DO NOTHING;
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (2, 3) ON CONFLICT DO NOTHING;
-- INSERT INTO plan_courses (plan_id, course_id) VALUES (2, 4) ON CONFLICT DO NOTHING;
