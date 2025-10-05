// src/pages/Auth/Login.tsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/useAuth';
import { User } from '../../types/auth';
import { useTheme } from '../../context/useTheme';

const Login: React.FC = () => {
  const [role, setRole] = useState<'admin' | 'user'>('user');
  const [name, setName] = useState('John Doe');
  const navigate = useNavigate();
  const { login } = useAuth();
  const { theme } = useTheme();
  const isDark = theme === 'dark';

  const handleLogin = () => {
    const mockUser: User = {
      id: Math.random().toString(36).substring(2, 9),
      name,
      role,
    };
    login(mockUser);
    navigate(`/${role}`);
  };

  return (
    <div className={`min-h-screen flex items-center justify-center p-4 ${isDark ? 'bg-gray-900' : 'bg-gray-100'}`}>
      <div
        className={`w-full max-w-md rounded-xl p-8 space-y-6 shadow-lg ${
          isDark ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'
        }`}
      >
        <div className="text-center">
          <h2 className="text-2xl font-bold">Вход в систему</h2>
          <p className={`mt-2 ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
            Выберите роль и войдите
          </p>
        </div>

        <div>
          <label className={`block text-sm font-medium mb-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>
            Имя
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className={`w-full px-4 py-2.5 rounded-lg outline-none transition ${
              isDark
                ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500 focus:border-blue-500'
                : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500 focus:border-blue-500'
            }`}
            placeholder="Введите ваше имя"
          />
        </div>

        <div>
          <label className={`block text-sm font-medium mb-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>
            Роль
          </label>
          <div className="flex space-x-4">
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="role"
                checked={role === 'user'}
                onChange={() => setRole('user')}
                className="h-4 w-4 focus:ring-blue-500"
                style={{
                  accentColor: '#3b82f6'
                }}
              />
              <span className={`ml-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>Пользователь</span>
            </label>
            <label className="inline-flex items-center">
              <input
                type="radio"
                name="role"
                checked={role === 'admin'}
                onChange={() => setRole('admin')}
                className="h-4 w-4 focus:ring-blue-500"
                style={{
                  accentColor: '#3b82f6',
                }}
              />
              <span className={`ml-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>Администратор</span>
            </label>
          </div>
        </div>

        <button
          onClick={handleLogin}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition duration-200 shadow-sm"
        >
          Войти
        </button>
      </div>
    </div>
  );
};

export default Login;