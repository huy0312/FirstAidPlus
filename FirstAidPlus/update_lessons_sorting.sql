-- Hãy chạy Script này trong DBeaver/pgAdmin để cập nhật database
-- Script này thêm cột created_at và order_index (nếu chưa có) vào bảng course_lessons

DO $$ 
BEGIN 
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='course_lessons' AND column_name='created_at') THEN
        ALTER TABLE course_lessons ADD COLUMN created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT NOW();
    END IF;

    -- Đảm bảo cột order_index tồn tại
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='course_lessons' AND column_name='order_index') THEN
        ALTER TABLE course_lessons ADD COLUMN order_index INT DEFAULT 0;
    END IF;
END $$;
