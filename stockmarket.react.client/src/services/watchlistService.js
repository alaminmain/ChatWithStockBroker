
import axios from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
});

export const getWatchlist = (date) => {
  return api.get('/watchlist', { params: { date } });
};

export const addToWatchlist = (compId) => {
  return api.post(`/watchlist/${compId}`);
};

export const removeFromWatchlist = (compId) => {
  return api.delete(`/watchlist/${compId}`);
};
