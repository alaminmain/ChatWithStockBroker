
import React from 'react';
import { NavLink } from 'react-router-dom';
import './Sidebar.css';

const Sidebar = () => {
  return (
    <div className="sidebar">
      <ul className="nav flex-column">
        <li className="nav-item">
          <NavLink to="/" className="nav-link" end>
            <i className="fas fa-tachometer-alt me-2"></i>Dashboard
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/companies" className="nav-link">
            <i className="fas fa-building me-2"></i>Company List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/company-analytics" className="nav-link">
            <i className="fas fa-chart-bar me-2"></i>Company Analytics
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/heatmap" className="nav-link">
            <i className="fas fa-th me-2"></i>Heatmap
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/stocks" className="nav-link">
            <i className="fas fa-chart-line me-2"></i>Stock List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/users" className="nav-link">
            <i className="fas fa-users me-2"></i>User List
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/chart" className="nav-link">
            <i className="fas fa-chart-area me-2"></i>Stock Chart
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/chat" className="nav-link">
            <i className="fas fa-comments me-2"></i>Chat
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/dividend-info" className="nav-link">
            <i className="fas fa-money-bill-wave me-2"></i>Dividend Info
          </NavLink>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
