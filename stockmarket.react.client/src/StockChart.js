import React, { useState, useEffect, useMemo } from 'react';
import Chart from 'react-apexcharts'; // Import ApexCharts
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
      console.log("Raw API response data:", response.data);
      // Transform data for ApexCharts candlestick
      const chartData = response.data.map(item => ({
        x: new Date(item.transDt).getTime(), // ApexCharts expects timestamp
        y: [item.open, item.high, item.low, item.close]
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

  // ApexCharts options
  const chartOptions = useMemo(() => ({
    chart: {
      type: 'candlestick',
      height: 350,
      background: '#253248',
      foreColor: 'rgba(255, 255, 255, 0.9)',
    },
    title: {
      text: 'Candlestick Chart',
      align: 'left',
      style: {
        color: 'rgba(255, 255, 255, 0.9)'
      }
    },
    xaxis: {
      type: 'datetime',
      tickPlacement: 'on',
      labels: {
        style: {
          colors: 'rgba(255, 255, 255, 0.7)'
        }
      }
    },
    yaxis: {
      tooltip: {
        enabled: true
      },
      labels: {
        style: {
          colors: 'rgba(255, 255, 255, 0.7)'
        }
      }
    },
    grid: {
      borderColor: '#334158'
    },
    plotOptions: {
      candlestick: {
        colors: {
          upward: '#00B746',
          downward: '#EF403C'
        }
      }
    }
  }), []);

  // ApexCharts series data
  const series = useMemo(() => [{
    name: 'Candlestick',
    data: marPriceData,
  }], [marPriceData]);

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
          <Chart options={chartOptions} series={series} type="candlestick" height={350} />
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
