-- Create game_category_experts table
CREATE TABLE IF NOT EXISTS game_category_experts (
    id SERIAL PRIMARY KEY,
    expert_id INTEGER NOT NULL,
    category_id INTEGER NOT NULL,
    assigned_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_gce_expert FOREIGN KEY (expert_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_gce_category FOREIGN KEY (category_id) REFERENCES family_course_categories(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_game_category_experts_expert_id ON game_category_experts(expert_id);
CREATE INDEX IF NOT EXISTS ix_game_category_experts_category_id ON game_category_experts(category_id);
