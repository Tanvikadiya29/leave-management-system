import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import api from "../api/axiosInstance";
import toast from "react-hot-toast";

export default function LeaveRequestsPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [leaves, setLeaves] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ fromDate: "", toDate: "", reason: "" });
  const [rejectId, setRejectId] = useState(null);
  const [remarks, setRemarks] = useState("");

  useEffect(() => {
    fetchLeaves();
  }, []);

  async function fetchLeaves() {
    try {
      const res = await api.get("/leave-requests");
      setLeaves(res.data);
    } catch {
      toast.error("Could not load leave requests.");
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();

    if (form.toDate < form.fromDate) {
      toast.error("To date cannot be before from date.");
      return;
    }

    try {
      await api.post("/leave-requests", form);
      toast.success("Leave request submitted.");
      setShowForm(false);
      setForm({ fromDate: "", toDate: "", reason: "" });
      fetchLeaves();
    } catch (err) {
      toast.error(err.response?.data?.message || "Could not submit leave.");
    }
  }

  async function handleApprove(id) {
    try {
      await api.put(`/leave-requests/${id}/review`, {
        status: "Approved",
        remarks: "",
      });
      toast.success("Leave approved.");
      fetchLeaves();
    } catch {
      toast.error("Could not approve.");
    }
  }

  async function handleReject(id) {
    try {
      await api.put(`/leave-requests/${id}/review`, {
        status: "Rejected",
        remarks,
      });
      toast.success("Leave rejected.");
      setRejectId(null);
      setRemarks("");
      fetchLeaves();
    } catch {
      toast.error("Could not reject.");
    }
  }

  function getBadgeClass(status) {
    if (status === "Approved") return "badge badge-approved";
    if (status === "Rejected") return "badge badge-rejected";
    return "badge badge-pending";
  }

  return (
    <div>
      {/* Navbar */}
      <div className="navbar">
        <span className="navbar-title">Leave Management System</span>
        <div className="navbar-right">
          <span className="navbar-user">
            Hello, {user?.firstName} ({user?.role})
          </span>
          {user?.role === "Admin" && (
            <button
              className="navbar-link"
              onClick={() => navigate("/employees")}
            >
              Employees
            </button>
          )}
          <button className="btn-logout" onClick={logout}>
            Logout
          </button>
        </div>
      </div>

      <div className="page-wrapper">
        {/* Page header */}
        <div className="page-header">
          <h2 className="page-title">
            Leave Requests
            <span className="page-subtitle">
              {user?.role === "Admin" ? "— All Employees" : "— My Requests"}
            </span>
          </h2>
          {user?.role === "Employee" && (
            <button
              className="btn btn-primary"
              onClick={() => setShowForm(!showForm)}
            >
              + Apply for Leave
            </button>
          )}
        </div>

        {/* Apply form */}
        {showForm && (
          <div className="card">
            <div className="card-title">New Leave Request</div>
            <form onSubmit={handleSubmit}>
              <div className="form-row">
                <div className="form-group">
                  <label>From Date *</label>
                  <input
                    type="date"
                    required
                    value={form.fromDate}
                    min={new Date().toISOString().split("T")[0]}
                    onChange={(e) =>
                      setForm({ ...form, fromDate: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>To Date *</label>
                  <input
                    type="date"
                    required
                    value={form.toDate}
                    min={new Date().toISOString().split("T")[0]}
                    onChange={(e) =>
                      setForm({ ...form, toDate: e.target.value })
                    }
                  />
                </div>
              </div>
              <div className="form-group" style={{ marginBottom: "12px" }}>
                <label>Reason *</label>
                <textarea
                  required
                  rows={3}
                  value={form.reason}
                  onChange={(e) => setForm({ ...form, reason: e.target.value })}
                  placeholder="Briefly describe your reason for leave"
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="btn btn-success">
                  Submit
                </button>
                <button
                  type="button"
                  className="btn btn-default"
                  onClick={() => setShowForm(false)}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}

        {/* Table */}
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                {user?.role === "Admin" && <th>Employee</th>}
                <th>From</th>
                <th>To</th>
                <th>Reason</th>
                <th>Status</th>
                <th>Applied On</th>
                {user?.role === "Admin" && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {leaves.length === 0 && (
                <tr className="empty-row">
                  <td colSpan="7">No leave requests found.</td>
                </tr>
              )}
              {leaves.map((leave) => (
                <tr key={leave.id}>
                  {user?.role === "Admin" && <td>{leave.employeeName}</td>}
                  <td>{leave.fromDate}</td>
                  <td>{leave.toDate}</td>
                  <td>{leave.reason}</td>
                  <td>
                    <span className={getBadgeClass(leave.status)}>
                      {leave.status}
                    </span>
                  </td>
                  <td>{leave.createdAt}</td>
                  {user?.role === "Admin" && (
                    <td>
                      {leave.status === "Pending" ? (
                        <>
                          <button
                            className="action-btn action-btn-approve"
                            onClick={() => handleApprove(leave.id)}
                          >
                            Approve
                          </button>

                          {rejectId === leave.id ? (
                            <div
                              style={{
                                display: "flex",
                                gap: "6px",
                                marginTop: "6px",
                                alignItems: "center",
                              }}
                            >
                              <input
                                placeholder="Remarks (optional)"
                                value={remarks}
                                onChange={(e) => setRemarks(e.target.value)}
                                style={{
                                  padding: "4px 8px",
                                  fontSize: "12px",
                                  border: "1px solid #ccc",
                                  borderRadius: "4px",
                                }}
                              />
                              <button
                                className="action-btn action-btn-reject"
                                onClick={() => handleReject(leave.id)}
                              >
                                Confirm
                              </button>
                              <button
                                className="action-btn btn-default"
                                onClick={() => {
                                  setRejectId(null);
                                  setRemarks("");
                                }}
                              >
                                Cancel
                              </button>
                            </div>
                          ) : (
                            <button
                              className="action-btn action-btn-reject"
                              onClick={() => setRejectId(leave.id)}
                            >
                              Reject
                            </button>
                          )}
                        </>
                      ) : (
                        <span style={{ color: "#bbb", fontSize: "12px" }}>
                          —
                        </span>
                      )}
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
