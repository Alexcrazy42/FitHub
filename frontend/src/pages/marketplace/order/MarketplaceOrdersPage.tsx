import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Alert,
  Button,
  Card,
  Empty,
  Image,
  Space,
  Table,
  Tag,
  Typography,
} from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { EyeOutlined, ReloadOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import { MarketplaceOrder } from '../../../types/marketplace';

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

export const MarketplaceOrdersPage: React.FC = () => {
  const navigate = useNavigate();
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [orders, setOrders] = useState<MarketplaceOrder[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadOrders = async (nextPage = page, nextPageSize = pageSize) => {
    setLoading(true);
    setError(null);

    const response = await marketplaceApi.getMyOrders(nextPage, nextPageSize);

    if (!response.success || !response.data) {
      setOrders([]);
      setTotal(0);
      setError(response.error?.detail ?? 'Не удалось загрузить заказы.');
      setLoading(false);
      return;
    }

    setOrders(response.data.items);
    setTotal(response.data.totalItems ?? response.data.items.length);
    setLoading(false);
  };

  useEffect(() => {
    loadOrders();
  }, []);

  const columns: ColumnsType<MarketplaceOrder> = [
    {
      title: 'Заказ',
      dataIndex: 'orderId',
      render: (_, order) => {
        const firstItem = order.items[0];

        return (
          <Space>
            {firstItem?.image ? (
              <Image
                preview={false}
                src={firstItem.image.url}
                alt={firstItem.image.altText ?? firstItem.productName}
                width={56}
                height={56}
                style={{ objectFit: 'cover' }}
              />
            ) : (
              <div className="flex h-14 w-14 items-center justify-center bg-gray-100">
                <ShoppingCartOutlined className="text-2xl text-gray-400" />
              </div>
            )}
            <Space direction="vertical" size={0}>
              <Text strong>{firstItem?.productName ?? 'Заказ'}</Text>
              <Text type="secondary">{order.orderId}</Text>
              {order.items.length > 1 && (
                <Text type="secondary">Позиций: {order.items.length}</Text>
              )}
            </Space>
          </Space>
        );
      },
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      width: 160,
      render: (status: string) => <Tag color={getStatusColor(status)}>{status}</Tag>,
    },
    {
      title: 'Сумма',
      dataIndex: 'total',
      width: 160,
      render: (_, order) => formatMoney(order.total.amount, order.total.currency),
    },
    {
      title: 'Создан',
      dataIndex: 'createdAt',
      width: 180,
      render: (createdAt: string) => new Date(createdAt).toLocaleString('ru-RU'),
    },
    {
      title: '',
      key: 'actions',
      width: 120,
      render: (_, order) => (
        <Button
          type="link"
          icon={<EyeOutlined />}
          onClick={() => navigate(`/gym-admin/marketplace/orders/${order.orderId}`)}
        >
          Открыть
        </Button>
      ),
    },
  ];

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div className="flex items-center justify-between gap-4">
          <div>
            <Title level={2}>Мои заказы</Title>
            <Text type="secondary">Заказы из магазина и дальнейший статус доставки.</Text>
          </div>
          <Button icon={<ReloadOutlined />} onClick={() => loadOrders()} loading={loading}>
            Обновить
          </Button>
        </div>

        {error && (
          <Alert type="error" showIcon message="Заказы недоступны" description={error} />
        )}

        <Card>
          <Table
            rowKey="orderId"
            columns={columns}
            dataSource={orders}
            loading={loading}
            locale={{ emptyText: <Empty description="Заказов пока нет." /> }}
            pagination={{
              current: page,
              pageSize,
              total,
              showSizeChanger: true,
              onChange: (nextPage, nextPageSize) => {
                setPage(nextPage);
                setPageSize(nextPageSize);
                loadOrders(nextPage, nextPageSize);
              },
            }}
          />
        </Card>
      </Space>
    </div>
  );
};
