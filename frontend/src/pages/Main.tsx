import React from "react";
import { Button, Typography, Card, Row, Col } from "antd";
import { useNavigate } from "react-router-dom";
import { UserOutlined, TeamOutlined, ScheduleOutlined, CreditCardOutlined } from '@ant-design/icons';

const { Title, Paragraph } = Typography;

export const Main: React.FC = () => {
  const navigate = useNavigate();

  const goToLogin = () => {
    navigate("/login");
  };

  return (
    <div className="min-h-screen bg-gray-100 flex flex-col items-center py-16 px-4">
      <div className="text-center mb-12">
        <Title level={1} className="text-gray-800">
          Добро пожаловать в FitHub
        </Title>
        <Paragraph className="text-gray-600 max-w-xl mx-auto">
          Управляйте своим спортзалом легко и удобно: следите за тренировками, тренерами и абонементами.
        </Paragraph>
        <Button type="primary" size="large" onClick={goToLogin}>
          Войти в аккаунт
        </Button>
      </div>

      <Row gutter={[24, 24]} className="max-w-5xl w-full">
        <Col xs={24} sm={12} md={6}>
          <Card className="text-center hover:shadow-xl transition-shadow duration-300">
            <UserOutlined style={{ fontSize: 40, color: '#1890ff' }} />
            <Title level={4} className="mt-4">Клиенты</Title>
            <Paragraph className="text-gray-500">Следите за посещениями и абонементами клиентов.</Paragraph>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card className="text-center hover:shadow-xl transition-shadow duration-300">
            <TeamOutlined style={{ fontSize: 40, color: '#52c41a' }} />
            <Title level={4} className="mt-4">Тренеры</Title>
            <Paragraph className="text-gray-500">Управляйте тренерским составом и их расписанием.</Paragraph>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card className="text-center hover:shadow-xl transition-shadow duration-300">
            <ScheduleOutlined style={{ fontSize: 40, color: '#faad14' }} />
            <Title level={4} className="mt-4">Тренировки</Title>
            <Paragraph className="text-gray-500">Создавайте групповые и индивидуальные тренировки.</Paragraph>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card className="text-center hover:shadow-xl transition-shadow duration-300">
            <CreditCardOutlined style={{ fontSize: 40, color: '#eb2f96' }} />
            <Title level={4} className="mt-4">Абонементы</Title>
            <Paragraph className="text-gray-500">Следите за подписками и платежами клиентов.</Paragraph>
          </Card>
        </Col>
      </Row>
    </div>
  );
};
