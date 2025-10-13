import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import companyService from '../../services/companyService';

const FinancialReports = () => {
  const { companyId } = useParams();
  const [company, setCompany] = useState(null);
  const [reports, setReports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchReports = async () => {
      try {
        setLoading(true);
        setError(null);
        const companyRes = await companyService.getCompany(companyId);
        setCompany(companyRes.data);
        const reportsRes = await companyService.getFinancialReports(companyId);
        setReports(reportsRes.data);
      } catch (err) {
        setError('Failed to fetch financial reports.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchReports();
  }, [companyId]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div className="alert alert-danger">{error}</div>;
  }

  return (
    <div className="container mt-4">
      <h2>Financial Reports for {company?.compNm}</h2>
      {reports.length === 0 ? (
        <p>No financial reports found for this company.</p>
      ) : (
        reports.map(report => (
          <div key={report.id} className="card mb-3">
            <div className="card-header">
              <strong>{report.statementType} - {report.year}</strong>
            </div>
            <div className="card-body">
              <table className="table table-sm">
                <thead>
                  <tr>
                    <th>Account</th>
                    <th className="text-end">Value</th>
                  </tr>
                </thead>
                <tbody>
                  {report.entries.map(entry => (
                    <tr key={entry.id}>
                      <td>{entry.standardAccountName}</td>
                      <td className="text-end">{entry.value.toLocaleString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        ))
      )}
    </div>
  );
};

export default FinancialReports;
