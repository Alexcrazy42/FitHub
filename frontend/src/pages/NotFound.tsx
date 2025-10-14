import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { Result, Button, Space, Card } from 'antd';
import { HomeOutlined, ArrowLeftOutlined, CustomerServiceOutlined } from '@ant-design/icons';
import { roleRoutes } from '../types/auth';

const NotFound: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const goHome = () => {
    if (user) {
      navigate(`${roleRoutes[user.currentRole]}`);
    } else {
      navigate('/login');
    }
  };

  const goBack = () => {
    navigate(-1);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 to-blue-50 p-4">
      <Card className="max-w-2xl w-full shadow-lg border-0">
        <Result
          status="404"
          title="404"
          subTitle={
            <div className="text-center">
              <div className="text-lg text-gray-600 mb-2">
                Извините, страница не найдена
              </div>
              <div className="text-gray-500 text-base">
                Запрашиваемая страница не существует или была перемещена.
              </div>
            </div>
          }
          extra={
            <Space direction="vertical" size="middle" className="w-full">
              <div className="flex flex-col sm:flex-row gap-3 justify-center">
                <Button
                  type="primary"
                  size="large"
                  icon={<HomeOutlined />}
                  onClick={goHome}
                  className="min-w-40 h-12"
                >
                  На главную
                </Button>
                <Button
                  size="large"
                  icon={<ArrowLeftOutlined />}
                  onClick={goBack}
                  className="min-w-40 h-12"
                >
                  Назад
                </Button>
              </div>
              
              <div className="border-t pt-4 mt-4">
                <div className="text-gray-500 text-sm mb-3 text-center">
                  Нужна помощь?
                </div>
                <div className="flex justify-center gap-4">
                  <Button 
                    type="link" 
                    icon={<CustomerServiceOutlined />}
                    onClick={() => navigate('/support')}
                  >
                    Поддержка
                  </Button>
                  <Button 
                    type="link"
                    onClick={() => navigate('/help')}
                  >
                    База знаний
                  </Button>
                </div>
              </div>
            </Space>
          }
        />
        
        {/* Дополнительная техническая информация */}
        <div className="mt-6 p-3 bg-gray-50 rounded-lg">
          <div className="text-xs text-gray-500 spacwe-y-1">
            <div>URL: <code className="ml-1 text-orange-600">{window.location.href}</code></div>
            <div>Время: <span className="ml-1">{new Date().toLocaleString('ru-RU')}</span></div>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default NotFound;