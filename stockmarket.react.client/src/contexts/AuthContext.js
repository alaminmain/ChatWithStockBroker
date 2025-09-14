import React, { createContext, useState, useContext, useMemo } from 'react';
import authService from '../services/authService';

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(authService.getCurrentUser());

  const login = (email, password) => {
    return authService.login(email, password).then(user => {
      console.log('AuthContext: setting user:', user);
      setCurrentUser(user);
      return user;
    });
  };

  const logout = () => {
    authService.logout();
    setCurrentUser(null);
  };

  const value = useMemo(() => ({
    currentUser,
    login,
    logout,
  }), [currentUser]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
