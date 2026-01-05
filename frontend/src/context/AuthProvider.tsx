import React, { useState, ReactNode } from 'react';
import { IGymAdminResponse, User } from '../types/auth';
import { AuthContext } from './AuthContext';
import { useApiServiceWithoutNavigate } from '../api/useApiService';
import { IGymResponse } from '../types/gyms';

export interface AuthContextType {
  user: User | null;
  gymAdmin: IGymAdminResponse | null;
  currentGym: IGymResponse | null;
  login: (user: User) => void;
  logout: () => void;
}

const userLocalStorage = 'user';
const gymAdminLocalStorage = 'gymAdmin';
const currentGymLocalStorage = 'currentGym';

// TODO: перевести на redux
export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const userJson = localStorage.getItem(userLocalStorage);
  const tempUser: User | null = userJson ? JSON.parse(userJson) : null;
  const [user, setUser] = useState<User | null>(tempUser);


  const gymAdminJson = localStorage.getItem(gymAdminLocalStorage);
  const tempGymAdmin: IGymAdminResponse | null = gymAdminJson ? JSON.parse(gymAdminJson) : null;
  const [gymAdmin, setGymAdmin] = useState<IGymAdminResponse | null>(tempGymAdmin);

  const gymJson = localStorage.getItem(currentGymLocalStorage);
  const tempCurrentGym: IGymResponse | null = gymJson ? JSON.parse(gymJson) : null;
  const [currentGym, setCurrencyGym] = useState<IGymResponse | null>(tempCurrentGym);

  const apiService = useApiServiceWithoutNavigate();

  const login = async (userData: User) => {
    localStorage.setItem(userLocalStorage, JSON.stringify(userData));
    setUser(userData);

    if (userData.currentRole == 'GymAdmin') {
      const response = await apiService.get<IGymAdminResponse>('v1/gym-admins/me');
      if (response.success) {
        const gymAdmin = response.data || null;
        setGymAdmin(gymAdmin);
        localStorage.setItem(gymAdminLocalStorage, JSON.stringify(gymAdmin));
        const gym = gymAdmin?.gyms[0] || null;
        setCurrencyGym(gym);
        localStorage.setItem(currentGymLocalStorage, JSON.stringify(gym));
      }
    }
  };

  const logout = () => {
    localStorage.removeItem(userLocalStorage);
    localStorage.removeItem(gymAdminLocalStorage);
    localStorage.removeItem(currentGymLocalStorage);
    setUser(null);
    setGymAdmin(null);
    setCurrencyGym(null);
  };

  return (
    <AuthContext.Provider value={{ user, gymAdmin, currentGym, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

