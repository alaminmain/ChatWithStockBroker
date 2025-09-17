import React, { useState, useEffect, useMemo } from 'react';
import { getLatestStockPrices, getCompanyDetails } from '../../api';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import './TodaysPrice.css';

const TodaysPrice = () => {
  const [prices, setPrices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortConfig, setSortConfig] = useState({ key: 'instrCd', direction: 'ascending' });

  // Modal state
  const [showModal, setShowModal] = useState(false);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [modalLoading, setModalLoading] = useState(false);


  useEffect(() => {
    const fetchPrices = async () => {
      try {
        setLoading(true);
        const response = await getLatestStockPrices();
        setPrices(response.data);
        setError(null);
      } catch (err) {
        setError('Could not fetch latest prices.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchPrices();
  }, []);

  const handleSymbolClick = async (compCd) => {
    if (!compCd) return;
    setShowModal(true);
    setModalLoading(true);
    try {
      const response = await getCompanyDetails(compCd);
      setSelectedCompany(response.data);
    } catch (err) {
      console.error("Failed to fetch company details", err);
      // Optionally set an error state for the modal
    } finally {
      setModalLoading(false);
    }
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedCompany(null);
  }

  const sortedAndFilteredPrices = useMemo(() => {
    let sortableItems = [...prices];

    if (sortConfig.key !== null) {
      sortableItems.sort((a, b) => {
        const valA = a[sortConfig.key] || 0; // Handle null/undefined values
        const valB = b[sortConfig.key] || 0;
        if (valA < valB) {
          return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (valA > valB) {
          return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
      });
    }

    if (searchTerm) {
      return sortableItems.filter(price =>
        (price.instrCd && price.instrCd.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (price.category && price.category.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (price.sectorName && price.sectorName.toLowerCase().includes(searchTerm.toLowerCase()))
      );
    }

    return sortableItems;
  }, [prices, searchTerm, sortConfig]);

  const requestSort = (key) => {
    let direction = 'ascending';
    if (sortConfig.key === key && sortConfig.direction === 'ascending') {
      direction = 'descending';
    }
    setSortConfig({ key, direction });
  };

  const getSortIndicator = (key) => {
    if (sortConfig.key === key) {
      return sortConfig.direction === 'ascending' ? ' ▲' : ' ▼';
    }
    return null;
  };

  const getStatusTag = (change) => {
    if (change > 0) return <span className="badge bg-success">Gainer</span>;
    if (change < 0) return <span className="badge bg-danger">Loser</span>;
    return <span className="badge bg-secondary">Unchanged</span>;
  };

  if (loading) return <p>Loading today's prices...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <>
      <div className="container-fluid mt-4">
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h2>Today's Market Prices</h2>
          <input
            type="text"
            placeholder="Search by Symbol, Category, or Sector..."
            className="form-control"
            style={{ maxWidth: '400px' }}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <div className="table-responsive">
          <table className="table table-striped table-hover table-sm todays-price-table">
            <thead className="table-dark">
              <tr>
                <th onClick={() => requestSort('instrCd')}>Symbol{getSortIndicator('instrCd')}</th>
                <th onClick={() => requestSort('category')}>Category{getSortIndicator('category')}</th>
                <th onClick={() => requestSort('sectorName')}>Sector{getSortIndicator('sectorName')}</th>
                <th onClick={() => requestSort('ltp')}>LTP{getSortIndicator('ltp')}</th>
                <th onClick={() => requestSort('open')}>Open{getSortIndicator('open')}</th>
                <th onClick={() => requestSort('high')}>High{getSortIndicator('high')}</th>
                <th onClick={() => requestSort('low')}>Low{getSortIndicator('low')}</th>
                <th onClick={() => requestSort('close')}>Close{getSortIndicator('close')}</th>
                <th onClick={() => requestSort('chg')}>Change{getSortIndicator('chg')}</th>
                <th onClick={() => requestSort('changePercent')}>% Change{getSortIndicator('changePercent')}</th>
                <th onClick={() => requestSort('value')}>Value (MN){getSortIndicator('value')}</th>
                <th onClick={() => requestSort('volume')}>Volume{getSortIndicator('volume')}</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {sortedAndFilteredPrices.map((price, index) => (
                <tr key={index}>
                  <td className="symbol-link" onClick={() => handleSymbolClick(price.compCd)}>{price.instrCd}</td>
                  <td>{price.category}</td>
                  <td>{price.sectorName}</td>
                  <td>{price.ltp?.toFixed(2)}</td>
                  <td>{price.open?.toFixed(2)}</td>
                  <td>{price.high?.toFixed(2)}</td>
                  <td>{price.low?.toFixed(2)}</td>
                  <td>{price.close?.toFixed(2)}</td>
                  <td className={price.chg > 0 ? 'text-success' : price.chg < 0 ? 'text-danger' : ''}>
                    {price.chg?.toFixed(2)}
                  </td>
                  <td className={price.chg > 0 ? 'text-success' : price.chg < 0 ? 'text-danger' : ''}>
                    {price.changePercent?.toFixed(2)}%
                  </td>
                  <td>{(price.value / 1000000)?.toFixed(2)}</td>
                  <td>{price.volume?.toLocaleString()}</td>
                  <td>{getStatusTag(price.chg)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal show={showModal} onHide={handleCloseModal} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>{selectedCompany ? selectedCompany.compNm : 'Loading...'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {modalLoading ? (
            <p>Loading details...</p>
          ) : selectedCompany ? (
            <div>
              <p><strong>Instrument Code:</strong> {selectedCompany.instrCd}</p>
              <p><strong>Category:</strong> {selectedCompany.category}</p>
              <p><strong>Sector:</strong> {selectedCompany.sectorName}</p>
              <p><strong>ISIN:</strong> {selectedCompany.isinCd}</p>
              <p><strong>Registered Office:</strong> {selectedCompany.regOff}</p>
              <p><strong>Email:</strong> {selectedCompany.eMail}</p>
              <p><strong>Phone:</strong> {selectedCompany.tel}</p>
              <p><strong>Website:</strong> <a href={selectedCompany.website} target="_blank" rel="noopener noreferrer">{selectedCompany.website}</a></p>
              <hr />
              <h5>Capital Structure</h5>
              <p><strong>Authorized Capital:</strong> {selectedCompany.athoCap?.toLocaleString()}</p>
              <p><strong>Paid-up Capital:</strong> {selectedCompany.paidCap?.toLocaleString()}</p>
              <p><strong>Face Value:</strong> {selectedCompany.fcVal}</p>
              <p><strong>Total Securities:</strong> {selectedCompany.noShrs?.toLocaleString()}</p>
            </div>
          ) : (
            <p>Could not load company details.</p>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCloseModal}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default TodaysPrice;
