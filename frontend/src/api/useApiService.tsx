import { useNavigate } from 'react-router-dom';
import { ApiService } from './ApiService';

export const API_URL = import.meta.env.VITE_API_URL + "/api" ?? "http://localhost:5209/api";

export const useApiService = () => {
    const navigate = useNavigate();
    
    const apiService = new ApiService(
        API_URL,
        () => navigate('/login'),
        () => {}
    );
    
    return apiService;
};