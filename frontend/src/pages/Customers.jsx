import { useEffect, useState } from "react";
import {
  getCustomers,
  updateCustomer,
  toggleCustomerActive,
} from "../services/customerService";

function Customers() {
  const [customers, setCustomers] = useState([]);
  const [error, setError] = useState(null);
  const [search, setSearch] = useState("");

  // Edit form
  const [editing, setEditing] = useState(null); // the customer being edited, or null
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [address, setAddress] = useState("");

  // Get customers from the API (with the search text, if any)
  function loadCustomers(text) {
    getCustomers(text)
      .then((data) => setCustomers(data))
      .catch((err) => setError(err.message));
  }

  // Run once when the page opens
  useEffect(() => {
    loadCustomers("");
  }, []);

  // Search button
  function handleSearch(e) {
    e.preventDefault();
    setError(null);
    loadCustomers(search);
  }

  // Clear button
  function handleClear() {
    setSearch("");
    setError(null);
    loadCustomers("");
  }

  // Edit button = put the customer values in the form
  function handleEdit(c) {
    setEditing(c);
    setFullName(c.fullName);
    setPhone(c.phone);
    setAddress(c.address);
  }

  // Save the edit (we send userId too, so the login link is kept)
  async function handleSave(e) {
    e.preventDefault();
    setError(null);
    try {
      await updateCustomer({
        id: editing.id,
        fullName,
        phone,
        address,
        userId: editing.userId,
      });
      setEditing(null);
      loadCustomers(search);
    } catch (err) {
      setError(err.message);
    }
  }

  // Cancel the edit
  function handleCancel() {
    setEditing(null);
  }

  // Activate / Deactivate
  async function handleToggle(id) {
    setError(null);
    try {
      await toggleCustomerActive(id);
    } catch (err) {
      setError(err.message);
    }
    loadCustomers(search);
  }

  return (
    <div className="container py-4">
      <h1>Customers</h1>
      {error && <div className="alert alert-danger">{error}</div>}

      <form onSubmit={handleSearch} className="row g-2 mb-4">
        <div className="col-md-5">
          <input
            className="form-control"
            placeholder="Search by name"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className="col-md-4">
          <button type="submit" className="btn btn-primary me-2">
            Search
          </button>
          <button type="button" className="btn btn-secondary" onClick={handleClear}>
            Clear
          </button>
        </div>
      </form>

      {editing && (
        <form onSubmit={handleSave} className="row g-2 mb-4">
          <div className="col-md-3">
            <input
              className="form-control"
              placeholder="Full name"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              required
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
          <div className="col-md-3">
            <input
              className="form-control"
              placeholder="Address"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
            />
          </div>
          <div className="col-md-3">
            <button type="submit" className="btn btn-primary me-2">
              Save
            </button>
            <button type="button" className="btn btn-secondary" onClick={handleCancel}>
              Cancel
            </button>
          </div>
        </form>
      )}

      <table className="table">
        <thead>
          <tr>
            <th>Full Name</th>
            <th>Phone</th>
            <th>Address</th>
            <th>Email</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {customers.map((c) => (
            <tr key={c.id}>
              <td>{c.fullName}</td>
              <td>{c.phone}</td>
              <td>{c.address}</td>
              <td>{c.email}</td>
              <td>{c.isActive ? "Active" : "Inactive"}</td>
              <td>
                <button
                  className="btn btn-success btn-sm me-1"
                  onClick={() => handleEdit(c)}
                >
                  Edit
                </button>
                <button
                  className={
                    c.isActive ? "btn btn-danger btn-sm" : "btn btn-primary btn-sm"
                  }
                  onClick={() => handleToggle(c.id)}
                >
                  {c.isActive ? "Deactivate" : "Activate"}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Customers;