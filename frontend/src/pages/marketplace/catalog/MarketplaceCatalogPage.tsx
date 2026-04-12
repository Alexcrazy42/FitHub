import React from 'react';
import { Button, Card, Empty, Space, Typography } from 'antd';
import { ReloadOutlined, ShoppingCartOutlined } from '@ant-design/icons';

const { Title, Text } = Typography;

export const MarketplaceCatalogPage: React.FC = () => {
  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Магазин</Title>
          <Text type="secondary">Каталог товаров для зала.</Text>
        </div>

        <Card>
          <Empty
            image={<ShoppingCartOutlined className="text-4xl text-gray-400" />}
            description="Товары появятся после подключения каталога."
          >
            <Button icon={<ReloadOutlined />}>Обновить</Button>
          </Empty>
        </Card>
      </Space>
    </div>
  );
};
