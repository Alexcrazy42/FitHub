import React, { useState, ReactNode } from 'react';
import { User } from '../types/auth';
import { AuthContext } from './AuthContext';

export interface AuthContextType {
  user: User | null;
  login: (user: User) => void;
  logout: () => void;
}

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const userJson = localStorage.getItem('user');
  const tempUser: User | null = userJson ? JSON.parse(userJson) : null;
  const [user, setUser] = useState<User | null>(tempUser);

  const login = (userData: User) => {
    const userString = JSON.stringify(userData);
    localStorage.setItem('user', userString);
    setUser(userData);
  };

  const logout = () => {
    localStorage.removeItem('user');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

