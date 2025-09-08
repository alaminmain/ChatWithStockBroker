import axios from 'axios';

const API_URL = 'http://localhost:5000/api/StockMarket'; // Assuming the API is running on port 5000

export const getCompanies = (search) => {
  return axios.get(`${API_URL}/companies`, {
    params: { search, pageNumber: 1, pageSize: 10 }
  });
};

export const getMarPriceData = (compCd, period) => {
  return axios.get(`${API_URL}/marprice/${compCd}?period=${period}`);
};
