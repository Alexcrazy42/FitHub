import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Alert,
  Button,
  Card,
  Descriptions,
  Empty,
  Image,
  Result,
  Skeleton,
  Space,
  Steps,
  Table,
  Tag,
  Typography,
} from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import { Delivery, MarketplaceOrder, MarketplaceOrderItem } from '../../../types/marketplace';

const { Text, Title } = Typography;

const formatMoney = (amount: number, currency: string) =>
  new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);

const getStatusColor = (status: string) => {
  if (status === 'Created') {
    return 'green';
  }

  if (status === 'Cancelled') {
    return 'red';
  }

  return 'blue';
};

export const MarketplaceOrderStatusPage: React.FC = () => {
  const { orderId } = useParams();
  const navigate = useNavigate();
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [order, setOrder] = useState<MarketplaceOrder | null>(null);
  const [delivery, setDelivery] = useState<Delivery | null>(null);
  const [deliveryError, setDeliveryError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadOrder = useCallback(async (showLoading = false) => {
    if (!orderId) {
      setError('Не найден идентификатор заказа.');
      return;
    }

    if (showLoading) {
      setLoading(true);
    }

    setError(null);
    const response = await marketplaceApi.getOrder(orderId);

    if (!response.success || !response.data) {
      setOrder(null);
      setError(response.error?.detail ?? 'Не удалось загрузить заказ.');
      setLoading(false);
      return;
    }

    setOrder(response.data);
    const deliveryResponse = await marketplaceApi.getOrderDelivery(response.data.orderId);

    if (deliveryResponse.success && deliveryResponse.data) {
      setDelivery(deliveryResponse.data);
      setDeliveryError(null);
    } else {
      setDelivery(null);
      setDeliveryError(deliveryResponse.error?.detail ?? 'Доставка для заказа еще не создана.');
    }

    if (showLoading) {
      setLoading(false);
    }
  }, [marketplaceApi, orderId]);

  useEffect(() => {
    loadOrder(true);
    const intervalId = window.setInterval(() => loadOrder(false), 5000);

    return () => {
      window.clearInterval(intervalId);
    };
  }, [loadOrder]);

  const columns: ColumnsType<MarketplaceOrderItem> = [
    {
      title: 'Товар',
      dataIndex: 'productName',
      render: (_, item) => (
        <Space>
          {item.image ? (
            <Image
              preview={false}
              src={item.image.url}
              alt={item.image.altText ?? item.productName}
              width={64}
              height={64}
              style={{ objectFit: 'cover' }}
            />
          ) : (
            <div className="flex h-16 w-16 items-center justify-center bg-gray-100">
              <ShoppingCartOutlined className="text-2xl text-gray-400" />
            </div>
          )}
          <Space direction="vertical" size={0}>
            {item.brandName && <Text type="secondary">{item.brandName}</Text>}
            <Text strong>{item.productName}</Text>
            <Text type="secondary">{item.variantName ?? item.sku}</Text>
            {item.attributeSummary && <Text type="secondary">{item.attributeSummary}</Text>}
          </Space>
        </Space>
      ),
    },
    {
      title: 'Количество',
      dataIndex: 'quantity',
      width: 130,
    },
    {
      title: 'Цена',
      dataIndex: 'unitPrice',
      width: 160,
      render: (_, item) => formatMoney(item.unitPrice.amount, item.unitPrice.currency),
    },
    {
      title: 'Итого',
      dataIndex: 'total',
      width: 160,
      render: (_, item) => formatMoney(item.total.amount, item.total.currency),
    },
  ];

  if (loading) {
    return (
      <div className="container mx-auto p-6">
        <Skeleton active />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto p-6">
        <Result
          status="warning"
          title="Заказ недоступен"
          subTitle={error}
          extra={<Button onClick={() => navigate('/gym-admin/marketplace/catalog')}>Вернуться в каталог</Button>}
        />
      </div>
    );
  }

  if (!order) {
    return (
      <div className="container mx-auto p-6">
        <Empty description="Заказ не найден." />
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Заказ оформлен</Title>
          <Text type="secondary">Мы готовим заказ к передаче в доставку.</Text>
        </div>

        <Alert
          type="success"
          showIcon
          message="Оплата подтверждена"
          description="Заказ создан. Следующий шаг появится после подготовки доставки."
        />

        <Card>
          <Descriptions column={{ xs: 1, md: 2 }}>
            <Descriptions.Item label="Заказ">{order.orderId}</Descriptions.Item>
            <Descriptions.Item label="Статус">
              <Tag color={getStatusColor(order.status)}>{order.status}</Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Создан">{new Date(order.createdAt).toLocaleString('ru-RU')}</Descriptions.Item>
            <Descriptions.Item label="Сумма">
              {formatMoney(order.total.amount, order.total.currency)}
            </Descriptions.Item>
          </Descriptions>
        </Card>

        <Card title="Состав заказа">
          <Table
            rowKey="orderItemId"
            columns={columns}
            dataSource={order.items}
            pagination={false}
          />
        </Card>

        <Card title="Доставка">
          {delivery ? (
            <Space direction="vertical" size="middle" className="w-full">
              <Descriptions column={{ xs: 1, md: 2 }}>
                <Descriptions.Item label="Статус">
                  <Tag color={getStatusColor(delivery.status)}>{delivery.status}</Tag>
                </Descriptions.Item>
                <Descriptions.Item label="Курьер">
                  {delivery.courierName ?? 'Курьер еще не назначен'}
                </Descriptions.Item>
                <Descriptions.Item label="Создана">
                  {new Date(delivery.createdAt).toLocaleString('ru-RU')}
                </Descriptions.Item>
                <Descriptions.Item label="Последнее изменение">
                  {new Date(delivery.lastStateChangedAt).toLocaleString('ru-RU')}
                </Descriptions.Item>
                {delivery.courierAssignmentExpiresAt && (
                  <Descriptions.Item label="Ожидаем ответ до">
                    {new Date(delivery.courierAssignmentExpiresAt).toLocaleString('ru-RU')}
                  </Descriptions.Item>
                )}
              </Descriptions>

              <Steps
                direction="vertical"
                items={delivery.events.map((event) => ({
                  title: event.status,
                  description: event.message ?? new Date(event.createdAt).toLocaleString('ru-RU'),
                }))}
              />
            </Space>
          ) : (
            <Alert
              type="info"
              showIcon
              message="Доставка готовится"
              description={deliveryError ?? 'Статус доставки появится после подготовки заказа.'}
            />
          )}
        </Card>

        <Card title="История статусов">
          <Steps
            direction="vertical"
            items={order.statusHistory.map((history) => ({
              title: history.status,
              description: history.reason ?? new Date(history.createdAt).toLocaleString('ru-RU'),
            }))}
          />
        </Card>

        <Button onClick={() => navigate('/gym-admin/marketplace/catalog')}>
          Вернуться в каталог
        </Button>
      </Space>
    </div>
  );
};
