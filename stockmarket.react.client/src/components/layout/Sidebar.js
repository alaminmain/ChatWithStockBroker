
import React from 'react';
import { NavLink } from 'react-router-dom';
import './Sidebar.css';

const Sidebar = () => {
  return (
    <div className="sidebar">
      <ul className="nav flex-column">
        <li className="nav-item">
          <NavLink to="/" className="nav-link" end>
            Dashboard
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/companies" className="nav-link">
            Company List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/stocks" className="nav-link">
            Stock List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/users" className="nav-link">
            User List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/chart" className="nav-link">
            Stock Chart
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/chat" className="nav-link">
            Chat
          </NavLink>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
