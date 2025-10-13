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
import CompanyProfile from './components/pages/CompanyProfile'; // Import CompanyProfile
import CompanyAnalytics from './components/pages/CompanyAnalytics'; // Import CompanyAnalytics
import TodaysPrice from './components/pages/TodaysPrice';
import HeatmapPage from './components/pages/HeatmapPage';
import DividendInfoPage from './components/pages/DividendInfoPage';
import Watchlist from './components/pages/Watchlist';
import AccountingDataUpload from './components/pages/AccountingDataUpload';
import FinancialReports from './components/pages/FinancialReports'; // Import FinancialReports

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
                    <Route path="/todays-price" element={<TodaysPrice />} />
                    <Route path="/heatmap" element={<HeatmapPage />} />
                    <Route path="/companies" element={<CompanyList />} />
                    <Route path="/stocks" element={<StockList />} />
                    <Route path="/users" element={<UserList />} />
                    <Route path="/chart" element={<StockChart />} />
                    <Route path="/chat" element={<Chat />} />
                    <Route path="/company-profile/:id" element={<CompanyProfile />} /> {/* New route for company profile */}
                    <Route path="/company-analytics" element={<CompanyAnalytics />} /> {/* New route for company analytics */}
                    <Route path="/dividend-info" element={<DividendInfoPage />} />
                    <Route path="/watchlist" element={<Watchlist />} />
                    <Route path="/upload-accounting-data" element={<AccountingDataUpload />} />
                    <Route path="/company/:companyId/reports" element={<FinancialReports />} />
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