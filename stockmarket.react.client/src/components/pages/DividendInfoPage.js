import React, { useState, useEffect } from 'react';
import dividendInfoService from '../../services/dividendInfoService';
import { format } from 'date-fns';

const DividendInfoPage = () => {
  const [dividends, setDividends] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filterCompCd, setFilterCompCd] = useState('');
  const [filterDivType, setFilterDivType] = useState('');
  const [filterBokClFdt, setFilterBokClFdt] = useState('');
  const [editingDividend, setEditingDividend] = useState(null);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false); // State for modal visibility
  const [newDividend, setNewDividend] = useState({
    compCd: '',
    agmDt: '',
    fyear: '',
    cfyear: '',
    divType: '',
    rate: '',
    ratio1: '',
    ratio2: '',
    premium: '',
    paymentDt: '',
    bokClFdt: '',
    bokClTdt: '',
    opName: '',
    discount: '',
    remarks: '',
    bsCompCd: '',
  });

  const divTypeMapping = {
    R: 'Right Share',
    I: 'Interim Dividend',
    B: 'Bonus Share',
    C: 'Fractional Cash',
    F: 'Final Dividend',
  };

  const fetchDividends = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await dividendInfoService.getDividendInfo(
        filterCompCd || null,
        filterDivType || null,
        filterBokClFdt ? new Date(filterBokClFdt) : null
      );
      setDividends(response.data);
    } catch (err) {
      setError('Failed to fetch dividend info.');
      console.error('Error fetching dividend info:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDividends();
  }, []); // Fetch on initial load

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    if (name === 'filterCompCd') setFilterCompCd(value);
    else if (name === 'filterDivType') setFilterDivType(value);
    else if (name === 'filterBokClFdt') setFilterBokClFdt(value);
  };

  const handleFilterSubmit = (e) => {
    e.preventDefault();
    fetchDividends();
  };

  const handleNewDividendChange = (e) => {
    const { name, value } = e.target;
    setNewDividend((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleAddDividend = async (e) => {
    e.preventDefault();
    try {
      await dividendInfoService.createDividendInfo(newDividend);
      setNewDividend({
        compCd: '',
        agmDt: '',
        fyear: '',
        cfyear: '',
        divType: '',
        rate: '',
        ratio1: '',
        ratio2: '',
        premium: '',
        paymentDt: '',
        bokClFdt: '',
        bokClTdt: '',
        opName: '',
        discount: '',
        remarks: '',
        bsCompCd: '',
      });
      setIsAddModalOpen(false); // Close modal on success
      fetchDividends();
    } catch (err) {
      setError('Failed to add dividend info.');
      console.error('Error adding dividend info:', err);
    }
  };

  const handleEditClick = (dividend) => {
    setEditingDividend({ ...dividend });
  };

  const handleEditChange = (e) => {
    const { name, value } = e.target;
    setEditingDividend((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleUpdateDividend = async (e) => {
    e.preventDefault();
    try {
      await dividendInfoService.updateDividendInfo(editingDividend.id, editingDividend);
      setEditingDividend(null);
      fetchDividends();
    } catch (err) {
      setError('Failed to update dividend info.');
      console.error('Error updating dividend info:', err);
    }
  };

  const handleDeleteDividend = async (id) => {
    try {
      await dividendInfoService.deleteDividendInfo(id);
      fetchDividends();
    } catch (err) {
      setError('Failed to delete dividend info.');
      console.error('Error deleting dividend info:', err);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Dividend Information</h1>

      {/* Filter Section */}
      <form onSubmit={handleFilterSubmit} className="mb-4 p-4 border rounded shadow-sm">
        <h2 className="text-xl font-semibold mb-2">Filter Dividends</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label htmlFor="filterCompCd" className="block text-sm font-medium text-gray-700">Company Code:</label>
            <input
              type="text"
              id="filterCompCd"
              name="filterCompCd"
              value={filterCompCd}
              onChange={handleFilterChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
            />
          </div>
          <div>
            <label htmlFor="filterDivType" className="block text-sm font-medium text-gray-700">Dividend Type:</label>
            <select
              id="filterDivType"
              name="filterDivType"
              value={filterDivType}
              onChange={handleFilterChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
            >
              <option value="">All</option>
              {Object.entries(divTypeMapping).map(([key, value]) => (
                <option key={key} value={key}>{value}</option>
              ))}
            </select>
          </div>
          <div>
            <label htmlFor="filterBokClFdt" className="block text-sm font-medium text-gray-700">Book Closure From Date:</label>
            <input
              type="date"
              id="filterBokClFdt"
              name="filterBokClFdt"
              value={filterBokClFdt}
              onChange={handleFilterChange}
              className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
            />
          </div>
        </div>
        <button
          type="submit"
          className="mt-4 px-4 py-2 bg-blue-600 text-white font-semibold rounded-md shadow-sm hover:bg-blue-700"
        >
          Apply Filters
        </button>
      </form>

      {/* Add New Dividend Button */}
      <button
        onClick={() => setIsAddModalOpen(true)}
        className="mb-4 px-4 py-2 bg-blue-600 text-white font-semibold rounded-md shadow-sm hover:bg-blue-700"
      >
        Add New Dividend
      </button>

      {/* Add New Dividend Modal */}
      {isAddModalOpen && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-xl max-w-2xl w-full">
            <h2 className="text-2xl font-bold mb-4">Add New Dividend</h2>
            <form onSubmit={handleAddDividend} className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {Object.keys(newDividend).map((key) => {
                if (key === 'id') return null;

                const type = [
                  'agmDt', 'paymentDt', 'bokClFdt', 'bokClTdt'
                ].includes(key) ? 'date' : [
                  'rate', 'ratio1', 'ratio2', 'premium', 'discount', 'compCd', 'bsCompCd'
                ].includes(key) ? 'number' : 'text';

                const label = key.replace(/([A-Z])/g, ' $1').replace(/^./, (str) => str.toUpperCase());

                if (key === 'divType') {
                  return (
                    <div key={key}>
                      <label htmlFor={key} className="block text-sm font-medium text-gray-700">{label}:</label>
                      <select
                        id={key}
                        name={key}
                        value={newDividend[key]}
                        onChange={handleNewDividendChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
                      >
                        <option value="">Select Type</option>
                        {Object.entries(divTypeMapping).map(([typeKey, typeValue]) => (
                          <option key={typeKey} value={typeKey}>{typeValue}</option>
                        ))}
                      </select>
                    </div>
                  );
                }

                return (
                  <div key={key}>
                    <label htmlFor={key} className="block text-sm font-medium text-gray-700">{label}:</label>
                    <input
                      type={type}
                      id={key}
                      name={key}
                      value={newDividend[key]}
                      onChange={handleNewDividendChange}
                      className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
                      step={type === 'number' ? '0.01' : undefined}
                    />
                  </div>
                );
              })}
              <div className="md:col-span-3 flex justify-end space-x-2 mt-4">
                <button
                  type="button"
                  onClick={() => setIsAddModalOpen(false)}
                  className="px-4 py-2 bg-gray-300 text-gray-800 font-semibold rounded-md shadow-sm hover:bg-gray-400"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-green-600 text-white font-semibold rounded-md shadow-sm hover:bg-green-700"
                >
                  Add Dividend
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Dividends Table */}
      <div className="overflow-x-auto">
        <table className="min-w-full bg-white border border-gray-200">
          <thead>
            <tr>
              <th className="py-2 px-4 border-b">ID</th>
              <th className="py-2 px-4 border-b">Company Code</th>
              <th className="py-2 px-4 border-b">AGM Date</th>
              <th className="py-2 px-4 border-b">Fiscal Year</th>
              <th className="py-2 px-4 border-b">Current Fiscal Year</th>
              <th className="py-2 px-4 border-b">Dividend Type</th>
              <th className="py-2 px-4 border-b">Rate</th>
              <th className="py-2 px-4 border-b">Ratio 1</th>
              <th className="py-2 px-4 border-b">Ratio 2</th>
              <th className="py-2 px-4 border-b">Premium</th>
              <th className="py-2 px-4 border-b">Payment Date</th>
              <th className="py-2 px-4 border-b">Book Closure From</th>
              <th className="py-2 px-4 border-b">Book Closure To</th>
              <th className="py-2 px-4 border-b">Operator Name</th>
              <th className="py-2 px-4 border-b">Discount</th>
              <th className="py-2 px-4 border-b">Remarks</th>
              <th className="py-2 px-4 border-b">BS Company Code</th>
              <th className="py-2 px-4 border-b">Actions</th>
            </tr>
          </thead>
          <tbody>
            {dividends.map((dividend) => (
              <tr key={dividend.id} className="hover:bg-gray-50">
                <td className="py-2 px-4 border-b text-center">{dividend.id}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.compCd}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.agmDt ? format(new Date(dividend.agmDt), 'yyyy-MM-dd') : 'N/A'}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.fyear}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.cfyear}</td>
                <td className="py-2 px-4 border-b text-center">{divTypeMapping[dividend.divType] || dividend.divType}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.rate}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.ratio1}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.ratio2}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.premium}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.paymentDt ? format(new Date(dividend.paymentDt), 'yyyy-MM-dd') : 'N/A'}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.bokClFdt ? format(new Date(dividend.bokClFdt), 'yyyy-MM-dd') : 'N/A'}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.bokClTdt ? format(new Date(dividend.bokClTdt), 'yyyy-MM-dd') : 'N/A'}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.opName}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.discount}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.remarks}</td>
                <td className="py-2 px-4 border-b text-center">{dividend.bsCompCd}</td>
                <td className="py-2 px-4 border-b text-center">
                  <button
                    onClick={() => handleEditClick(dividend)}
                    className="bg-yellow-500 text-white px-3 py-1 rounded-md text-sm mr-2 hover:bg-yellow-600"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDeleteDividend(dividend.id)}
                    className="bg-red-500 text-white px-3 py-1 rounded-md text-sm hover:bg-red-600"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Edit Dividend Modal/Form */}
      {editingDividend && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex justify-center items-center">
          <div className="bg-white p-8 rounded-lg shadow-xl max-w-2xl w-full">
            <h2 className="text-2xl font-bold mb-4">Edit Dividend</h2>
            <form onSubmit={handleUpdateDividend} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {Object.keys(editingDividend).map((key) => {
                if (key === 'id') return null; // ID is not editable

                const type = [
                  'agmDt', 'paymentDt', 'bokClFdt', 'bokClTdt'
                ].includes(key) ? 'date' : [
                  'rate', 'ratio1', 'ratio2', 'premium', 'discount', 'compCd', 'bsCompCd'
                ].includes(key) ? 'number' : 'text';

                const label = key.replace(/([A-Z])/g, ' $1').replace(/^./, (str) => str.toUpperCase());

                if (key === 'divType') {
                  return (
                    <div key={key}>
                      <label htmlFor={key} className="block text-sm font-medium text-gray-700">{label}:</label>
                      <select
                        id={key}
                        name={key}
                        value={editingDividend[key]}
                        onChange={handleEditChange}
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
                      >
                        <option value="">Select Type</option>
                        {Object.entries(divTypeMapping).map(([typeKey, typeValue]) => (
                          <option key={typeKey} value={typeKey}>{typeValue}</option>
                        ))}
                      </select>
                    </div>
                  );
                }

                return (
                  <div key={key}>
                    <label htmlFor={key} className="block text-sm font-medium text-gray-700">{label}:</label>
                    <input
                      type={type}
                      id={key}
                      name={key}
                      value={editingDividend[key] ? (type === 'date' ? format(new Date(editingDividend[key]), 'yyyy-MM-dd') : editingDividend[key]) : ''}
                      onChange={handleEditChange}
                      className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm p-2"
                      step={type === 'number' ? '0.01' : undefined}
                    />
                  </div>
                );
              })}
              <div className="md:col-span-2 flex justify-end space-x-2 mt-4">
                <button
                  type="button"
                  onClick={() => setEditingDividend(null)}
                  className="px-4 py-2 bg-gray-300 text-gray-800 font-semibold rounded-md shadow-sm hover:bg-gray-400"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white font-semibold rounded-md shadow-sm hover:bg-blue-700"
                >
                  Update
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default DividendInfoPage;
