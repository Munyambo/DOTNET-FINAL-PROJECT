import { useEffect, useState } from "react";
import {
  getBranches,
  createBranch,
  updateBranch,
  deleteBranch,
} from "../services/branchService";

function Branches() {
  const [branches, setBranches] = useState([]);
  const [error, setError] = useState(null);
  const [name, setName] = useState("");
  const [address, setAddress] = useState("");
  const [phone, setPhone] = useState("");
  const [editingId, setEditingId] = useState(null);

  // Get all branches from the API
  function loadBranches() {
    getBranches()
      .then((data) => setBranches(data))
      .catch((err) => setError(err.message));
  }

  // Run once when the page opens
  useEffect(() => {
    loadBranches();
  }, []);

  // Save = create a new branch, or update the one being edited
  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    try {
      if (editingId === null) {
        await createBranch({ name, address, phone });
      } else {
        await updateBranch({ id: editingId, name, address, phone });
      }
      setName("");
      setAddress("");
      setPhone("");
      setEditingId(null);
      loadBranches();
    } catch (err) {
      setError(err.message);
    }
  }

  // Edit = put the branch values in the form
  function handleEdit(b) {
    setEditingId(b.id);
    setName(b.name);
    setAddress(b.address);
    setPhone(b.phone);
  }

  // Delete one branch
  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this branch?")) return;
    setError(null);
    try {
      await deleteBranch(id);
    } catch (err) {
      setError(err.message);
    }
    loadBranches();
  }

  return (
    <div className="container py-4">
      <h1>Branches</h1>
      {error && <div className="alert alert-danger">{error}</div>}

      <form onSubmit={handleSubmit} className="row g-2 mb-4">
        <div className="col-md-3">
          <input
            className="form-control"
            placeholder="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        </div>
        <div className="col-md-4">
          <input
            className="form-control"
            placeholder="Address"
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
        </div>
        <div className="col-md-3">
          <input
            className="form-control"
            placeholder="Phone"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </div>
        <div className="col-md-2">
          <button type="submit" className="btn btn-primary">
            {editingId === null ? "New Branch" : "Save"}
          </button>
        </div>
      </form>

      <table className="table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Address</th>
            <th>Phone</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {branches.map((b) => (
            <tr key={b.id}>
              <td>{b.name}</td>
              <td>{b.address}</td>
              <td>{b.phone}</td>
              <td>
                <button
                  className="btn btn-success btn-sm me-1"
                  onClick={() => handleEdit(b)}
                >
                  Edit
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => handleDelete(b.id)}
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

export default Branches;