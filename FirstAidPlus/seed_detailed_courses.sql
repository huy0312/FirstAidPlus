-- COMPREHENSIVE SEED DATA FOR 10 COURSES (Instructor User ID: 5)
-- Targeted for PostgreSQL

DO $$
DECLARE
    v_instructor_id INT;
    v_course_id INT;
    v_syllabus_id INT;
    v_i INT;
    v_course_titles TEXT[] := ARRAY[
        'Sơ cứu Ngưng tim ngưng thở (CPR) Cơ bản',
        'Xử trí Dị vật đường thở (Hóc dị vật)',
        'Sơ cứu Vết thương Chảy máu & Băng bó',
        'Xử trí Gãy xương & Chấn thương Phần mềm',
        'Sơ cứu Bỏng & Ngộ độc tại nhà',
        'Kỹ năng Sơ cứu Tai nạn Giao thông',
        'Sơ cứu Đuối nước & Tai nạn Dưới nước',
        'Xử trí Sốc phản vệ & Dị ứng nặng',
        'Sơ cứu Co giật & Đột quỵ',
        'Sơ cứu cho Trẻ em (Dành cho Cha mẹ)'
    ];
    v_course_descs TEXT[] := ARRAY[
        'Khóa học cung cấp kiến thức nền tảng về hồi sức tim phổi (CPR) và cách sử dụng máy khử rung tim tự động (AED). Phù hợp cho mọi đối tượng muốn trang bị kỹ năng cứu sống tính mạng trong tình huống khẩn cấp.',
        'Hướng dẫn thực hiện kỹ năng vỗ lưng ấn ngực và nghiệm pháp Heimlich để loại bỏ dị vật trong đường thở cho cả trẻ sơ sinh, trẻ nhỏ và người lớn.',
        'Học cách phân biệt các loại vết thương, phương pháp cầm máu hiệu quả và các kỹ thuật băng bó cơ bản đến nâng cao để tránh nhiễm trùng và mất máu quá nhiều.',
        'Quy trình cố định xương gãy bằng nẹp tạm thời, cách xử lý bong gân, trật khớp và giảm đau nhanh chóng trước khi nhân viên y tế đến.',
        'Cách sơ cứu các loại bỏng (nhiệt, điện, hóa chất) và xử trí khẩn cấp khi uống nhầm hóa chất hoặc bị ngộ độc thực phẩm tại gia đình.',
        'Các bước tiếp cận hiện trường tai nạn an toàn, di chuyển nạn nhân đúng cách và sơ cứu đa chấn thương để hạn chế tối đa di chứng.',
        'Kỹ thuật tiếp cận, đưa nạn nhân lên bờ an toàn và thực hiện cấp cứu ngưng tuần hoàn hô hấp cho người bị đuối nước.',
        'Nhận biết nhanh các dấu hiệu sốc phản vệ, hạ huyết áp cấp tính và hướng dẫn sử dụng bút tiêm adrenaline để cứu sống bệnh nhân.',
        'Quy trình xử trí khi gặp người bị co giật, động kinh và cách nhận diện sớm các dấu hiệu đột quỵ (FAST) để tận dụng "giờ vàng".',
        'Tổng hợp các tình huống tai nạn thường gặp nhất ở trẻ em: hóc, bỏng, ngã, ngộ độc và sự khác biệt trong sơ cứu trẻ so với người lớn.'
    ];
    v_objectives TEXT[][] := ARRAY[
        ARRAY['Thực hiện đúng kỹ thuật ép tim ngoài lồng ngực', 'Biết cách thổi ngạt chuẩn xác', 'Sử dụng thành thạo máy AED'],
        ARRAY['Phân biệt tắc nghẽn đường thở một phần và hoàn toàn', 'Thực hiện đúng nghiệm pháp Heimlich', 'Xử trí hóc dị vật cho trẻ sơ sinh'],
        ARRAY['Biết cách dùng băng ép và ga-rô đúng lúc', 'Nhận diện vết thương hở và kín', 'Nguyên tắc vệ sinh vết thương'],
        ARRAY['Sử dụng các vật dụng tại chỗ để làm nẹp', 'Xử lý bong gân bằng phương pháp R.I.C.E', 'Hạn chế tổn thương tủy sống'],
        ARRAY['Làm mát vết bỏng đúng cách', 'Không sử dụng các mẹo dân gian sai lầm', 'Gây nôn đúng cách khi bị ngộ độc'],
        ARRAY['Giữ an toàn hiện trường cho bản thân', 'Kiểm tra tri giác nạn nhân', 'Cầm máu vùng đầu và cổ'],
        ARRAY['Nguyên tắc cứu hộ không xuống nước', 'Xả nước trong phổi nạn nhân', 'Sưởi ấm chống hạ thân nhiệt'],
        ARRAY['Xác định các tác nhân gây dị ứng phổ biến', 'Phân biệt dị ứng nhẹ và sốc phản vệ', 'Tư thế nằm an toàn cho bệnh nhân'],
        ARRAY['Bảo vệ nạn nhân khỏi va đập khi co giật', 'Ghi chú thời gian cơn co giật', 'Kiểm tra các dấu hiệu thần kinh'],
        ARRAY['Tâm lý bình tĩnh khi trẻ gặp tai nạn', 'Sơ cứu các vết trầy xước nhỏ', 'Nhận biết dấu hiệu khó thở ở trẻ']
    ];
