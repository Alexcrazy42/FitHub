import React from "react";
import { Card, Statistic, Row, Col, Button } from "antd";
import {
  TeamOutlined,
  EnvironmentOutlined,
  ToolOutlined,
  ThunderboltOutlined,
  ReloadOutlined,
} from "@ant-design/icons";

const AdminDashboard: React.FC = () => {
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

        <Button
          type="primary"
          icon={<ReloadOutlined />}
        >
          Обновить данные
        </Button>
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
              title="Зоны"
              value={24}
              prefix={<ThunderboltOutlined className="text-orange-500" />}
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

      {/* Нижний блок — краткое описание */}
      <Card className="rounded-2xl shadow-sm">
        <h2 className="text-lg font-semibold text-gray-700 mb-2">
          Что можно делать здесь:
        </h2>
        <ul className="list-disc pl-6 text-gray-600 space-y-1">
          <li>Управлять списком спортзалов, зон и тренажёров</li>
          <li>Редактировать контент и изображения залов</li>
          <li>Следить за активностью пользователей</li>
          <li>Добавлять и обновлять данные через панель управления</li>
        </ul>
      </Card>
    </div>
  );
};

export default AdminDashboard;
