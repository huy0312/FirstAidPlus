-- Xóa dữ liệu cũ nếu có (tùy chọn, uncomment nếu muốn làm mới lại)
-- DELETE FROM game_options;
-- DELETE FROM game_situations;
-- DELETE FROM family_course_categories WHERE title = 'Sơ cứu hóc dị vật ở trẻ em';

DO $$
DECLARE
    new_category_id INT;
    sit1_id INT;
    sit2_id INT;
    sit3_id INT;
BEGIN
    -- 1. Tạo Danh mục (Category)
    INSERT INTO family_course_categories (title, description, icon_url, color_hex)
    VALUES (
        'Sơ cứu hóc dị vật ở trẻ em',
        'Tình huống khấn cấp khi trẻ bị hóc dị vật (thức ăn, đồ chơi nhỏ). Học cách phản ứng nhanh chóng và chính xác để cứu sống trẻ.',
        'https://cdn-icons-png.flaticon.com/512/3233/3233483.png',
        '#e74c3c'
    ) RETURNING id INTO new_category_id;

    -- 2. Tạo các Tình huống (Situations) và Lựa chọn (Options)
    
    -- Tình huống 1
    INSERT INTO game_situations (category_id, title, character_context, situation_description, question, image_url)
    VALUES (
        new_category_id,
        'Bé đang ăn dặm bỗng ho sặc sụa',
        'Bạn đang cho bé (10 tháng tuổi) ăn dặm bằng trái cây cắt nhỏ.',
        'Bé đột nhiên ho sặc sụa, mặt đỏ bừng lên. Bé vẫn có thể ho và khóc phát ra tiếng nhỏ.',
        'Bạn nên làm gì đầu tiên?',
        NULL
    ) RETURNING id INTO sit1_id;

    -- Các lựa chọn cho Tình huống 1
    INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url)
    VALUES 
        (sit1_id, 'Cố gắng thò ngón tay vào miệng bé để móc dị vật ra.', FALSE, 'Tuyệt đối KHÔNG dùng ngón tay móc mù (khi không nhìn thấy dị vật). Việc này có thể đẩy dị vật vào sâu hơn, gây tắc nghẽn hoàn toàn đường thở.', -10, NULL),
        (sit1_id, 'Khuyến khích bé ho và theo dõi sát sao.', TRUE, 'Nếu bé vẫn đang ho và khóc được nghĩa là đường thở chỉ tắc nghẽn một phần. Phản xạ ho là cách tốt nhất để đẩy dị vật ra ngoài.', 20, NULL),
        (sit1_id, 'Dốc ngược bé lên lập tức và vỗ lưng.', FALSE, 'Trẻ đang ho và khóc được không cần can thiệp vật lý như vỗ lưng ngay. Can thiệp lúc này có thể làm dị vật rơi sâu hơn.', -5, NULL),
        (sit1_id, 'Cho bé uống một ngụm nước lớn để trôi dị vật.', FALSE, 'Không bao giờ cho uống nước khi đang hóc, nước có thể tràn vào phổi gây sặc hoặc làm dị vật (nếu là dạng bánh/kẹo) nở ra gây tắc thêm.', -10, NULL);

    -- Tình huống 2
    INSERT INTO game_situations (category_id, title, character_context, situation_description, question, image_url)
    VALUES (
        new_category_id,
        'Trẻ ôm cổ, không ho, không khóc được',
        'Bé (2 tuổi) đang chơi đồ chơi xếp hình bằng nhựa nhỏ.',
        'Đột nhiên bé buông đồ chơi, hai tay ôm lấy cổ (dấu hiệu hóc dị vật), miệng há hốc nhưng không phát ra tiếng động nào. Môi bé bắt đầu tím tái.',
        'Tình trạng của bé nghiêm trọng. Hành động ngay lập tức của bạn là gì?',
        NULL
    ) RETURNING id INTO sit2_id;

    -- Các lựa chọn cho Tình huống 2
    INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url)
    VALUES 
        (sit2_id, 'Thực hiện thủ thuật Heimlich (vỗ lưng, ấn ngực/bụng) ngay lập tức.', TRUE, 'Khẩn cấp! Đây là dấu hiệu tắc nghẽn đường thở hoàn toàn. Phải lập tức thực hiện 5 lần vỗ lưng, luân phiên 5 lần ép ngực/bụng tùy độ tuổi.', 30, NULL),
        (sit2_id, 'Gọi cấp cứu 115 và ngồi chờ bác sĩ tới.', FALSE, 'Gọi cấp cứu là đúng, NHƯNG ngồi chờ sẽ làm trẻ tử vong do ngạt thở. Phải tiến hành sơ cứu ngay trong lúc nhờ người khác gọi 115.', -20, NULL),
        (sit2_id, 'Vuốt ngực bé từ trên xuống dưới cho xuôi.', FALSE, 'Hành động vuốt ngực không có tác dụng đẩy dị vật ra khỏi đường thở, làm lãng phí thời gian vàng để cứu trẻ.', -10, NULL);

    -- Tình huống 3
    INSERT INTO game_situations (category_id, title, character_context, situation_description, question, image_url)
    VALUES (
        new_category_id,
        'Thủ thuật vỗ lưng cho trẻ dưới 1 tuổi',
        'Bạn xác định cần thực hiện vỗ lưng cho bé sơ sinh (dưới 1 tuổi) do bé nghẹt thở hoàn toàn.',
        'Bạn bế bé lên để chuẩn bị thao tác vỗ lưng.',
        'Tư thế ĐÚNG để vỗ lưng cho trẻ dưới 1 tuổi là gì?',
        NULL
    ) RETURNING id INTO sit3_id;

    -- Các lựa chọn cho Tình huống 3
    INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url)
    VALUES 
        (sit3_id, 'Bế bé ngồi thẳng trên đùi bạn, úp ngực bé vào ngực bạn.', FALSE, 'Tư thế này không sử dụng được trọng lực để giúp dị vật rơi ra ngoài.', -5, NULL),
        (sit3_id, 'Để bé nằm ngửa trên giường cứng.', FALSE, 'Nằm ngửa là tư thế để ép ngực (sau khi vỗ lưng không thành công), không phải tư thế vỗ lưng.', -5, NULL),
        (sit3_id, 'Đặt bé nằm sấp dọc theo cánh tay bạn, đầu thấp hơn ngực, đỡ vằm và cổ bé.', TRUE, 'Đúng! Tư thế đầu thấp hơn ngực giúp trọng lực hỗ trợ đẩy dị vật ra. Phải đỡ chắc phần cằm/cổ bé nhưng không bóp vào cổ họng.', 25, NULL);

    RAISE NOTICE 'Đã chèn thành công toàn bộ dữ liệu mẫu cho Trò chơi: Sơ cứu hóc dị vật ở trẻ em';
END $$;
