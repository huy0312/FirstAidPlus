-- Seed sample feedbacks
INSERT INTO feedbacks (user_id, course_id, rating, comment, created_at) VALUES
(1, 1, 5, 'Khóa học rất hữu ích, giúp mình tự tin hơn khi sơ cứu vết thương.', NOW() - INTERVAL '1 day'),
(2, 1, 4, 'Nội dung chi tiết, giảng viên nhiệt tình.', NOW() - INTERVAL '2 days'),
(3, 2, 5, 'Rất hài lòng với kiến thức nhận được.', NOW() - INTERVAL '3 days'),
(4, 1, 3, 'Khá tốt nhưng cần thêm bài tập thực hành.', NOW() - INTERVAL '4 days'),
(5, 2, 5, 'Tuyệt vời, sẽ giới thiệu cho bạn bè.', NOW() - INTERVAL '5 days'),
(1, 2, 4, 'Bài giảng dễ hiểu, hình ảnh minh họa rõ ràng.', NOW() - INTERVAL '6 days'),
(2, 2, 5, 'Kiến thức thực tế, áp dụng được ngay.', NOW() - INTERVAL '7 days'),
(3, 1, 4, 'Cảm ơn FirstAid+, mình đã biết cách xử lý khi bị bỏng.', NOW() - INTERVAL '8 days'),
(4, 2, 5, 'Dịch vụ tốt, nội dung chất lượng.', NOW() - INTERVAL '9 days'),
(5, 1, 4, 'Mong có thêm nhiều khóa học chuyên sâu hơn.', NOW() - INTERVAL '10 days');
