import React from 'react';
import { useParams } from 'react-router-dom';
import { Card, Descriptions, Empty, Space, Typography } from 'antd';

const { Title, Text } = Typography;

export const ProductDetailsPage: React.FC = () => {
  const { productId } = useParams();

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Товар</Title>
          <Text type="secondary">Выбор варианта и покупка.</Text>
        </div>

        <Card>
          <Descriptions column={1} size="small">
            <Descriptions.Item label="Product ID">{productId}</Descriptions.Item>
          </Descriptions>
          <Empty description="Детали товара появятся после подключения backend." />
        </Card>
      </Space>
    </div>
  );
};
