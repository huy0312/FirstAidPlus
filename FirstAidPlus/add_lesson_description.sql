-- Migration: Add description column to course_lessons table
ALTER TABLE course_lessons ADD COLUMN description TEXT;
