// pages/GymsPage.tsx
import React, { FC, useState } from "react";
import { Table, Button, Space, Typography, Card, Row, Col } from "antd";
import type { ColumnsType } from "antd/es/table";

const { Title } = Typography;

interface Gym {
  id: string;
  name: string;
  location: string;
  capacity: number;
}

const initialGyms: Gym[] = [
  { id: "1", name: "FitLife", location: "Центр города", capacity: 50 },
  { id: "2", name: "ProGym", location: "Северный район", capacity: 35 },
  { id: "3", name: "Yoga & More", location: "Южный район", capacity: 20 },
];

const GymsPage: FC = () => {
  const [gyms, setGyms] = useState<Gym[]>(initialGyms);

  const handleDelete = (id: string): void => {
    setGyms((prev) => prev.filter((gym) => gym.id !== id));
  };

  const columns: ColumnsType<Gym> = [
    { 
      title: "Название", 
      dataIndex: "name", 
      key: "name",
      width: "25%" 
    },
    { 
      title: "Адрес / Локация", 
      dataIndex: "location", 
      key: "location",
      width: "35%" 
    },
    { 
      title: "Вместимость", 
      dataIndex: "capacity", 
      key: "capacity",
      width: "20%" 
    },
    {
      title: "Действия",
      key: "actions",
      width: "20%",
      render: (_, record: Gym) => (
        <Space size="small">
          <Button type="link" size="small">
            Редактировать
          </Button>
          <Button 
            type="link" 
            danger 
            size="small" 
            onClick={() => handleDelete(record.id)}
          >
            Удалить
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ 
      padding: 24, 
      minHeight: "100vh", 
      boxSizing: "border-box" 
    }}>
      <Card 
        style={{ 
          width: "100%", 
          minHeight: "calc(100vh - 48px)",
          boxShadow: "0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24)"
        }}
        bodyStyle={{ 
          padding: 24,
          height: "100%",
          display: "flex",
          flexDirection: "column" 
        }}
      >
        {/* Заголовок + кнопка */}
        <Row 
          justify="space-between" 
          align="middle" 
          style={{ marginBottom: 24 }}
        >
          <Col>
            <Title 
              level={2} 
              style={{ 
                margin: 0,
                fontSize: "24px",
                fontWeight: 600
              }}
            >
              Спортзалы
            </Title>
          </Col>
          <Col>
            <Button 
              type="primary" 
              size="large"
            >
              Добавить спортзал
            </Button>
          </Col>
        </Row>

        {/* Таблица */}
        <div style={{ flex: 1, overflow: "hidden" }}>
          <Table<Gym>
            dataSource={gyms}
            columns={columns}
            rowKey={(record: Gym) => record.id}
            pagination={false}
            scroll={{ y: "calc(100vh - 200px)" }}
            style={{ 
              height: "100%",
              border: "1px solid #f0f0f0",
              borderRadius: "8px"
            }}
          />
        </div>
      </Card>
    </div>
  );
};

export default GymsPage;