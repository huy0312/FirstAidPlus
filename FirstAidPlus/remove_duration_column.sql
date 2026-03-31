-- Remove obsolete duration column from courses table
-- This column is now replaced by a calculated property in the Course model
ALTER TABLE courses DROP COLUMN duration;
