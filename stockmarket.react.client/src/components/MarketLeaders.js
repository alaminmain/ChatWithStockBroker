import React, { useState, useEffect } from 'react';
import { getMarketLeaders } from '../api';
import './MarketLeaders.css';

const MarketLeaders = () => {
  const [leaders, setLeaders] = useState(null);
  const [activeTab, setActiveTab] = useState('topGainers');
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchLeaders = async () => {
      try {
        const response = await getMarketLeaders();
        setLeaders(response.data);
      } catch (err) {
        setError('Could not load market leaders.');
        console.error(err);
      }
    };
    fetchLeaders();
  }, []);

  const renderTable = (data, tab) => {
    if (!data || data.length === 0) {
      return <p>No data available for {tab}.</p>;
    }

    return (
      <table className="table table-sm table-hover leaders-table">
        <thead>
          <tr>
            <th>Symbol</th>
            <th>LTP</th>
            {tab.toLowerCase().includes('gainer') || tab.toLowerCase().includes('loser') ? (
              <th>% Change</th>
            ) : (
              <th>{tab.replace('top','')}</th>
            )}
          </tr>
        </thead>
        <tbody>
          {data.map((item, index) => (
            <tr key={index}>
              <td>{item.instCd}</td>
              <td>{item.close?.toFixed(2)}</td>
              {tab.toLowerCase().includes('gainer') || tab.toLowerCase().includes('loser') ? (
                <td className={item.chg > 0 ? 'text-success' : 'text-danger'}>
                  {item.changePercent?.toFixed(2)}%
                </td>
              ) : (
                <td>{item[tab.replace('top','').toLowerCase()]?.toLocaleString()}</td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    );
  };

  const renderActiveTabContent = () => {
    if (error) return <p className="text-danger">{error}</p>;
    if (!leaders) return <p>Loading leaders...</p>;

    return renderTable(leaders[activeTab], activeTab);
  };

  return (
    <div className="market-leaders-container card">
        <div className="card-header">
            <ul className="nav nav-tabs card-header-tabs">
                <li className="nav-item">
                    <button className={`nav-link ${activeTab === 'topGainers' ? 'active' : ''}`} onClick={() => setActiveTab('topGainers')}>Top Gainers</button>
                </li>
                <li className="nav-item">
                    <button className={`nav-link ${activeTab === 'topLosers' ? 'active' : ''}`} onClick={() => setActiveTab('topLosers')}>Top Losers</button>
                </li>
                <li className="nav-item">
                    <button className={`nav-link ${activeTab === 'topVolume' ? 'active' : ''}`} onClick={() => setActiveTab('topVolume')}>Top Volume</button>
                </li>
                <li className="nav-item">
                    <button className={`nav-link ${activeTab === 'topValue' ? 'active' : ''}`} onClick={() => setActiveTab('topValue')}>Top Value</button>
                </li>
            </ul>
        </div>
        <div className="card-body">
            {renderActiveTabContent()}
        </div>
    </div>
  );
};

export default MarketLeaders;
