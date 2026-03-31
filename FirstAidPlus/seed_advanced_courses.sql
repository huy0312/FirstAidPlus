-- COMPREHENSIVE SEED DATA FOR 10 ADDITIONAL COURSES (Expert User ID: 5)
-- Targeted for PostgreSQL

DO $$
DECLARE
    v_user_id INT := 5; -- Root User / Expert
    v_course_id INT;
    v_syllabus_id INT;
    v_i INT;
    v_course_titles TEXT[] := ARRAY[
        'Cấp cứu chấn thương sọ não cơ bản',
        'Xử trí cầm máu băng gạc từ vật dụng hàng ngày',
        'Sơ cấp cứu khi động vật hoang dã / rắn cắn',
        'Huấn luyện Sơ cứu tại Công trường (OSHA chuẩn)',
        'Sơ cứu Chấn thương thể thao & Chuột rút',
        'Cấp cứu Hạ đường huyết & Biến chứng Tiểu đường',
        'Xử trí Say nắng, Cảm lạnh & Thay đổi Nhiệt độ Đột ngột',
        'Sơ cấp cứu Tâm lý (PFA) - Sốc tâm lý sau thảm họa',
        'Kỹ năng Di chuyển Nạn nhân an toàn ra khỏi Đám Cháy',
        'Phòng ngừa và Cấp cứu Ngạt khí độc (CO, Metan)'
    ];
    v_course_descs TEXT[] := ARRAY[
        'Tìm hiểu cách nhận biết các dấu hiệu tổn thương não bộ, cách giữ cố định cột sống cổ và những việc tuyệt đối không được làm khi có nghi ngờ chấn thương sọ não.',
        'Kỹ năng sinh tồn: làm thế nào để cầm máu động mạch chỉ với một chiếc áo thun, dây thắt lưng hoặc cành cây khi bạn không có bộ sơ cứu y tế (First Aid Kit) bên mình.',
        'Hướng dẫn xử lý vết thương do chó mèo dại cắn, các loại rắn độc thường gặp, sứa biển hoặc côn trùng có nọc độc khi đi dã ngoại/rừng núi.',
        'Khóa học chuyên sâu dành riêng cho công nhân, kỹ sư giám sát tại các công trường xây dựng, bao gồm xử lý điện giật, ngã giàn giáo và tai nạn máy móc.',
        'Cách sơ cứu bong gân, rách cơ, trật khớp xương và xử lý nhanh tình trạng chuột rút, kiệt sức do vận động cường độ cao ở các vận động viên chuyên nghiệp lẫn nghiệp dư.',
        'Kiến thức cực kỳ quan trọng cho người nhà bệnh nhân tiểu đường: Phân biệt hôn mê do hạ đường hoặc tăng đường huyết và cách cấp cứu bằng đường mạch nha/nước ngọt.',
        'Nhận biết triệu chứng rối loạn thân nhiệt do thời tiết khắc nghiệt: Sốc nhiệt mùa hè và Hạ thân nhiệt mùa đông. Cách bù nước, điện giải và làm sơ cứu.',
        'Sơ cứu không chỉ cho thể xác. Khóa học này dạy bạn cách tiếp cận, an ủi, hỗ trợ tinh thần cho các nạn nhân vừa trải qua cú sốc tâm lý mạnh (tai nạn, hỏa hoạn, mất mát).',
        'Kỹ năng sống còn trong hỏa hoạn: Cách tạo mặt nạ lọc khói tạm thời, tư thế bò dưới khói hạn chế ngạt thở, và kỹ thuật kéo lê nạn nhân bất tỉnh ra khỏi vùng nguy hiểm.',
        'Phân tích cơ chế gây ngạt của các khí độc sinh hoạt (khí CO từ máy phát điện trong nhà kín, khí Metan hầm cầu) và giải pháp đưa bệnh nhân ra môi trường an toàn.'
    ];
    v_objectives TEXT[][] := ARRAY[
        ARRAY['Nhận diện chấn thương sọ não', 'Cố định cột sống cổ', 'Cách di chuyển an toàn'],
        ARRAY['Ga-rô tùy biến', 'Cầm máu động mạch chủ', 'Kỹ thuật băng ép áp lực'],
        ARRAY['Trang bị băng ép nọc độc răn', 'Xác định loại rắn độc', 'Xử lý chó dại / mèo dại cắn'],
        ARRAY['Ngắt nguồn điện an toàn', 'Sơ cứu điện tim giật', 'Cách gỡ người bị kẹt máy móc'],
        ARRAY['Nhận diện RICE', 'Kéo dãn cơ giảm chuột rút', 'Phân biệt trật khớp và gãy xương'],
        ARRAY['Nhận diện hạ đường huyết', 'Cách pha dung dịch đường cấp cứu', 'Kỹ thuật đặt bệnh nhân nằm nghiêng'],
        ARRAY['Chườm mát khoa học', 'Nhận diện say nắng mức độ nặng', 'Tuyệt đối không nhúng bệnh nhân vào nước đá'],
        ARRAY['Quy tắc Lắng nghe Tâm lý', 'Tránh đặt câu hỏi dồn dập', 'Tạo vùng không gian an toàn cho người sốc'],
        ARRAY['Tư thế tránh khói độc', 'Cách khiêng người bất tỉnh chuẩn', 'Chặn khói tràn vào phòng'],
        ARRAY['Nhận diện mùi khí dễ cháy nổ', 'Thông gió an toàn không dùng điện', 'Hô hấp nhân tạo cho người ngạt khí']
    ];
