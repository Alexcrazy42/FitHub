import React from 'react';
import { Result, Button } from 'antd';
import { LockOutlined, HomeOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { roleRoutes } from '../types/auth';

const AccessDenied: React.FC = () => {
const { user } = useAuth();
  const navigate = useNavigate();

  const handleGoHome = () => {
      if (user) {
        navigate(`${roleRoutes[user.currentRole]}`);
      } else {
        navigate('/login');
      }
    };

  const handleGoBack = () => {
    navigate(-1);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
      <Result
        icon={<LockOutlined className="text-red-500" />}
        status="403"
        title="Доступ запрещен"
        subTitle="Извините, у вас нет прав для просмотра этой страницы."
        extra={[
          <Button
            key="home"
            type="primary"
            icon={<HomeOutlined />}
            onClick={handleGoHome}
            className="bg-blue-600 hover:bg-blue-700 border-blue-600"
          >
            На главную
          </Button>,
          <Button
            key="back"
            icon={<HomeOutlined />}
            onClick={handleGoBack}
            className="border-gray-300 hover:border-gray-400"
          >
            Назад
          </Button>,
        ]}
        className="max-w-md w-full"
      />
    </div>
  );
};

export default AccessDenied;