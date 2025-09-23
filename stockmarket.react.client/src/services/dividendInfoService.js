import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL + '/DividendInfo';

const getDividendInfo = (compCd, divType, bokClFdt) => {
  return axios.get(API_URL, {
    params: {
      compCd,
      divType,
      bokClFdt: bokClFdt ? bokClFdt.toISOString() : null, // Format date for API
    },
  });
};

const getDividendInfoById = (id) => {
  return axios.get(`${API_URL}/${id}`);
};

const createDividendInfo = (data) => {
  return axios.post(API_URL, data);
};

const updateDividendInfo = (id, data) => {
  return axios.put(`${API_URL}/${id}`, data);
};

const deleteDividendInfo = (id) => {
  return axios.delete(`${API_URL}/${id}`);
};

const getDividendInfoByCompCd = (compCd) => {
    return axios.get(`${API_URL}/company/${compCd}`);
};

const dividendInfoService = {
  getDividendInfo,
  getDividendInfoById,
  createDividendInfo,
  updateDividendInfo,
  deleteDividendInfo,
  getDividendInfoByCompCd,
};

export default dividendInfoService;
