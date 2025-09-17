import React, { useState, useEffect } from 'react';
import { getCompanies } from '../../api';
import { Link } from 'react-router-dom'; // Import Link

const CompanyList = () => {
  const [companies, setCompanies] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [sortBy, setSortBy] = useState('compNm');
  const [sortDirection, setSortDirection] = useState('asc');
  const [search, setSearch] = useState('');

  useEffect(() => {
    const fetchCompanies = async () => {
      try {
        setLoading(true);
        const response = await getCompanies(search, pageNumber, pageSize, sortBy, sortDirection);
        setCompanies(response.data.companies);
        setTotalCount(response.data.totalCount);
        setError(null);
      } catch (err) {
        setError('Failed to fetch companies.');
        console.error(err);
      }
      setLoading(false);
    };

    const timer = setTimeout(() => {
        fetchCompanies();
    }, 500); // Debounce search

    return () => clearTimeout(timer);

  }, [pageNumber, pageSize, sortBy, sortDirection, search]);

  const handleSort = (column) => {
    if (sortBy === column) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortDirection('asc');
    }
    setPageNumber(1); // Reset to first page on sort
  };

  const handleSearchChange = (e) => {
    setSearch(e.target.value);
    setPageNumber(1); // Reset to first page on search
  }

  const totalPages = Math.ceil(totalCount / pageSize);

  const getSortIndicator = (key) => {
    if (sortBy === key) {
      return sortDirection === 'asc' ? ' ▲' : ' ▼';
    }
    return null;
  };

  return (
    <div className="container-fluid mt-4">
      <h2>Company List</h2>
      <div className="mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Search by Name, Symbol, Category..."
          value={search}
          onChange={handleSearchChange}
        />
      </div>

      {loading && <p>Loading...</p>}
      {error && <div className="alert alert-danger">{error}</div>}

      {!loading && !error && (
        <>
          <div className="table-responsive">
            <table className="table table-striped table-hover">
              <thead className="table-dark">
                <tr>
                  <th onClick={() => handleSort('compNm')}>Name{getSortIndicator('compNm')}</th>
                  <th onClick={() => handleSort('instrCd')}>Symbol{getSortIndicator('instrCd')}</th>
                  <th onClick={() => handleSort('category')}>Category{getSortIndicator('category')}</th>
                  <th onClick={() => handleSort('sectorName')}>Sector{getSortIndicator('sectorName')}</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {companies.map((company) => (
                  <tr key={company.id}>
                    <td>
                      <Link to={`/company-profile/${company.compCd}`}>{company.compNm}</Link>
                    </td>
                    <td>{company.instrCd}</td>
                    <td>{company.category}</td>
                    <td>{company.sectorName}</td>
                    <td>
                      <Link to={`/company-profile/${company.compCd}`} className="btn btn-primary btn-sm">Details</Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="d-flex justify-content-between align-items-center">
            <button 
              className="btn btn-secondary" 
              onClick={() => setPageNumber(p => Math.max(p - 1, 1))}
              disabled={pageNumber === 1}
            >
              Previous
            </button>
            <span>Page {pageNumber} of {totalPages}</span>
            <button 
              className="btn btn-secondary" 
              onClick={() => setPageNumber(p => Math.min(p + 1, totalPages))}
              disabled={pageNumber === totalPages}
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default CompanyList;