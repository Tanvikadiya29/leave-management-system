INSERT INTO roles (role_name) VALUES ('Admin');
INSERT INTO roles (role_name) VALUES ('Employee');

-- Insert admin details
INSERT INTO users (
    first_name,
    last_name,
    email,
    password_hash,
    role_id,
    department,
    designation,
    date_of_joining,
    is_active
)
VALUES (
    'System',
    'Admin',
    'admin@company.com',
    '$2b$10$KCLzTzTaaiT7ndR1JNq04OHohFGzeJt5XU58bazXrdHv4Xr.Icbuy',
    1,
    'IT',
    'System Administrator',
    CURRENT_DATE,
    TRUE
);