CREATE TABLE family_course_categories (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NULL,
    icon_url TEXT NULL,
    color_hex VARCHAR(50) NULL
);

CREATE TABLE game_situations (
    id SERIAL PRIMARY KEY,
    category_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    character_context TEXT NULL,
    situation_description TEXT NULL,
    question TEXT NOT NULL,
    image_url TEXT NULL,
    FOREIGN KEY (category_id) REFERENCES family_course_categories(id)
);

CREATE TABLE game_options (
    id SERIAL PRIMARY KEY,
    situation_id INT NOT NULL,
    option_text TEXT NOT NULL,
    is_correct BOOLEAN NOT NULL DEFAULT FALSE,
    explanation TEXT NULL,
    points INT NOT NULL DEFAULT 0,
    icon_url TEXT NULL,
    FOREIGN KEY (situation_id) REFERENCES game_situations(id)
);

CREATE TABLE user_game_progress (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    situation_id INT NOT NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    score_earned INT NOT NULL DEFAULT 0,
    completed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (situation_id) REFERENCES game_situations(id)
);
