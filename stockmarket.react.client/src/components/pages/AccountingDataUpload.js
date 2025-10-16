import React, { useState, useEffect } from 'react';
import axios from 'axios';
import companyService from '../../services/companyService';

const AccountingDataUpload = () => {
  const [selectedFile, setSelectedFile] = useState(null);
  const [companyId, setCompanyId] = useState('');
  const [fiscalYear, setFiscalYear] = useState('');
  const [companies, setCompanies] = useState([]);
  const [analysisResult, setAnalysisResult] = useState(null);

  useEffect(() => {
    companyService.getCompanies().then(response => {
      setCompanies(response.data.companies);
    });
  }, []);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleFileChange = (event) => {
    setSelectedFile(event.target.files[0]);
  };

  const handleUpload = async () => {
    if (selectedFile) {
      const formData = new FormData();
      formData.append('file', selectedFile);
      formData.append('companyId', companyId);
      formData.append('fiscalYear', fiscalYear);

      try {
        setLoading(true);
        setError(null);
        const response = await axios.post(`${process.env.REACT_APP_API_URL}/accountingdata/upload`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });
        setAnalysisResult(response.data);
      } catch (err) {
        setError('File upload failed. Please try again.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <div className="container mt-4">
      <h2>Upload Accounting Data</h2>
      <div className="mb-3">
        <label htmlFor="companyId" className="form-label">Company</label>
        <select className="form-control" id="companyId" value={companyId} onChange={(e) => setCompanyId(e.target.value)}>
          <option value="">Select a company</option>
          {companies.map(company => (
            <option key={company.compCd} value={company.compCd}>{company.compNm}</option>
          ))}
        </select>
      </div>
      <div className="mb-3">
        <label htmlFor="fiscalYear" className="form-label">Fiscal Year</label>
        <input type="text" className="form-control" id="fiscalYear" value={fiscalYear} onChange={(e) => setFiscalYear(e.target.value)} />
      </div>
      <div className="mb-3">
        <label htmlFor="formFile" className="form-label">Select a PDF file</label>
        <input className="form-control" type="file" id="formFile" accept=".pdf" onChange={handleFileChange} />
      </div>
      <button className="btn btn-primary" onClick={handleUpload} disabled={!selectedFile || loading}>
        {loading ? 'Uploading...' : 'Upload and Analyze'}
      </button>

      {error && <div className="alert alert-danger mt-3">{error}</div>}

      {analysisResult && (
        <div className="mt-4">
          <h3>Analysis Result</h3>
          <pre>{JSON.stringify(analysisResult, null, 2)}</pre>
        </div>
      )}
    </div>
  );
};

export default AccountingDataUpload;
