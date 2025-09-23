
import React, { useEffect, useState } from 'react';
import dividendInfoService from '../../services/dividendInfoService';

const DividendInfo = ({ compCd }) => {
  const [dividendInfo, setDividendInfo] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await dividendInfoService.getDividendInfoByCompCd(compCd);
        setDividendInfo(response.data);
      } catch (error) {
        console.error('Error fetching dividend info:', error);
      }
    };

    fetchData();
  }, [compCd]);

  return (
    <div>
      <h5>Dividend Information</h5>
      <table>
        <thead>
          <tr>
            <th>Year</th>
            <th>Type</th>
            <th>Rate</th>
          </tr>
        </thead>
        <tbody>
          {dividendInfo.map((info) => (
            <tr key={info.id}>
              <td>{info.fyear}</td>
              <td>{info.divType}</td>
              <td>{info.rate}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default DividendInfo;
