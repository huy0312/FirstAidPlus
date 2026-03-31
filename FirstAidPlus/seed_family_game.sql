-- Xóa dữ liệu cũ (nếu có) để tránh lỗi trùng lặp
DELETE FROM game_options;
DELETE FROM game_situations;
DELETE FROM family_course_categories;

-- Thêm Chủ đề (Categories)
INSERT INTO family_course_categories (id, title, description, icon_url, color_hex) VALUES 
(1, 'An toàn ở nhà', 'Các tình huống nguy hiểm thường gặp trong môi trường gia đình.', 'fas fa-home', '#4caf50'),
(2, 'Vui chơi ngoài trời', 'Xử lý các sự cố khi đi dã ngoại, công viên, hồ bơi.', 'fas fa-tree', '#ff9800'),
(3, 'Cấp cứu cơ bản', 'Nhận biết và sơ cứu các chấn thương phổ biến.', 'fas fa-first-aid', '#f44336');

-- Thêm Tình huống (Situations)
INSERT INTO game_situations (id, category_id, title, character_context, situation_description, question, image_url) VALUES 
(1, 1, 'Phỏng nước sôi', 'Bé Bi (4 tuổi) đang chơi trong bếp.', 'Mẹ vừa rót 1 ly nước sôi để trên bàn. Bé Bi với tay lấy đồ chơi và vô tình làm đổ ly nước lên tay.', 'Bạn sẽ làm gì ĐẦU TIÊN để sơ cứu cho bé Bi?', 'https://cdn-icons-png.flaticon.com/512/3209/3209935.png'),
(2, 1, 'Hóc dị vật', 'Bé Na (2 tuổi) đang ngồi chơi đồ chơi xếp hình.', 'Đột nhiên bé Na ho sặc sụa, mặt tím tái và chỉ tay vào cổ họng. Bé không thể khóc thành tiếng.', 'Dấu hiệu này cho thấy bé Na bị làm sao và bạn cần làm gì?', 'https://cdn-icons-png.flaticon.com/512/2854/2854346.png'),
(3, 2, 'Ong đốt', 'Hai anh em Tèo và Tí đang chơi đá bóng ngoài công viên.', 'Tèo vô tình đá quả bóng vào một bụi cây và bị một con ong cắn vào bắp tay. Vết cắn bắt đầu sưng đỏ và đau nhức.', 'Cách xử lý nào dưới đây là ĐÚNG NHẤT?', 'https://cdn-icons-png.flaticon.com/512/2550/2550419.png'),
(4, 3, 'Chảy máu cam', 'Bé Bo (7 tuổi) đang ngồi xem TV.', 'Trời hanh khô, tự nhiên mũi bé Bo chảy máu ròng ròng xuống áo áo.', 'Bạn nên bảo bé Bo quay đầu như thế nào để cầm máu?', 'https://cdn-icons-png.flaticon.com/512/5770/5770617.png');

-- Thêm Lựa chọn (Options)
-- Lựa chọn cho Tình huống 1 (Phỏng)
INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url) VALUES 
(1, 'Bôi kem đánh răng lên vết bỏng ngay lập tức.', FALSE, 'Sai lầm! Kem đánh răng có tính kiềm, có thể làm vết bỏng sâu hơn và dễ nhiễm trùng.', 0, 'fas fa-times-circle'),
(2, 'Xả vết bỏng dưới vòi nước mát chảy nhẹ khoảng 15-20 phút.', TRUE, 'Chính xác! Nước mát giúp hạ nhiệt vùng da bị bỏng, giảm sưng đau và ngăn tổn thương sâu.', 100, 'fas fa-tint'),
(3, 'Lấy đá lạnh chườm trực tiếp lên vết bỏng.', FALSE, 'Tuyệt đối không! Đá lạnh có thể gây "bỏng lạnh" khiến vùng da bị tổn thương nặng nề hơn.', 0, 'fas fa-snowflake'),
(4, 'Băng kín vết bỏng lại bằng băng dính.', FALSE, 'Không nên băng kín ngay, điều này làm nhiệt không thể thoát ra và bong tróc da khi tháo băng.', 0, 'fas fa-band-aid');

