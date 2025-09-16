import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

const Navbar = () => {
  const { currentUser, logout } = useAuth();

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container-fluid">
        <Link className="navbar-brand" to="/">StockNow</Link>
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            <li className="nav-item">
              <Link className="nav-link" to="/"><i className="fas fa-home me-2"></i>Home</Link>
            </li>
            {currentUser ? (
              <li className="nav-item">
                <a href="#!" className="nav-link" onClick={logout}><i className="fas fa-sign-out-alt me-2"></i>Logout</a>
              </li>
            ) : (
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/login"><i className="fas fa-sign-in-alt me-2"></i>Login</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/register"><i className="fas fa-user-plus me-2"></i>Register</Link>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;