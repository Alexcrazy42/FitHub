import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useApiService } from '../../api/useApiService';
import { useAuth } from '../../context/useAuth';
import { useTheme } from '../../context/useTheme';
import { LoginResponse, roleMapping, roleRoutes, User, UserRole } from '../../types/auth';
import { toast } from 'react-toastify';
import { Alert, Button, Space } from "antd";

interface LoginRequest {
  username: string;
  password: string;
}

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [availableRoles, setAvailableRoles] = useState<{ role: UserRole; displayName: string; }[]>([]);
  const [selectedRole, setSelectedRole] = useState<UserRole | null>(null);
  const [showRoleSelection, setShowRoleSelection] = useState(false);
  const [loading, setLoading] = useState(true);
  const [firstLogin, setFirstLogin] = useState(true);
  
  const navigate = useNavigate();
  const { login, user } = useAuth();
  const { theme } = useTheme();
  const apiService = useApiService();
  
  const isDark = theme === 'dark';

  const forgot = async () => {
    try {
      if(username != '') {
        const response = await apiService.post(`v1/auth/init-reset-password?email=${username}`);
        if(response.success) {
          toast.success("Ожидайте сообщения на почту!");
        } else {
          toast.error(response.error?.detail);
        }
      } else {
        setError("Заполнить email!");
      }
      
    } catch {
      toast.error("Ошибка!");
    }
  }

  useEffect(() => {
    if(user) {
      setLoading(true);
      setFirstLogin(false);
      setLoading(false);
    }
  }, []);


  const handleLogin = async () => {
    if (showRoleSelection) {
      if (!selectedRole) {
        setError('Пожалуйста, выберите роль для входа');
        return;
      }

      const mapping = roleRoutes[selectedRole];
      const user: User = {
        id: username,
        email: username,
        roles: [selectedRole],
        currentRole: selectedRole
      };
      
      login(user);
      navigate(`${mapping}/home`);
      return;
    }


    if (!username.trim() || !password.trim()) {
      setError('Пожалуйста, заполните все поля');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const loginData: LoginRequest = {
        username: username.trim(),
        password: password.trim()
      };

      const result = await apiService.post<LoginResponse>('/v1/auth/login', loginData);

      if (result.success && result.data) {
        const userData = result.data;
        const userRoles = userData.roleNames;

        if (userRoles.length === 0) {
          setError('У пользователя нет доступных ролей');
          return;
        }

        const rolesForSelection = userRoles.map(role => ({
          role,
          displayName: roleMapping[role] || role
        }));

        if (userRoles.length === 1) {
          const mapping = userRoles[0];
          const user: User = {
            id: userData.userId,
            email: userData.email,
            roles: userRoles,
            currentRole: userRoles[0]
          };
          
          login(user);
          const route = roleRoutes[mapping];
          navigate(route);
        } else {
          // Если несколько ролей - показываем выбор
          setAvailableRoles(rolesForSelection);
          setSelectedRole(userRoles[0]); // выбираем первую роль по умолчанию
          setShowRoleSelection(true);
        }
        
      } else {
        if (result.error) {
          setError(result.error.detail || 'Ошибка при входе в систему');
        } else {
          setError('Произошла неизвестная ошибка');
        }
      }
    } catch (error) {
      setError('Ошибка соединения с сервером');
    } finally {
      setIsLoading(false);
    }
  };

  const handleBackToLogin = () => {
    setShowRoleSelection(false);
    setAvailableRoles([]);
    setSelectedRole(null);
    setError('');
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleLogin();
    }
  };

  if(loading) {
    return <div>Загрузка...</div>
  }

  if (!firstLogin) {
    return (
      <div className="flex justify-center items-center min-h-screen bg-gray-100 px-4">
        <div className="max-w-md w-full">
          <Alert
            message="Вы уже зарегистрированы"
            description={
              <Space direction="vertical" size="middle" className="w-full">
                <span>
                  Вы уже вошли в аккаунт: <strong>{user?.email}</strong>
                </span>
                <span>Хотите выполнить повторный вход?</span>
                <Space>
                  <Button type="primary" onClick={() => setFirstLogin(true)}>
                    Да
                  </Button>
                  <Button
                    onClick={() => {
                      if(user?.currentRole) {
                        const route = roleRoutes[user?.currentRole];
                        navigate(route);
                      }
                      
                    }}
                  >
                    Нет, перейти в личный кабинет
                  </Button>
                </Space>
              </Space>
            }
            type="info"
            showIcon
          />
        </div>
      </div>
    );
  }

  if (showRoleSelection) {
    return (
      <div className={`min-h-screen flex items-center justify-center p-4 ${isDark ? 'bg-gray-900' : 'bg-gray-100'}`}>
        <div
          className={`w-full max-w-md rounded-xl p-8 space-y-6 shadow-lg ${
            isDark ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'
          }`}
        >
          <div className="text-center">
            <h2 className="text-2xl font-bold">Выберите роль</h2>
            <p className={`mt-2 ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
              Вы вошли как <strong>{username}</strong>
            </p>
            <p className={`text-sm ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
              Выберите под какой ролью войти в систему
            </p>
          </div>

          {error && (
            <div className={`p-3 rounded-lg ${
              isDark ? 'bg-red-900 text-red-200' : 'bg-red-100 text-red-700'
            }`}>
              {error}
            </div>
          )}

          <div className="space-y-3">
            {availableRoles.map((roleInfo) => (
              <label 
                key={roleInfo.role}
                className={`flex items-center p-4 rounded-lg border cursor-pointer transition ${
                  selectedRole === roleInfo.role
                    ? isDark 
                      ? 'bg-blue-900 border-blue-600' 
                      : 'bg-blue-50 border-blue-500'
                    : isDark 
                      ? 'bg-gray-700 border-gray-600 hover:bg-gray-600' 
                      : 'bg-white border-gray-300 hover:bg-gray-50'
                }`}
              >
                <input
                  type="radio"
                  name="role"
                  value={roleInfo.role}
                  checked={selectedRole === roleInfo.role}
                  onChange={() => setSelectedRole(roleInfo.role)}
                  className="h-4 w-4 focus:ring-blue-500"
                  style={{ accentColor: '#3b82f6' }}
                />
                <span className="ml-3 font-medium">{roleInfo.displayName}</span>
              </label>
            ))}
          </div>

          <div className="flex space-x-3">
            <button
              onClick={handleBackToLogin}
              className="flex-1 bg-gray-500 hover:bg-gray-600 text-white font-medium py-2.5 rounded-lg transition duration-200"
            >
              Назад
            </button>
            <button
              onClick={handleLogin}
              className="flex-1 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition duration-200"
            >
              Продолжить
            </button>
          </div>
        </div>
      </div>
    );
  }

  // Стандартная форма логина
  return (
    <div className={`min-h-screen flex items-center justify-center p-4 ${isDark ? 'bg-gray-900' : 'bg-gray-100'}`}>
      <div
        className={`w-full max-w-md rounded-2xl p-8 shadow-xl space-y-6 ${
          isDark ? 'bg-gray-800 text-gray-100' : 'bg-white text-gray-800'
        }`}
      >
        {/* Header */}
        <div className="text-center">
          <h2 className="text-3xl font-bold">Вход в систему</h2>
          <p className={`mt-2 ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
            Авторизуйтесь, чтобы продолжить
          </p>
        </div>

        {/* Error message */}
        {error && (
          <div
            className={`p-3 rounded-lg text-sm ${
              isDark ? 'bg-red-900 text-red-200' : 'bg-red-100 text-red-700'
            }`}
          >
            {error}
          </div>
        )}

        {/* Email */}
        <div>
          <label className={`block text-sm font-medium mb-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>
            Email
          </label>
          <input
            type="email"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Введите ваш email"
            disabled={isLoading}
            className={`w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 ${
              isDark
                ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500 focus:border-blue-500'
                : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500 focus:border-blue-500'
            }`}
          />
        </div>

        {/* Password */}
        <div>
          <label className={`block text-sm font-medium mb-2 ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>
            Пароль
          </label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Введите ваш пароль"
            disabled={isLoading}
            className={`w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 ${
              isDark
                ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500 focus:border-blue-500'
                : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500 focus:border-blue-500'
            }`}
          />
        </div>

        {/* Forgot password */}
        <div className="text-right -mt-3">
          <button
            onClick={forgot}
            className={`text-sm underline transition ${
              isDark
                ? 'text-gray-300 hover:text-white'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            Забыли пароль?
          </button>
        </div>

        {/* Buttons */}
        <div className="flex gap-3 pt-2">
          <button
            onClick={handleLogin}
            disabled={isLoading}
            className={`flex-1 font-semibold py-2.5 rounded-lg transition duration-200 shadow ${
              isLoading
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700 text-white"
            }`}
          >
            {isLoading ? "Вход..." : "Войти"}
          </button>

          <button
            disabled={isLoading}
            className={`flex-1 font-semibold py-2.5 rounded-lg transition duration-200 shadow ${
              isLoading
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-green-600 hover:bg-green-700 text-white"
            }`}
            onClick={() => { navigate("/register")} }
          >
            Регистрация
          </button>
        </div>
      </div>
    </div>
  );
};

export default Login;