BEGIN
    -- Set instructor_id directly to user_id 5
    v_instructor_id := 5;

    FOR v_i IN 1..10 LOOP
        -- Insert Course
        INSERT INTO courses (title, description, image_url, video_url, is_popular, instructor_id)
        VALUES (
            v_course_titles[v_i],
            v_course_descs[v_i],
            'https://images.unsplash.com/photo-1542884748-2b87b36c6b90?w=800',
            'https://www.youtube.com/watch?v=dQw4w9WgXcQ',
            (v_i % 3 = 0),
            v_instructor_id
        ) RETURNING id INTO v_course_id;

        -- Insert 3 Objectives for each course
        INSERT INTO course_objectives (course_id, content) VALUES
        (v_course_id, v_objectives[v_i][1]),
        (v_course_id, v_objectives[v_i][2]),
        (v_course_id, v_objectives[v_i][3]);

        -- Insert Syllabi & Lessons
        -- Chapter 1
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 1: Kiến thức nền tảng', 3) RETURNING id INTO v_syllabus_id;
        
        INSERT INTO course_lessons (syllabus_id, title, type, content, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 1: Giới thiệu khóa học và mục tiêu', 'Document', 'Chào mừng bạn đến với khóa học ' || v_course_titles[v_i] || '. Trong bài này chúng ta sẽ tìm hiểu...', 5, 0, NOW()),
        (v_syllabus_id, 'Bài 2: Tầm quan trọng của kỹ năng này', 'Document', 'Tại sao chúng ta cần biết về ' || v_course_titles[v_i] || '? Thống kê cho thấy...', 10, 1, NOW()),
        (v_syllabus_id, 'Bài 3: Các nguyên tắc an toàn chung', 'Document', 'Trước khi hỗ trợ người khác, bạn phải đảm bảo an toàn cho chính mình...', 8, 2, NOW());

        -- Chapter 2
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 2: Quy trình xử lý chi tiết', 4) RETURNING id INTO v_syllabus_id;

        INSERT INTO course_lessons (syllabus_id, title, type, content, video_url, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 4: Video hướng dẫn kỹ thuật chuẩn', 'Video', 'Xem kỹ video hướng dẫn thực hành dưới đây...', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 15, 0, NOW()),
        (v_syllabus_id, 'Bài 5: Phân tích các bước thực hiện', 'Document', 'Bươc 1: Kiểm tra hiện trường. Bước 2: Kiểm tra phản ứng...', 12, 1, NOW()),
        (v_syllabus_id, 'Bài 6: Các lỗi thường gặp khi thực hiện', 'Document', 'Nhiều người thường mắc lỗi sau: 1. Ép tim không đủ sâu. 2. Thổi ngạt quá mạnh...', 10, 2, NOW()),
        (v_syllabus_id, 'Bài 7: Những lưu ý đặc biệt', 'Document', 'Lưu ý khi thực hiện cho người cao tuổi hoặc người có bệnh nền...', 7, 3, NOW());

        -- Chapter 3
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 3: Thực hành và Kiểm tra', 3) RETURNING id INTO v_syllabus_id;

        INSERT INTO course_lessons (syllabus_id, title, type, content, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 8: Bài tập giả lập tình huống', 'Document', 'Giả sử bạn đang ở trong công viên và gặp một người bất tỉnh...', 15, 0, NOW()),
        (v_syllabus_id, 'Bài 9: Tổng hợp kiến thức toàn khóa', 'Document', 'Chúng ta đã đi qua các nội dung về...', 10, 1, NOW()),
        (v_syllabus_id, 'Bài 10: Bài kiểm tra cuối khóa', 'Exam', 'Vui lòng trả lời 10 câu hỏi trắc nghiệm dưới đây để hoàn thành khóa học...', 20, 2, NOW());

    END LOOP;
    
    RAISE NOTICE 'Successfully seeded 10 detailed courses for instructor_id %', v_instructor_id;
END $$;