-- Lựa chọn cho Tình huống 2 (Hóc)
INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url) VALUES 
(2, 'Cho bé uống một ngụm nước lớn để vật trôi xuống.', FALSE, 'Rất nguy hiểm! Nước có thể làm dị vật lọt sâu hơn vào đường thở gây ngạt thở hoàn toàn.', 0, 'fas fa-glass-water'),
(2, 'Lấy tay chọc thẳng vào họng bé để móc dị vật ra.', FALSE, 'Không được móc họng mù! Việc này có thể đẩy dị vật vào sâu hơn.', 0, 'fas fa-hand-paper'),
(2, 'Thực hiện vỗ lưng 5 lần và ấn ngực 5 lần (Nghiệm pháp Heimlich cho trẻ nhỏ).', TRUE, 'Chính xác! Đây là kỹ thuật chuẩn để tạo áp lực đẩy dị vật ra khỏi đường thở của trẻ.', 100, 'fas fa-hands-helping'),
(2, 'Bảo bé tự ho thật mạnh.', FALSE, 'Bé 2 tuổi đang tím tái và không khóc được (nghĩa là nghẹt đường thở hoàn toàn), bé không thể tự ho được nữa.', 0, 'fas fa-cough');

-- Lựa chọn cho Tình huống 3 (Ong đốt)
INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url) VALUES 
(3, 'Nặn bóp mạnh vết cắn để nặn máu và nọc độc ra ngoài.', FALSE, 'Việc nặn bóp sẽ làm nọc độc phát tán nhanh hơn vào máu.', 0, 'fas fa-hand-holding-water'),
(3, 'Dùng thẻ ATM (hoặc vật có cạnh cứng) gạt nhẹ để lấy ngòi ong ra, sau đó rửa xà phòng.', TRUE, 'Rất tốt! Gạt ngang thay vì nhổ sẽ tránh việc bóp thêm nọc độc vào da. Rửa xà phòng giúp sát khuẩn.', 100, 'fas fa-credit-card'),
(3, 'Bôi nước mắm hoặc kem đánh răng lên vết đốt.', FALSE, 'Đây là mẹo dân gian không có cơ sở khoa học, dễ gây nhiễm trùng vết thương.', 0, 'fas fa-vial');

-- Lựa chọn cho Tình huống 4 (Máu cam)
INSERT INTO game_options (situation_id, option_text, is_correct, explanation, points, icon_url) VALUES 
(4, 'Ngửa đầu ra phía sau thật xa để máu chảy ngược vào trong.', FALSE, 'Tuyệt đối không! Máu chảy ngược vào họng có thể tràn vào phổi hoặc dạ dày gây buồn nôn.', 0, 'fas fa-arrow-up'),
(4, 'Nhét giấy ăn vào sâu trong mũi để nút lại.', FALSE, 'Giấy ăn không sạch 100%, có thể gây nhiễm trùng. Hơn nữa, khi rút ra có thể làm cục máu đông vỡ lại.', 0, 'fas fa-toilet-paper'),
(4, 'Ngồi cúi đầu nhẹ về phía trước, dùng tay bóp chặt bích mũi khoảng 5-10 phút, thở bằng miệng.', TRUE, 'Chính xác! Cúi ra trước để máu chảy ra ngoài, bóp chặt để tạo áp lực cầm máu.', 100, 'fas fa-arrow-down');

-- Cập nhật sequence (ID tự động tăng) để sau này thêm dữ liệu không bị lỗi trùng ID
SELECT setval('family_course_categories_id_seq', (SELECT MAX(id) FROM family_course_categories));
SELECT setval('game_situations_id_seq', (SELECT MAX(id) FROM game_situations));
SELECT setval('game_options_id_seq', (SELECT MAX(id) FROM game_options));
