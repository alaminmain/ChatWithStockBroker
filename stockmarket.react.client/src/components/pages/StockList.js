import React, { useState, useEffect } from 'react';
import { getLatestStockPrices } from '../../api';

const StockList = () => {
  const [stocks, setStocks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchStockPrices = async () => {
      try {
        setLoading(true);
        const response = await getLatestStockPrices();
        setStocks(response.data);
        setError(null);
      } catch (err) {
        setError('Failed to fetch stock prices.');
        console.error(err);
      }
      setLoading(false);
    };

    fetchStockPrices();
  }, []);

  return (
    <div>
      <h2>Latest Stock Prices</h2>
      {loading && <p>Loading...</p>}
      {error && <div className="alert alert-danger">{error}</div>}
      {!loading && !error && (
        <table className="table table-striped">
          <thead>
            <tr>
              <th>Instrument Code</th>
              <th>Company Code</th>
              <th>Date</th>
              <th>Open</th>
              <th>High</th>
              <th>Low</th>
              <th>Close</th>
              <th>Volume</th>
            </tr>
          </thead>
          <tbody>
            {stocks.map((stock) => (
              <tr key={stock.instCd}>
                <td>{stock.instCd}</td>
                <td>{stock.compCd}</td>
                <td>{new Date(stock.transDt).toLocaleDateString()}</td>
                <td>{stock.open}</td>
                <td>{stock.high}</td>
                <td>{stock.low}</td>
                <td>{stock.close}</td>
                <td>{stock.vol}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default StockList;