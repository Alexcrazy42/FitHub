import React from 'react';
import { Card, Empty, Space, Typography } from 'antd';
import { ShopOutlined } from '@ant-design/icons';

const { Title, Text } = Typography;

export const MarketplaceProductsAdminPage: React.FC = () => {
  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Товары</Title>
          <Text type="secondary">Каталог, варианты, цены и остатки.</Text>
        </div>

        <Card>
          <Empty
            image={<ShopOutlined className="text-4xl text-gray-400" />}
            description="Редактор товаров появится после подключения admin API."
          />
        </Card>
      </Space>
    </div>
  );
};