BEGIN

    FOR v_i IN 1..10 LOOP
        -- Insert Course (InstructorId in C# maps to instructor_id in DB, but it now links to Users table)
        INSERT INTO courses (title, description, image_url, video_url, is_popular, instructor_id)
        VALUES (
            v_course_titles[v_i],
            v_course_descs[v_i],
            'https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800',
            'https://www.youtube.com/watch?v=dQw4w9WgXcQ',
            (v_i % 4 = 0),
            v_user_id
        ) RETURNING id INTO v_course_id;

        -- Insert 3 Objectives for each course
        INSERT INTO course_objectives (course_id, content) VALUES
        (v_course_id, v_objectives[v_i][1]),
        (v_course_id, v_objectives[v_i][2]),
        (v_course_id, v_objectives[v_i][3]);

        -- Insert Syllabi & Lessons
        -- Chapter 1
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 1: Các khái niệm cơ bản cần nắm', 3) RETURNING id INTO v_syllabus_id;
        
        INSERT INTO course_lessons (syllabus_id, title, type, content, video_url, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 1: Tổng quan và Lời mở đầu', 'Document', 'Tình trạng ' || v_course_titles[v_i] || ' là một trong những ca cấp cứu thường gặp nhất.', NULL, 5, 0, NOW()),
        (v_syllabus_id, 'Bài 2: Tính khẩn cấp và Giờ vàng', 'Video', 'Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 15, 1, NOW()),
        (v_syllabus_id, 'Bài 3: Chuẩn bị thiết bị cần thiết', 'Document', 'Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.', NULL, 10, 2, NOW());

        -- Chapter 2
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 2: Xử trí bước một và Hai', 4) RETURNING id INTO v_syllabus_id;

        INSERT INTO course_lessons (syllabus_id, title, type, content, video_url, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 4: Phân tích đánh giá tình trạng bệnh nhân', 'Video', 'Sử dụng thang đo DRABC để đánh giá tổng quan.', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 20, 0, NOW()),
        (v_syllabus_id, 'Bài 5: Hướng dẫn kỹ thuật thủ công', 'Document', 'Chi tiết các bước từ 1 đến 5 để thao tác.', NULL, 12, 1, NOW()),
        (v_syllabus_id, 'Bài 6: Phân biệt sự sai lầm thường mắc', 'Document', '5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.', NULL, 10, 2, NOW()),
        (v_syllabus_id, 'Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến', 'Document', 'Giữ thái độ bình tĩnh và điều phối người xung quanh.', NULL, 8, 3, NOW());

        -- Chapter 3
        INSERT INTO course_syllabus (course_id, title, lesson_count)
        VALUES (v_course_id, 'Chương 3: Ôn tập và Thi chứng chỉ', 2) RETURNING id INTO v_syllabus_id;

        INSERT INTO course_lessons (syllabus_id, title, type, content, video_url, duration, order_index, created_at) VALUES
        (v_syllabus_id, 'Bài 8: Ôn tập toàn diện khóa học', 'Document', 'Tổng hợp checklist để xử lý ' || v_course_titles[v_i], NULL, 10, 0, NOW()),
        (v_syllabus_id, 'Bài 9: Bài Thi Cấp Chứng Chỉ', 'Exam', 'Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.', NULL, 30, 1, NOW());

    END LOOP;
    
    RAISE NOTICE 'Successfully seeded 10 additional courses for User (Expert) ID %', v_user_id;
END $$;
