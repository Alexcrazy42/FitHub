import { useNavigate } from 'react-router-dom';
import { ApiService } from './ApiService';

export const API_URL = import.meta.env?.VITE_API_URL
  ? `${import.meta.env?.VITE_API_URL}/api`
  : (() => { throw new Error("VITE_API_URL не задан"); })();

export const API_URL_CLEAN = import.meta.env?.VITE_API_URL
  ? `${import.meta.env?.VITE_API_URL}`
  : (() => { throw new Error("VITE_API_URL не задан"); })();

export const useApiService = () => {
    const navigate = useNavigate();
    
    const apiService = new ApiService(
        API_URL,
        () => navigate('/login'),
        () => navigate('/access-denied')
    );
    
    return apiService;
};

export const useApiServiceWithoutNavigate = () => {
    const apiService = new ApiService(
      API_URL,
          () => {},
          () => {}
    );

    return apiService;
}