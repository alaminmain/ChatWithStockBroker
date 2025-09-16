import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom'; // Import Link
import axios from 'axios';
import { formatCurrency } from '../../utils/formatters';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Pie } from 'react-chartjs-2';

ChartJS.register(ArcElement, Tooltip, Legend);

const CompanyProfile = () => {
  const { id } = useParams();
  const [company, setCompany] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('basic'); // State for active tab

  useEffect(() => {
    const fetchCompanyDetails = async () => {
      try {
        const response = await axios.get(`${process.env.REACT_APP_API_URL}/StockMarket/companies/${id}`);
        setCompany(response.data);
      } catch (err) {
        setError('Failed to fetch company details.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchCompanyDetails();
  }, [id]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        <p className="ms-2">Loading company profile...</p>
      </div>
    );
  }

  if (error) {
    return <div className="alert alert-danger text-center mt-4">{error}</div>;
  }

  if (!company) {
    return <div className="alert alert-info text-center mt-4">No company data found.</div>;
  }

  const pieChartData = {
    labels: ['Director', 'Foreign', 'Government', 'Institute', 'Public'],
    datasets: [
      {
        label: 'Share Percentage',
        data: [
          company.sharePercentageDirector || 0,
          company.sharePercentageForeign || 0,
          company.sharePercentageGovt || 0,
          company.sharePercentageInstitute || 0,
          company.sharePercentagePublic || 0,
        ],
        backgroundColor: [
          'rgba(255, 99, 132, 0.6)',
          'rgba(54, 162, 235, 0.6)',
          'rgba(255, 206, 86, 0.6)',
          'rgba(75, 192, 192, 0.6)',
          'rgba(153, 102, 255, 0.6)',
        ],
        borderColor: [
          'rgba(255, 99, 132, 1)',
          'rgba(54, 162, 235, 1)',
          'rgba(255, 206, 86, 1)',
          'rgba(75, 192, 192, 1)',
          'rgba(153, 102, 255, 1)',
        ],
        borderWidth: 1,
      },
    ],
  };

  return (
    <div className="container mt-4">
      <Link to="/companies" className="btn btn-secondary mb-3"><i className="fas fa-arrow-left me-2"></i>Back to Company List</Link>
      <div className="card shadow-sm">
        <div className="card-header bg-primary text-white">
          <h2 className="mb-0"><i className="fas fa-building me-2"></i>{company.compNm} ({company.compSrtNm})</h2>
        </div>
        <div className="card-body">
          <ul className="nav nav-tabs mb-3">
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'basic' ? 'active' : ''}`}
                onClick={() => setActiveTab('basic')}
              >
                <i className="fas fa-info-circle me-2"></i>Basic Information
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'financial' ? 'active' : ''}`}
                onClick={() => setActiveTab('financial')}
              >
                <i className="fas fa-chart-line me-2"></i>Financial Information
              </button>
            </li>
            <li className="nav-item">
              <button
                className={`nav-link ${activeTab === 'other' ? 'active' : ''}`}
                onClick={() => setActiveTab('other')}
              >
                <i className="fas fa-ellipsis-h me-2"></i>Other Information
              </button>
            </li>
          </ul>

          <div className="tab-content">
            {/* Basic Information Tab */}
            <div className={`tab-pane fade ${activeTab === 'basic' ? 'show active' : ''}`}>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-hashtag me-2"></i>Company Code:</strong> {company.compCd || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-industry me-2"></i>Sector Major:</strong> {company.sectMajCd || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-map-marker-alt me-2"></i>Address:</strong> {company.regOff || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-envelope me-2"></i>Email:</strong> {company.eMail || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-phone me-2"></i>Telephone:</strong> {company.tel || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-fax me-2"></i>Fax:</strong> {company.fax || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-globe me-2"></i>Website:</strong> {company.website ? <a href={company.website} target="_blank" rel="noopener noreferrer">{company.website}</a> : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-alt me-2"></i>Listing Date:</strong> {company.lstDt ? new Date(company.lstDt).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-money-bill-wave me-2"></i>Face Value:</strong> {formatCurrency(company.fcVal)}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-boxes me-2"></i>Market Lot:</strong> {company.mlot || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-sort-numeric-up-alt me-2"></i>Total Securities:</strong> {company.noShrs || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-share-alt me-2"></i>Share Type:</strong> {company.catTp || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-check me-2"></i>Year End:</strong> {company.yearEnd ? new Date(company.yearEnd).toLocaleDateString() : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-day me-2"></i>Last AGM Held:</strong> {company.lastAgmHeld ? new Date(company.lastAgmHeld).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-box-open me-2"></i>Products:</strong> {company.prod || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-handshake me-2"></i>Sponsors:</strong> {company.spnr || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-gavel me-2"></i>Auditor:</strong> {company.auditor || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-cogs me-2"></i>Operational Status:</strong> {company.operationalStatus || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-12">
                  <strong><i className="fas fa-comment-alt me-2"></i>Remarks:</strong> {company.remarks || 'N/A'}
                </div>
              </div>
            </div>

            {/* Financial Information Tab */}
            <div className={`tab-pane fade ${activeTab === 'financial' ? 'show active' : ''}`}>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-money-check-alt me-2"></i>Paid-up Capital:</strong> {formatCurrency(company.paidCap)}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-money-bill-alt me-2"></i>Authorized Capital:</strong> {formatCurrency(company.athoCap)}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-chart-pie me-2"></i>Earning Per Share:</strong> {company.earningPerShare || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-chart-area me-2"></i>Net Asset Value Per Share:</strong> {company.netAssetValPerShare || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-hand-holding-usd me-2"></i>NOCF Per Share:</strong> {company.nocfPerShare || 'N/A'}
                </div>
              </div>
              <h4 className="mt-4"><i className="fas fa-chart-pie me-2"></i>Shareholding Pattern</h4>
              <div style={{ width: '400px', margin: 'auto' }}>
                <Pie data={pieChartData} />
              </div>
            </div>

            {/* Other Information Tab */}
            <div className={`tab-pane fade ${activeTab === 'other' ? 'show active' : ''}`}>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-barcode me-2"></i>Instrument Code:</strong> {company.instrCd || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-fingerprint me-2"></i>ISIN Code:</strong> {company.isinCd || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-alt me-2"></i>Listing Year:</strong> {company.listingYear || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-plus me-2"></i>Start Date:</strong> {company.startDt ? new Date(company.startDt).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-percent me-2"></i>Margin:</strong> {company.margin || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-exchange-alt me-2"></i>Trading Method:</strong> {company.tradeMeth || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-desktop me-2"></i>Trading Platform:</strong> {company.tradePlatform || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-code me-2"></i>CSE Instrument Code:</strong> {company.cseInstrCd || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-list-ol me-2"></i>Index List:</strong> {company.indxLst || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-day me-2"></i>Base Update Date:</strong> {company.baseUpdDt ? new Date(company.baseUpdDt).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-hdd me-2"></i>CDS:</strong> {company.cds || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-sliders-h me-2"></i>Control Rate:</strong> {company.ctlRt || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-network-wired me-2"></i>Net:</strong> {company.net || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-layer-group me-2"></i>Group:</strong> {company.grp || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-university me-2"></i>Merchant Bank ID:</strong> {company.merchanBankId || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-store-alt me-2"></i>OTC:</strong> {company.otc || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-times me-2"></i>IPO Cutoff Date:</strong> {company.ipoCutoffDt ? new Date(company.ipoCutoffDt).toLocaleDateString() : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-percentage me-2"></i>P-Margin:</strong> {company.pmargin || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-minus me-2"></i>Reissue Date From:</strong> {company.rissuDtFm ? new Date(company.rissuDtFm).toLocaleDateString() : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-plus me-2"></i>Reissue Date To:</strong> {company.rissuDtTo ? new Date(company.rissuDtTo).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-star me-2"></i>Premium:</strong> {company.premium || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-flag me-2"></i>C-Flag:</strong> {company.cflag || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-chart-bar me-2"></i>Market Float:</strong> {company.marFloat || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-day me-2"></i>Month To:</strong> {company.monTo || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-dollar-sign me-2"></i>S-Base Rate:</strong> {company.sbaseRt || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-alt me-2"></i>Float Date From:</strong> {company.flotDtFm ? new Date(company.flotDtFm).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-alt me-2"></i>Float Date To:</strong> {company.flotDtTo ? new Date(company.flotDtTo).toLocaleDateString() : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-check me-2"></i>Book Close From Date:</strong> {company.bokClFdt ? new Date(company.bokClFdt).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-calendar-times me-2"></i>Book Close To Date:</strong> {company.bokClTdt ? new Date(company.bokClTdt).toLocaleDateString() : 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-sync-alt me-2"></i>Rate Update Date:</strong> {company.rtUpdDt ? new Date(company.rtUpdDt).toLocaleDateString() : 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-flag me-2"></i>Flag:</strong> {company.flag || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-building me-2"></i>NS ICB:</strong> {company.nsIcb || 'N/A'}
                </div>
              </div>
              <div className="row mb-3">
                <div className="col-md-6">
                  <strong><i className="fas fa-cube me-2"></i>NS Unit:</strong> {company.nsUnit || 'N/A'}
                </div>
                <div className="col-md-6">
                  <strong><i className="fas fa-handshake me-2"></i>NS Mutual:</strong> {company.nsMutual || 'N/A'}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CompanyProfile;
