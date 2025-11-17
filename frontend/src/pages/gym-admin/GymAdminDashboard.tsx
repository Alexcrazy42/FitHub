import React from "react";
import { Card, Statistic, Row, Col } from "antd";
import {
  TeamOutlined,
  EnvironmentOutlined,
  ToolOutlined,
} from "@ant-design/icons";

const GymAdminDashboard: React.FC = () => {
  return (
    <div className="p-6 space-y-6">
      {/* Заголовок */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-800 mb-1">
            Панель администратора
          </h1>
          <p className="text-gray-500">Добро пожаловать в систему управления фитнес-клубом</p>
        </div>
      </div>

      {/* Карточки статистики */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false} className="shadow-md rounded-2xl">
            <Statistic
              title="Залы"
              value={8}
              prefix={<EnvironmentOutlined className="text-blue-500" />}
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false} className="shadow-md rounded-2xl">
            <Statistic
              title="Тренажёры"
              value={132}
              prefix={<ToolOutlined className="text-green-500" />}
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card bordered={false} className="shadow-md rounded-2xl">
            <Statistic
              title="Пользователи"
              value={56}
              prefix={<TeamOutlined className="text-purple-500" />}
            />
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default GymAdminDashboard;
