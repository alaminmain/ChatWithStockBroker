export const formatCurrency = (value, currency = 'BDT', locale = 'en-BD') => {
    if (value === null || value === undefined) {
      return 'N/A';
    }
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency: currency,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  };
  
  export const formatDate = (dateString, locale = 'en-BD') => {
    if (!dateString) {
      return 'N/A';
    }
    return new Date(dateString).toLocaleDateString(locale);
  };
  