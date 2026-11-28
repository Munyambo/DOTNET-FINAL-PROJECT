import { Link, useNavigate, useLocation } from 'react-router-dom'

function Navbar() {
  const navigate = useNavigate()
  useLocation()
  
  const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');
  const name = localStorage.getItem('name');

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };
  
  return (
  <nav className="navbar navbar-expand navbar-dark bg-dark px-3">

    <Link className="navbar-brand" to="/">RMS</Link>
    <Link className="nav-link text-white" to="/">Home</Link>

    {!token && (
      <>
        <Link className="nav-link text-white" to="/login">Login</Link>
        <Link className="nav-link text-white" to="/register">Register</Link>
      </>
    )}

    {/* only registered customers see this */}
    {role === "Customer" && (
      <>
        <Link className="nav-link text-white" to="/menu">Menu</Link>
        <Link className="nav-link text-white" to="/orders">Orders</Link>
        <Link className="nav-link text-white" to="/cart">Cart</Link>
        <Link className="nav-link text-white" to="/profile">Profile</Link>
      </>
    )}

    {token && (
      <div className="ms-auto text-white">
        <span className="me-3">{name}</span>
        <button className="btn btn-sm btn-light" onClick={handleLogout}>Logout</button>
      </div>
    )}

  </nav>
);
}


export default Navbar;