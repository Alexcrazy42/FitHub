// src/pages/NotFound.tsx
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';

const NotFound: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const goHome = () => {
    if (user) {
      navigate(`/${user.role}`);
    } else {
      navigate('/login');
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100">
      <h1 className="text-6xl font-bold text-gray-800">404</h1>
      <p className="text-xl text-gray-600 mt-4">Страница не найдена</p>
      <button
        onClick={goHome}
        className="mt-6 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
      >
        Вернуться домой
      </button>
    </div>
  );
};

export default NotFound;