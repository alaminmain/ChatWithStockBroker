import axios from "axios";

const API_URL = process.env.REACT_APP_API_URL;

export const getCompanies = (
  search,
  pageNumber,
  pageSize,
  sortBy,
  sortDirection
) => {
  return axios.get(`${API_URL}/StockMarket/companies`, {
    params: { search, pageNumber, pageSize, sortBy, sortDirection },
  });
};

export const getMarPriceData = (compCd, period) => {
  return axios.get(
    `${API_URL}/StockMarket/marprice/${compCd}?period=${period}`
  );
};

export const getUsers = () => {
  return axios.get(`${API_URL}/Users`);
};

export const getLatestStockPrices = () => {
  return axios.get(`${API_URL}/StockMarket/stocks/latest`);
};

export const getMarketLeaders = () => {
  return axios.get(`${API_URL}/StockMarket/market-leaders`);
};

export const getCompanyDetails = (compCd) => {
  return axios.get(`${API_URL}/StockMarket/companies/${compCd}`);
};

export const getHeatmapData = () => {
  return axios.get(`${API_URL}/StockMarket/heatmap-data`);
};

export const getMarketSummary = () => {
  return axios.get(`${API_URL}/StockMarket/market-summary`);
};
