import React, { useState, useEffect } from 'react';
import { Pie } from 'react-chartjs-2';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { getMarketSummary } from '../api';

ChartJS.register(ArcElement, Tooltip, Legend);

const MarketSummaryPieChart = () => {
  const [chartData, setChartData] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchMarketSummary = async () => {
      try {
        const response = await getMarketSummary();
        const summary = response.data;

        const data = {
          labels: [
            `Gainers (${summary.gainers.percentage}%)`,
            `Losers (${summary.losers.percentage}%)`,
            `Unchanged (${summary.unchanged.percentage}%)`
          ],
          datasets: [
            {
              label: '# of Instruments',
              data: [summary.gainers.count, summary.losers.count, summary.unchanged.count],
              backgroundColor: [
                'rgba(75, 192, 192, 0.6)', // Green
                'rgba(255, 99, 132, 0.6)',  // Red
                'rgba(201, 203, 207, 0.6)'  // Grey
              ],
              borderColor: [
                'rgba(75, 192, 192, 1)',
                'rgba(255, 99, 132, 1)',
                'rgba(201, 203, 207, 1)'
              ],
              borderWidth: 1,
            },
          ],
        };
        setChartData(data);
      } catch (error) {
        console.error('Error fetching market summary:', error);
        setError('Could not load market summary data.');
      }
    };

    fetchMarketSummary();
  }, []);

  if (error) {
    return <div style={{ textAlign: 'center', color: 'red' }}>{error}</div>;
  }

  if (!chartData) {
    return <div>Loading market summary...</div>;
  }

  return (
    <div style={{ width: '400px', margin: 'auto', marginBottom: '20px' }}>
      <h2 style={{ textAlign: 'center' }}>Today's Market Summary</h2>
      <Pie data={chartData} />
    </div>
  );
};

export default MarketSummaryPieChart;
