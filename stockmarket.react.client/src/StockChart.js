import React, { useState, useEffect } from 'react';
import { Chart } from 'lightweight-charts-react-wrapper';
import { getCompanies, getMarPriceData } from './api';

const StockChart = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [companies, setCompanies] = useState([]);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [marPriceData, setMarPriceData] = useState([]);
  const [selectedPeriod, setSelectedPeriod] = useState('1y');

  const handleSearch = async () => {
    if (searchQuery.length > 2) {
      try {
        const response = await getCompanies(searchQuery);
        setCompanies(response.data.companies);
      } catch (error) {
        console.error('Error fetching companies:', error);
      }
    }
  };

  const handleSelectCompany = (company) => {
    setSelectedCompany(company);
    setCompanies([]);
    fetchMarPriceData(company.compCd, selectedPeriod);
  };

  const fetchMarPriceData = async (compCd, period) => {
    try {
      const response = await getMarPriceData(compCd, period);
      const chartData = response.data.map(item => ({
        time: item.transDt.split('T')[0], // lightweight-charts expects 'YYYY-MM-DD'
        open: item.open,
        high: item.high,
        low: item.low,
        close: item.close
      }));
      setMarPriceData(chartData);
    } catch (error) {
      console.error('Error fetching market price data:', error);
    }
  };

  const handlePeriodChange = (e) => {
    const period = e.target.value;
    setSelectedPeriod(period);
    if (selectedCompany) {
      fetchMarPriceData(selectedCompany.compCd, period);
    }
  };

  const chartOptions = {
    layout: {
      background: { color: '#253248' },
      textColor: 'rgba(255, 255, 255, 0.9)',
    },
    grid: {
      vertLines: { color: '#334158' },
      horzLines: { color: '#334158' },
    },
    crosshair: {
      mode: 'normal',
    },
  };

  const series = [{
    type: 'Candlestick',
    data: marPriceData,
  }];

  return (
    <div className="container mt-4">
      <h2>Stock Chart</h2>
      <div className="mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Search Company by Name, Code, ISIN, or Instrument"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          onKeyUp={handleSearch}
        />
      </div>

      {companies.length > 0 && (
        <div className="list-group mb-3">
          {companies.map(company => (
            <a
              href="#"
              key={company.id}
              className="list-group-item list-group-item-action"
              onClick={(e) => { e.preventDefault(); handleSelectCompany(company); }}
            >
              {company.compNm} ({company.compCd}) - {company.isinCd}
            </a>
          ))}
        </div>
      )}

      {selectedCompany && (
        <div className="card mb-3">
          <div className="card-header">
            Selected Company: {selectedCompany.compNm} ({selectedCompany.compCd})
          </div>
          <div className="card-body">
            <p><strong>ISIN CD:</strong> {selectedCompany.isinCd}</p>
            <p><strong>Instrument Code:</strong> {selectedCompany.instrCd}</p>
          </div>
        </div>
      )}

      {selectedCompany && (
        <div className="mb-3">
          <label htmlFor="period-select" className="form-label">Select Period:</label>
          <select id="period-select" className="form-select" value={selectedPeriod} onChange={handlePeriodChange}>
            <option value="1y">1 Year</option>
            <option value="2y">2 Years</option>
            <option value="5y">5 Years</option>
            <option value="all">All</option>
          </select>
        </div>
      )}

      {marPriceData.length > 0 && (
        <div className="chart-container">
          <Chart options={chartOptions} series={series} autoSize={true} />
        </div>
      )}

      {selectedCompany && marPriceData.length === 0 && (
        <div className="alert alert-info">
          No market price data available for {selectedCompany.compNm}.
        </div>
      )}
    </div>
  );
};

export default StockChart;