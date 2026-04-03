-- 1. Dọn dẹp Schema bảng courses
ALTER TABLE public.courses DROP COLUMN IF EXISTS "Category";

-- 2. Nạp lại bảng plans
TRUNCATE TABLE public.plans RESTART IDENTITY CASCADE;
INSERT INTO public.plans (id, name, price, description, features, duration_value, duration_unit) VALUES
(1, 'Starter', 50000, 'Gói cơ bản cho cá nhân mới bắt đầu tìm hiểu về sơ cấp cứu.', 'Tất cả khóa học cơ bản,Chứng chỉ quốc tế,Dashboard quản lý,Hỗ trợ email,Báo cáo tiến độ', 1, 'Month'),
(2, 'Professional', 159000, 'Gói nâng cao cho chuyên gia và người muốn đào tạo chuyên sâu.', 'Tất cả khóa Starter,Khóa học nâng cao,Tùy chỉnh nội dung,Người quản lý tài khoản,Hỗ trợ ưu tiên', 1, 'Month');

-- 3. Đảm bảo bảng courses có cột category (viết thường)
DO $$ 
BEGIN 
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='courses' AND column_name='category') THEN
        ALTER TABLE public.courses ADD COLUMN category text;
    END IF;
END $$;
