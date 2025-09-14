import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import PrivateRoute from './components/auth/PrivateRoute';
import { AuthProvider } from './contexts/AuthContext';
import 'bootstrap/dist/css/bootstrap.min.css';
import MainLayout from './components/layout/MainLayout';
import Home from './components/pages/Home';
import CompanyList from './components/pages/CompanyList';
import StockList from './components/pages/StockList';
import UserList from './components/pages/UserList';
import StockChart from './StockChart';
import Chat from './Chat';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route 
            path="/*" 
            element={
              <PrivateRoute>
                <MainLayout>
                  <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/companies" element={<CompanyList />} />
                    <Route path="/stocks" element={<StockList />} />
                    <Route path="/users" element={<UserList />} />
                    <Route path="/chart" element={<StockChart />} />
                    <Route path="/chat" element={<Chat />} />
                  </Routes>
                </MainLayout>
              </PrivateRoute>
            }
          />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
