
import React, { useEffect, useState } from 'react';
import { getWatchlist, removeFromWatchlist } from '../../services/watchlistService';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

import PriceChart from './PriceChart';

import DividendInfo from './DividendInfo';

const Watchlist = () => {
  const [watchlist, setWatchlist] = useState([]);
  const [selectedDate, setSelectedDate] = useState(new Date());

  useEffect(() => {
    fetchWatchlist();
  }, [selectedDate]);

  const fetchWatchlist = async () => {
    try {
      const response = await getWatchlist(selectedDate);
      setWatchlist(response.data);
    } catch (error) {
      console.error('Error fetching watchlist:', error);
    }
  };

  const handleRemoveFromWatchlist = async (compId) => {
    try {
      await removeFromWatchlist(compId);
      fetchWatchlist(); // Refresh the watchlist
    } catch (error) {
      console.error('Error removing from watchlist:', error);
    }
  };

  return (
    <div>
      <h2>My Watchlist</h2>
      <DatePicker selected={selectedDate} onChange={(date) => setSelectedDate(date)} />
      <table>
        <thead>
          <tr>
            <th>Company Name</th>
            <th>Latest Price</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {watchlist.map((item) => (
            <React.Fragment key={item.id}>
              <tr>
                <td>{item.compNm}</td>
                <td>{item.latestPrice?.close}</td>
                <td>
                  <button onClick={() => handleRemoveFromWatchlist(item.id)}>Remove</button>
                </td>
              </tr>
              <tr>
                <td colSpan="3">
                  <PriceChart compCd={item.compCd} />
                </td>
              </tr>
              <tr>
                <td colSpan="3">
                  <DividendInfo compCd={item.compCd} />
                </td>
              </tr>
            </React.Fragment>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Watchlist;
