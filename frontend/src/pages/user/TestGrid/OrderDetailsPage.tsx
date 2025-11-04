// src/pages/user/OrderDetailsPage.tsx
import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
  Card,
  Descriptions,
  Tag,
  Badge,
  Typography,
  Space,
  Button,
  Skeleton,
} from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { Currency, Order, OrderStatus, PaymentMethod } from '../TestGrid/TestOrderGrid';
import { paymentMethodLabels } from './paymentMethodLabels';
import { orderStatusLabels } from './orderStatusLabels';

const { Title, Text } = Typography;

const OrderDetailsPage: React.FC = () => {
  const { orderId } = useParams<{ orderId: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!orderId) return;

    const loadOrder = async () => {
      setLoading(true);
      try {
        // Здесь должна быть настоящая загрузка по ID
        // Для демо — ищем в mockOrders
        const mockOrders = Array.from({ length: 150 }, (_, i) => ({
          id: `order-${i + 1}`,
          orderNumber: `ORD-${String(i + 1000).padStart(6, '0')}`,
          customerName: `Покупатель ${i + 1}`,
          email: `customer${i + 1}@example.com`,
          phone: `+7 (999) ${String(i + 100).padStart(4, '0')}`,
          status: ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'][i % 5] as OrderStatus,
          totalAmount: Math.floor(Math.random() * 1000) + 50,
          currency: ['RUB', 'USD', 'EUR'][i % 3] as Currency,
          createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString(),
          updatedAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString(),
          shippingAddress: `Улица ${i + 1}, Город ${i % 10 + 1}`,
          paymentMethod: ['Card', 'Paypal', 'Cash', 'BankTransfer'][i % 4] as PaymentMethod,
          itemsCount: Math.floor(Math.random() * 10) + 1,
          notes: i % 7 === 0 ? `Особое примечание к заказу ${i + 1}` : '',
        }));

        const found = mockOrders.find(o => o.id === orderId);
        setOrder(found || null);
      } catch (err) {
        console.error('Failed to load order', err);
      } finally {
        setLoading(false);
      }
    };

    loadOrder();
  }, [orderId]);

  if (loading) {
    return (
      <div className="p-4 max-w-4xl mx-auto">
        <Skeleton active />
      </div>
    );
  }

  if (!order) {
    return (
      <div className="p-4 max-w-4xl mx-auto text-center">
        <Title level={4}>Заказ не найден</Title>
        <Link to="/user/home">
          <Button type="link">← Вернуться к списку заказов</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="p-4 max-w-4xl mx-auto">
      <Card
        title={
          <Space>
            <Button type="text" icon={<ArrowLeftOutlined />} onClick={() => window.history.back()}>
              Назад
            </Button>
            <Title level={3} style={{ margin: 0 }}>
              Заказ №{order.orderNumber}
            </Title>
            <Badge
              status={
                order.status === 'Delivered'
                  ? 'success'
                  : order.status === 'Cancelled'
                  ? 'error'
                  : 'processing'
              }
              text={orderStatusLabels[order.status]}
            />
          </Space>
        }
      >
        <Descriptions column={1} bordered size="small">
          <Descriptions.Item label="Статус">
            <Tag
              color={
                order.status === 'Delivered'
                  ? 'green'
                  : order.status === 'Cancelled'
                  ? 'red'
                  : order.status === 'Pending'
                  ? 'orange'
                  : 'blue'
              }
            >
              {orderStatusLabels[order.status]}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="Сумма заказа">
            <Text strong>
              {order.totalAmount} {order.currency}
            </Text>
          </Descriptions.Item>
          <Descriptions.Item label="Количество товаров">
            {order.itemsCount} шт.
          </Descriptions.Item>
          <Descriptions.Item label="Дата создания">
            {dayjs(order.createdAt).format('DD.MM.YYYY HH:mm:ss')}
          </Descriptions.Item>
          <Descriptions.Item label="Последнее обновление">
            {dayjs(order.updatedAt).format('DD.MM.YYYY HH:mm:ss')}
          </Descriptions.Item>
          <Descriptions.Item label="Покупатель">
            {order.customerName}
          </Descriptions.Item>
          <Descriptions.Item label="Email">
            <a href={`mailto:${order.email}`}>{order.email}</a>
          </Descriptions.Item>
          <Descriptions.Item label="Телефон">
            <a href={`tel:${order.phone}`}>{order.phone}</a>
          </Descriptions.Item>
          <Descriptions.Item label="Адрес доставки">
            {order.shippingAddress}
          </Descriptions.Item>
          <Descriptions.Item label="Способ оплаты">
            {paymentMethodLabels[order.paymentMethod]}
          </Descriptions.Item>
          {order.notes && (
            <Descriptions.Item label="Примечания">
              {order.notes}
            </Descriptions.Item>
          )}
          <Descriptions.Item label="Внутренний ID">
            <Text copyable>{order.id}</Text>
          </Descriptions.Item>
        </Descriptions>
      </Card>
    </div>
  );
};

export default OrderDetailsPage;