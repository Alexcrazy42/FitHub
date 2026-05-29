import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Button, Card, Empty, Space, Table, Tag, Typography } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import { ReloadOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import { Delivery } from '../../../types/marketplace';

const { Text, Title } = Typography;

const getStatusColor = (status: string) => {
  if (status === 'Pending' || status === 'Assembling') {
    return 'blue';
  }

  if (status === 'Delivered') {
    return 'green';
  }

  if (status === 'Failed' || status === 'Cancelled') {
    return 'red';
  }

  return 'gold';
};

export const MarketplaceDeliveriesAdminPage: React.FC = () => {
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [deliveries, setDeliveries] = useState<Delivery[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);

  const loadDeliveries = useCallback(async (nextPage = page, nextPageSize = pageSize, showLoading = true) => {
    if (showLoading) {
      setLoading(true);
    }

    const response = await marketplaceApi.getDeliveries(nextPage, nextPageSize);

    if (response.success && response.data) {
      setDeliveries(response.data.items);
      setTotal(response.data.totalItems ?? response.data.items.length);
    } else {
      setDeliveries([]);
      setTotal(0);
    }

    if (showLoading) {
      setLoading(false);
    }
  }, [marketplaceApi, page, pageSize]);

  useEffect(() => {
    loadDeliveries(page, pageSize);
    const intervalId = window.setInterval(() => loadDeliveries(page, pageSize, false), 5000);

    return () => {
      window.clearInterval(intervalId);
    };
  }, [loadDeliveries, page, pageSize]);

  const columns: ColumnsType<Delivery> = [
    {
      title: 'Заказ',
      dataIndex: 'orderId',
      render: (_, delivery) => (
        <Space direction="vertical" size={0}>
          <Text strong>{delivery.order?.items[0]?.productName ?? 'Заказ'}</Text>
          <Text type="secondary">{delivery.orderId}</Text>
        </Space>
      ),
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      width: 170,
      render: (status: string) => <Tag color={getStatusColor(status)}>{status}</Tag>,
    },
    {
      title: 'Курьер',
      dataIndex: 'courierName',
      width: 180,
      render: (courierName: string | null) => courierName ?? 'Не назначен',
    },
    {
      title: 'Создана',
      dataIndex: 'createdAt',
      width: 180,
      render: (createdAt: string) => new Date(createdAt).toLocaleString('ru-RU'),
    },
  ];

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div className="flex items-center justify-between gap-4">
          <div>
            <Title level={2}>Доставки</Title>
            <Text type="secondary">Сборка заказов и назначение курьеров.</Text>
          </div>
          <Button icon={<ReloadOutlined />} onClick={() => loadDeliveries(page, pageSize)} loading={loading}>
            Обновить
          </Button>
        </div>

        <Card>
          <Table
            rowKey="deliveryId"
            columns={columns}
            dataSource={deliveries}
            loading={loading}
            locale={{ emptyText: <Empty description="Доставок пока нет." /> }}
            pagination={{
              current: page,
              pageSize,
              total,
              showSizeChanger: true,
              onChange: (nextPage, nextPageSize) => {
                setPage(nextPage);
                setPageSize(nextPageSize);
                loadDeliveries(nextPage, nextPageSize);
              },
            }}
          />
        </Card>
      </Space>
    </div>
  );
};
