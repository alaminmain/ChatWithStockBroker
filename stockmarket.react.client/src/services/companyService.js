import axios from 'axios';

const API_URL = `${process.env.REACT_APP_API_URL}`;

const getCompanies = () => {
  return axios.get(`${API_URL}/StockMarket/companies`);
};

const getCompany = (id) => {
  return axios.get(`${API_URL}/StockMarket/companies/${id}`);
};

const getFinancialReports = (companyId) => {
  return axios.get(`${API_URL}/accountingdata/${companyId}`);
};

const getCompanyProfile = (companyId) => {
    return axios.get(`${API_URL}/StockMarket/company-profile/${companyId}`);
};


const companyService = {
  getCompanies,
  getCompany,
  getFinancialReports,
  getCompanyProfile,
};

export default companyService;
