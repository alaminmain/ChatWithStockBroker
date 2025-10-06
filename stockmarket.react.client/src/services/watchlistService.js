
import axios from 'axios';
import authService from './authService';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
});

api.interceptors.request.use(
  config => {
    const user = authService.getCurrentUser();
    if (user && user.token) {
      config.headers['Authorization'] = 'Bearer ' + user.token;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

export const getWatchlist = (date) => {
  return api.get('/watchlist', { params: { date } });
};

export const addToWatchlist = (compId) => {
  return api.post(`/watchlist/${compId}`, null, { headers: { 'Content-Type': 'application/json' } });
};

export const removeFromWatchlist = (compId) => {
  return api.delete(`/watchlist/${compId}`, { headers: { 'Content-Type': 'application/json' } });
};
