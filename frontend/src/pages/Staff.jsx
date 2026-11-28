import { useEffect, useState } from "react";
import {
  getStaff,
  createStaff,
  updateStaff,
  deleteStaff,
} from "../services/staffService";
import { getBranches } from "../services/branchService";

function Staff() {
  const [staffList, setStaffList] = useState([]);
  const [branches, setBranches] = useState([]);
  const [error, setError] = useState(null);

  // Form fields
  const [editing, setEditing] = useState(null); // the staff being edited, or null
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [position, setPosition] = useState("");
  const [isManager, setIsManager] = useState(false);
  const [branchId, setBranchId] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  // Get the staff and the branches (for the dropdown)
  function loadStaff() {
    getStaff()
      .then((data) => setStaffList(data))
      .catch((err) => setError(err.message));
  }

  useEffect(() => {
    loadStaff();
    getBranches()
      .then((data) => setBranches(data))
      .catch((err) => setError(err.message));
  }, []);

  // Empty the form
  function clearForm() {
    setEditing(null);
    setFullName("");
    setPhone("");
    setPosition("");
    setIsManager(false);
    setBranchId("");
    setEmail("");
    setPassword("");
  }

  // Save = create a new staff, or update the one being edited
  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    try {
      if (editing === null) {
        await createStaff({
          fullName,
          phone,
          position,
          isManager,
          branchId: Number(branchId),
          email,
          password,
        });
      } else {
        // we send userId too, so the login link is kept
        await updateStaff({
          id: editing.id,
          fullName,
          phone,
          position,
          isManager,
          branchId: Number(branchId),
          userId: editing.userId,
        });
      }
      clearForm();
      loadStaff();
    } catch (err) {
      setError(err.message);
    }
  }

  // Edit = put the staff values in the form
  function handleEdit(s) {
    setEditing(s);
    setFullName(s.fullName);
    setPhone(s.phone || "");
    setPosition(s.position || "");
    setIsManager(s.isManager);
    setBranchId(String(s.branchId));
  }

  // Delete the staff (the API also deletes the login)
  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this staff member?")) return;
    setError(null);
    try {
      await deleteStaff(id);
    } catch (err) {
      setError(err.message);
    }
    loadStaff();
  }

  return (
    <div className="container py-4">
      <h1>Staff</h1>
      {error && <div className="alert alert-danger">{error}</div>}

      <form onSubmit={handleSubmit} className="row g-2 mb-4">
        <div className="col-md-3">
          <input
            className="form-control"
            placeholder="Full name"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            required
          />
        </div>
        <div className="col-md-2">
          <input
            className="form-control"
            placeholder="Phone"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </div>
        <div className="col-md-2">
          <input
            className="form-control"
            placeholder="Position"
            value={position}
            onChange={(e) => setPosition(e.target.value)}
          />
        </div>
        <div className="col-md-3">
          <select
            className="form-select"
            value={branchId}
            onChange={(e) => setBranchId(e.target.value)}
            required
          >
            <option value="">-- Select branch --</option>
            {branches.map((b) => (
              <option key={b.id} value={b.id}>
                {b.name}
              </option>
            ))}
          </select>
        </div>
        <div className="col-md-2 d-flex align-items-center">
          <input
            type="checkbox"
            className="form-check-input me-2"
            id="isManager"
            checked={isManager}
            onChange={(e) => setIsManager(e.target.checked)}
          />
          <label htmlFor="isManager">Manager</label>
        </div>

        {/* Email and password only when creating */}
        {editing === null && (
          <>
            <div className="col-md-4">
              <input
                type="email"
                className="form-control"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="col-md-4">
              <input
                type="password"
                className="form-control"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
          </>
        )}

        <div className="col-md-4">
          <button type="submit" className="btn btn-primary me-2">
            {editing === null ? "New Staff" : "Save"}
          </button>
          {editing !== null && (
            <button type="button" className="btn btn-secondary" onClick={clearForm}>
              Cancel
            </button>
          )}
        </div>
      </form>

      <table className="table">
        <thead>
          <tr>
            <th>Full Name</th>
            <th>Phone</th>
            <th>Position</th>
            <th>Manager</th>
            <th>Branch</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {staffList.map((s) => (
            <tr key={s.id}>
              <td>{s.fullName}</td>
              <td>{s.phone}</td>
              <td>{s.position}</td>
              <td>{s.isManager ? "Yes" : "No"}</td>
              <td>{s.branch?.name}</td>
              <td>
                <button
                  className="btn btn-success btn-sm me-1"
                  onClick={() => handleEdit(s)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => handleDelete(s.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Staff;