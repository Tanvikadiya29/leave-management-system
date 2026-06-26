CREATE OR REPLACE VIEW v_leave_details AS
SELECT
    lr.id,
    u.first_name || ' ' || u.last_name AS employee_name,
    u.email,
    lr.from_date,
    lr.to_date,
    (lr.to_date - lr.from_date + 1) AS total_days,
    lr.reason,
    lr.status,
    lr.remarks,
    lr.created_at
FROM leave_requests lr
JOIN users u ON u.id = lr.employee_id
ORDER BY lr.created_at DESC;