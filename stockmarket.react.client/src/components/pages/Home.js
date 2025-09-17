import React from 'react';
import { Link } from 'react-router-dom';
import MarketSummaryPieChart from '../MarketSummaryPieChart';
import MarketLeaders from '../MarketLeaders';

const Home = () => {
  return (
    <div className="container-fluid mt-4">
      <div className="row">
        <div className="col-lg-4 mb-4">
          <MarketSummaryPieChart />
        </div>
        <div className="col-lg-8 mb-4">
          <MarketLeaders />
        </div>
      </div>
      <div className="text-center">
        <Link to="/todays-price" className="btn btn-primary btn-lg">View All Market Data</Link>
      </div>
    </div>
  );
};

export default Home;
