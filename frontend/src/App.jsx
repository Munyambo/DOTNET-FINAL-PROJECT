import { useEffect, useState } from "react";
import { getBranches } from "./services/branchService";

function App() {
  const [branches, setBranches] = useState([]);
  const [error, setError] = useState(null);

  // When the page opens, get all branches from the API
  useEffect(() => {
    getBranches()
      .then((data) => setBranches(data))
      .catch((err) => setError(err.message));
  }, []);

  return (
    <div className="container py-5">
      <h1>RMS - Restaurant</h1>
      {error && <div className="alert alert-danger">{error}</div>}

      <h4 className="mt-4">Branches List</h4>
      <ul className="list-group">
        {branches.map((b) => (
          <li key={b.id} className="list-group-item">
            {b.name} — {b.address} — {b.phone}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;