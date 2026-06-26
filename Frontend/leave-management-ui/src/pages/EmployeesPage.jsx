import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import api from "../api/axiosInstance";
import toast from "react-hot-toast";

export default function EmployeesPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [employees, setEmployees] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    department: "",
    designation: "",
  });
  const [passwordError, setPasswordError] = useState("");
  const [editId, setEditId] = useState(null);
  const [editForm, setEditForm] = useState({
    firstName: "",
    lastName: "",
    department: "",
    designation: "",
  });

  useEffect(() => {
    fetchEmployees();
  }, []);

  async function fetchEmployees() {
    try {
      const res = await api.get("/employees");
      setEmployees(res.data);
    } catch {
      toast.error("Failed to load employees.");
    }
  }

  function validatePassword(value) {
    if (value.length === 0) {
      setPasswordError("");
      return;
    }
    if (value.length < 6) {
      setPasswordError("Min 6 characters required.");
      return;
    }
    if (!/[A-Z]/.test(value)) {
      setPasswordError("Must have at least one uppercase letter.");
      return;
    }
    if (!/[0-9]/.test(value)) {
      setPasswordError("Must have at least one number.");
      return;
    }
    setPasswordError("");
  }

  async function handleSave(e) {
    e.preventDefault();

    if (passwordError) {
      toast.error("Please fix the password before saving.");
      return;
    }

    try {
      await api.post("/employees", form);
      toast.success("Employee added successfully.");
      setShowForm(false);
      setPasswordError("");
      setForm({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        department: "",
        designation: "",
      });
      fetchEmployees();
    } catch (err) {
      toast.error(err.response?.data?.message || "Could not add employee.");
    }
  }

  async function handleEdit(e) {
    e.preventDefault();
    try {
      await api.put(`/employees/${editId}`, editForm);
      toast.success("Employee updated.");
      setEditId(null);
      fetchEmployees();
    } catch (err) {
      toast.error(err.response?.data?.message || "Could not update.");
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to remove this employee?"))
      return;
    try {
      await api.delete(`/employees/${id}`);
      toast.success("Employee removed.");
      fetchEmployees();
    } catch {
      toast.error("Could not remove employee.");
    }
  }

  function startEdit(emp) {
    setEditId(emp.id);
    setShowForm(false);
    setEditForm({
      firstName: emp.firstName,
      lastName: emp.lastName,
      department: emp.department || "",
      designation: emp.designation || "",
    });
  }

  return (
    <div>
      {/* Navbar */}
      <div className="navbar">
        <span className="navbar-title">Leave Management System</span>
        <div className="navbar-right">
          <span className="navbar-user">Hello, {user?.firstName} (Admin)</span>
          <button className="navbar-link" onClick={() => navigate("/leaves")}>
            Leave Requests
          </button>
          <button className="btn-logout" onClick={logout}>
            Logout
          </button>
        </div>
      </div>

      <div className="page-wrapper">
        {/* Page header */}
        <div className="page-header">
          <h2 className="page-title">Employees</h2>
          <button
            className="btn btn-primary"
            onClick={() => {
              setShowForm(!showForm);
              setEditId(null);
              setPasswordError("");
            }}
          >
            + Add Employee
          </button>
        </div>

        {/* Add form */}
        {showForm && (
          <div className="card">
            <div className="card-title">New Employee</div>
            <form onSubmit={handleSave}>
              <div className="form-row">
                <div className="form-group">
                  <label>First Name *</label>
                  <input
                    required
                    placeholder="First name"
                    value={form.firstName}
                    onChange={(e) =>
                      setForm({ ...form, firstName: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Last Name *</label>
                  <input
                    required
                    placeholder="Last name"
                    value={form.lastName}
                    onChange={(e) =>
                      setForm({ ...form, lastName: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Email *</label>
                  <input
                    required
                    type="email"
                    placeholder="Email address"
                    value={form.email}
                    onChange={(e) =>
                      setForm({ ...form, email: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Password *</label>
                  <input
                    required
                    type="password"
                    placeholder="Password"
                    value={form.password}
                    onChange={(e) => {
                      setForm({ ...form, password: e.target.value });
                      validatePassword(e.target.value);
                    }}
                    style={{ borderColor: passwordError ? "red" : "" }}
                  />
                  {passwordError && (
                    <span
                      style={{
                        color: "red",
                        fontSize: "12px",
                        marginTop: "3px",
                      }}
                    >
                      {passwordError}
                    </span>
                  )}
                </div>
                <div className="form-group">
                  <label>Department</label>
                  <input
                    placeholder="Department"
                    value={form.department}
                    onChange={(e) =>
                      setForm({ ...form, department: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Designation</label>
                  <input
                    placeholder="Designation"
                    value={form.designation}
                    onChange={(e) =>
                      setForm({ ...form, designation: e.target.value })
                    }
                  />
                </div>
              </div>
              <div className="form-actions">
                <button type="submit" className="btn btn-success">
                  Save
                </button>
                <button
                  type="button"
                  className="btn btn-default"
                  onClick={() => {
                    setShowForm(false);
                    setPasswordError("");
                  }}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}

        {/* Edit form */}
        {editId && (
          <div className="card" style={{ borderLeft: "4px solid #e65100" }}>
            <div className="card-title">Edit Employee</div>
            <form onSubmit={handleEdit}>
              <div className="form-row">
                <div className="form-group">
                  <label>First Name *</label>
                  <input
                    required
                    placeholder="First name"
                    value={editForm.firstName}
                    onChange={(e) =>
                      setEditForm({ ...editForm, firstName: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Last Name *</label>
                  <input
                    required
                    placeholder="Last name"
                    value={editForm.lastName}
                    onChange={(e) =>
                      setEditForm({ ...editForm, lastName: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Department</label>
                  <input
                    placeholder="Department"
                    value={editForm.department}
                    onChange={(e) =>
                      setEditForm({ ...editForm, department: e.target.value })
                    }
                  />
                </div>
                <div className="form-group">
                  <label>Designation</label>
                  <input
                    placeholder="Designation"
                    value={editForm.designation}
                    onChange={(e) =>
                      setEditForm({ ...editForm, designation: e.target.value })
                    }
                  />
                </div>
              </div>
              <div className="form-actions">
                <button type="submit" className="btn btn-warning">
                  Update
                </button>
                <button
                  type="button"
                  className="btn btn-default"
                  onClick={() => setEditId(null)}
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
                <th>Full Name</th>
                <th>Email</th>
                <th>Department</th>
                <th>Designation</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {employees.length === 0 && (
                <tr className="empty-row">
                  <td colSpan="5">No employees found.</td>
                </tr>
              )}
              {employees.map((emp) => (
                <tr key={emp.id}>
                  <td>
                    {emp.firstName} {emp.lastName}
                  </td>
                  <td>{emp.email}</td>
                  <td>{emp.department || "—"}</td>
                  <td>{emp.designation || "—"}</td>
                  <td>
                    <button
                      className="action-btn action-btn-edit"
                      onClick={() => startEdit(emp)}
                    >
                      Edit
                    </button>
                    <button
                      className="action-btn action-btn-delete"
                      onClick={() => handleDelete(emp.id)}
                    >
                      Remove
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
