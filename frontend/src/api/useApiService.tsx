import { useNavigate } from 'react-router-dom';
import { ApiService } from './ApiService';

export const useApiService = () => {
    const navigate = useNavigate();
    
    const apiService = new ApiService(
        "http://localhost:5209/api/",
        () => navigate('/login'),
        () => {}
    );
    
    return apiService;
};