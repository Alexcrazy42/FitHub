import React, { useState, useEffect, useMemo } from 'react';
import {
  Table,
  Pagination,
  Input,
  Tag,
  Card,
  InputNumber,
  Select,
  Checkbox,
  Popover,
  Button,
  Empty
} from 'antd';
import { SettingOutlined, SortAscendingOutlined } from '@ant-design/icons';
import { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { OrderDetailsDrawer } from './OrderDetailsDrawer';
import { statusLabels } from './statusLabels';
import { paymentMethodLabels } from './paymentMethodLabels';

const { Option } = Select;

export interface Order {
  id: string;
  orderNumber: string;
  customerName: string;
  email: string;
  phone: string;
  status: 'pending' | 'confirmed' | 'shipped' | 'delivered' | 'cancelled';
  totalAmount: number;
  currency: 'USD' | 'EUR' | 'RUB';
  createdAt: string;
  updatedAt: string;
  shippingAddress: string;
  paymentMethod: 'card' | 'paypal' | 'cash' | 'bank_transfer';
  itemsCount: number;
  notes: string;
}

type OrderFilters = {
  status?: Order['status'][];
  paymentMethod?: Order['paymentMethod'][];
  minAmount?: number;
  maxAmount?: number;
};

type SortConfig = {
  field: keyof Order;
  direction: 'asc' | 'desc';
};

const mockOrders: Order[] = Array.from({ length: 150 }, (_, i) => ({
  id: `order-${i + 1}`,
  orderNumber: `ORD-${String(i + 1000).padStart(6, '0')}`,
  customerName: `Покупатель ${i + 1}`,
  email: `customer${i + 1}@example.com`,
  phone: `+7 (999) ${String(i + 100).padStart(4, '0')}`,
  status: ['pending', 'confirmed', 'shipped', 'delivered', 'cancelled'][i % 5] as Order['status'],
  totalAmount: Math.floor(Math.random() * 1000) + 50,
  currency: ['RUB', 'USD', 'EUR'][i % 3] as Order['currency'],
  createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString(),
  updatedAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString(),
  shippingAddress: `Улица ${i + 1}, Город ${i % 10 + 1}`,
  paymentMethod: ['card', 'paypal', 'cash', 'bank_transfer'][i % 4] as Order['paymentMethod'],
  itemsCount: Math.floor(Math.random() * 10) + 1,
  notes: i % 7 === 0 ? `Особое примечание к заказу ${i + 1}` : '',
}));

const columnLabels: Record<keyof Order, string> = {
  id: 'Айди',
  orderNumber: 'Номер заказа',
  customerName: 'Покупатель',
  email: 'Email',
  phone: 'Телефон',
  status: 'Статус',
  totalAmount: 'Сумма',
  currency: 'Валюта',
  createdAt: 'Создан',
  updatedAt: 'Обновлён',
  shippingAddress: 'Адрес доставки',
  paymentMethod: 'Способ оплата',
  itemsCount: 'Товаров',
  notes: 'Примечания',
};

const sortableFields: { value: keyof Order; label: string }[] = [
  { value: 'createdAt', label: 'Дата создания' },
  { value: 'updatedAt', label: 'Дата обновления' },
  { value: 'totalAmount', label: 'Сумма заказа' },
  { value: 'orderNumber', label: 'Номер заказа' },
];

const fetchOrders = ({
  page,
  pageSize,
  searchQuery,
  filters,
  sortConfig
}: {
  page: number;
  pageSize: number;
  searchQuery?: string;
  filters?: OrderFilters;
  sortConfig?: SortConfig;
}): Promise<{ data: Order[]; total: number }> => {
  return new Promise((resolve) => {
    setTimeout(() => {
      let filtered = [...mockOrders];

      if (searchQuery) {
        const q = searchQuery.toLowerCase();
        filtered = filtered.filter((order) =>
          Object.values(order).some(
            (val) =>
              (typeof val === 'string' || typeof val === 'number') &&
              String(val).toLowerCase().includes(q)
          )
        );
      }

      if (filters) {
        if (filters.status?.length) {
          filtered = filtered.filter((order) => filters.status!.includes(order.status));
        }
        if (filters.paymentMethod?.length) {
          filtered = filtered.filter((order) => filters.paymentMethod!.includes(order.paymentMethod));
        }
        if (filters.minAmount !== undefined) {
          filtered = filtered.filter((order) => order.totalAmount >= filters.minAmount!);
        }
        if (filters.maxAmount !== undefined) {
          filtered = filtered.filter((order) => order.totalAmount <= filters.maxAmount!);
        }
      }

      if (sortConfig) {
        filtered.sort((a, b) => {
          const aVal = a[sortConfig.field];
          const bVal = b[sortConfig.field];

          if (typeof aVal === 'string' && typeof bVal === 'string') {
            return sortConfig.direction === 'desc'
              ? bVal.localeCompare(aVal)
              : aVal.localeCompare(bVal);
          }
          if (typeof aVal === 'number' && typeof bVal === 'number') {
            return sortConfig.direction === 'desc' ? bVal - aVal : aVal - bVal;
          }
          return 0;
        });
      }

      const total = filtered.length;
      const start = (page - 1) * pageSize;
      const paginated = filtered.slice(start, start + pageSize);

      resolve({ data: paginated, total });
    }, 400);
  });
};

const UserHome: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchQuery, setSearchQuery] = useState('');
  const [appliedFilters, setAppliedFilters] = useState<OrderFilters>({});
  const [visibleColumns, setVisibleColumns] = useState<Record<keyof Order, boolean>>({
    id: false,
    orderNumber: true,
    customerName: true,
    email: true,
    phone: true,
    status: true,
    totalAmount: true,
    currency: true,
    createdAt: true,
    updatedAt: true,
    shippingAddress: false,
    paymentMethod: false,
    itemsCount: false,
    notes: false,
  });
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [drawerVisible, setDrawerVisible] = useState(false);

  const [tempSort, setTempSort] = useState<SortConfig>({
    field: 'createdAt',
    direction: 'desc'
  });
  const [appliedSort, setAppliedSort] = useState<SortConfig>({
    field: 'createdAt',
    direction: 'desc'
  });

  const handleRowClick = (order: Order) => {
    setSelectedOrder(order);
    setDrawerVisible(true);
  };

  const buildQuery = (filters: OrderFilters, search: string, sortConfig: SortConfig) => {
    const params = new URLSearchParams();

    if (search) {
      params.set('q', search);
    }

    // Статусы
    if (filters.status && filters.status.length > 0) {
      filters.status.forEach((status) => params.append('status', status));
    }

    if (filters.paymentMethod && filters.paymentMethod.length > 0) {
      filters.paymentMethod.forEach((method) => params.append('paymentMethod', method));
    }

    if (filters.minAmount !== undefined) {
      params.set('minAmount', filters.minAmount.toString());
    }
    if (filters.maxAmount !== undefined) {
      params.set('maxAmount', filters.maxAmount.toString());
    }

    if (sortConfig) {
      params.set('sortBy', sortConfig.field);
      params.set('sortOrder', sortConfig.direction);
    }

    params.set('page', page.toString());
    params.set('pageSize', pageSize.toString());

    const queryString = params.toString();
    console.log('Fake API request:', `/api/orders?${queryString}`);
    return queryString;
  };


  const closeDrawer = () => {
    setDrawerVisible(false);
    setSelectedOrder(null);
  };

  const loadOrders = async () => {
    setLoading(true);
    const response = await fetchOrders({
      page,
      pageSize,
      searchQuery,
      filters: appliedFilters,
      sortConfig: appliedSort
    });
    setOrders(response.data);
    setTotal(response.total);
    setLoading(false);
  };

  useEffect(() => {
    loadOrders();
  }, [page, pageSize]);

  const toggleColumn = (key: keyof Order) => {
    setVisibleColumns((prev) => ({ ...prev, [key]: !prev[key] }));
  };

  const handleAmountChange = (field: 'minAmount' | 'maxAmount', value: number | null) => {
    setAppliedFilters((prev) => ({ ...prev, [field]: value ?? undefined }));
  };


  const applyFiltersAndSort = () => {
    setAppliedSort(tempSort);
    setPage(1);
    loadOrders();
    buildQuery(appliedFilters, searchQuery, tempSort);
  };

  const columns: ColumnsType<Order> = useMemo(() => {
    const allColumns: ColumnsType<Order> = [
      {
        title: 'Айди',
        dataIndex: 'id',
        key: 'id',
        width: 120,
      },
      {
        title: 'Номер заказа',
        dataIndex: 'orderNumber',
        key: 'orderNumber',
        width: 140,
      },
      {
        title: 'Покупатель',
        dataIndex: 'customerName',
        key: 'customerName',
        width: 160,
      },
      {
        title: 'Email',
        dataIndex: 'email',
        key: 'email',
        width: 200,
      },
      {
        title: 'Телефон',
        dataIndex: 'phone',
        key: 'phone',
        width: 140,
      },
      {
        title: 'Статус',
        dataIndex: 'status',
        key: 'status',
        width: 120,
        render: (status: Order['status']) => {
          const colorMap = {
            pending: 'orange',
            confirmed: 'blue',
            shipped: 'geekblue',
            delivered: 'green',
            cancelled: 'red',
          };
          return <Tag color={colorMap[status]}>{statusLabels[status]}</Tag>;
        },
      },
      {
        title: 'Сумма',
        dataIndex: 'totalAmount',
        key: 'totalAmount',
        width: 120,
        render: (amount: number, record) => `${amount} ${record.currency}`,
      },
      {
        title: 'Создан',
        dataIndex: 'createdAt',
        key: 'createdAt',
        width: 160,
        render: (date: string) => dayjs(date).format('DD.MM.YYYY HH:mm'),
      },
      {
        title: 'Обновлён',
        dataIndex: 'updatedAt',
        key: 'updatedAt',
        width: 160,
        render: (date: string) => dayjs(date).format('DD.MM.YYYY HH:mm'),
      },
      {
        title: 'Адрес доставки',
        dataIndex: 'shippingAddress',
        key: 'shippingAddress',
        width: 200,
      },
      {
        title: 'Способ оплаты',
        dataIndex: 'paymentMethod',
        key: 'paymentMethod',
        onFilter: () => true,
        width: 140,
        render: (method: Order['paymentMethod']) => paymentMethodLabels[method],
      },
      {
        title: 'Товаров',
        dataIndex: 'itemsCount',
        key: 'itemsCount',
        width: 80,
      },
      {
        title: 'Примечания',
        dataIndex: 'notes',
        key: 'notes',
        width: 200,
      },
    ];


    return allColumns.filter((col) => visibleColumns[col.key as keyof Order]);
  }, [visibleColumns]);

  return (
    <div className="p-4 max-w-7xl mx-auto">
      <Card className="mb-4">
        <div className="flex flex-wrap gap-4 items-end justify-between">
          <Input
            placeholder="Поиск по заказам..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            allowClear
            style={{ width: 300 }}
          />

          <Popover
            content={
              <div className="flex flex-col gap-2 max-h-60 overflow-y-auto p-2 w-48">
                {Object.entries(visibleColumns).map(([key, visible]) => (
                  <Checkbox
                    key={key}
                    checked={visible}
                    onChange={() => toggleColumn(key as keyof Order)}
                  >
                    <span className="text-sm">{columnLabels[key as keyof Order]}</span>
                  </Checkbox>
                ))}
              </div>
            }
            title="Отображаемые колонки"
            trigger="click"
            placement="bottomRight"
          >
            <Button icon={<SettingOutlined />} size="middle">
              Колонки
            </Button>
          </Popover>
        </div>

        {/* Дополнительные фильтры под поиском */}
        <div className="mt-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="flex gap-2">
            <InputNumber
              placeholder="От суммы"
              value={appliedFilters.minAmount ?? null}
              onChange={(val) => handleAmountChange('minAmount', val)}
              className="w-full"
            />
            <InputNumber
              placeholder="До суммы"
              value={appliedFilters.maxAmount ?? null}
              onChange={(val) => handleAmountChange('maxAmount', val)}
              className="w-full"
            />
          </div>

          <Select
            mode="multiple"
            placeholder="Статус"
            value={appliedFilters.status}
            onChange={(value) => setAppliedFilters((prev) => ({ ...prev, status: value }))}
            allowClear
            className="w-full"
          >
            <Option value="pending">В ожидании</Option>
            <Option value="confirmed">Подтверждён</Option>
            <Option value="shipped">Отправлен</Option>
            <Option value="delivered">Доставлен</Option>
            <Option value="cancelled">Отменён</Option>
          </Select>

          <Select
            mode="multiple"
            placeholder="Способ оплаты"
            value={appliedFilters.paymentMethod}
            onChange={(value) => setAppliedFilters((prev) => ({ ...prev, paymentMethod: value }))}
            allowClear
            className="w-full"
          >
            <Option value="card">Карта</Option>
            <Option value="paypal">PayPal</Option>
            <Option value="cash">Наличные</Option>
            <Option value="bank_transfer">Банковский перевод</Option>
          </Select>

          {/* 🔥 ДОБАВЛЯЕМ ВЫБОР СОРТИРОВКИ */}
          <div className="flex gap-2">
            <Select
              value={tempSort.field}
              onChange={(value) => setTempSort(prev => ({ ...prev, field: value }))}
              className="w-full"
              placeholder="Сортировать по"
            >
              {sortableFields.map(field => (
                <Option key={field.value} value={field.value}>
                  {field.label}
                </Option>
              ))}
            </Select>
            
            <Select
              value={tempSort.direction}
              onChange={(value) => setTempSort(prev => ({ ...prev, direction: value }))}
              className="w-32"
            >
              <Option value="asc">По возрастанию</Option>
              <Option value="desc">По убыванию</Option>
            </Select>
          </div>
        </div>

        {/* Кнопка применения */}
        <div className="mt-4 flex justify-end">
          <Button
            icon={<SortAscendingOutlined />}
            onClick={applyFiltersAndSort}
            style={{ width: 200 }}
          >
            Применить
          </Button>
        </div>
      </Card>

      {/* Пагинация сверху */}
      <div className="mb-4 flex justify-end">
        <Pagination
          current={page}
          pageSize={pageSize}
          total={total}
          onChange={(p, ps) => {
            setPage(p);
            if (ps !== pageSize) setPageSize(ps!);
          }}
          showSizeChanger
          showTotal={(total) => `Всего: ${total} заказов`}
          locale={{
            items_per_page: '/ страница',
            jump_to: 'Перейти',
            jump_to_confirm: 'подтвердить',
            page: 'Страница',
            prev_page: 'Предыдущая',
            next_page: 'Следующая',
            prev_5: 'Предыдущие 5',
            next_5: 'Следующие 5',
            prev_3: 'Предыдущие 3',
            next_3: 'Следующие 3',
          }}
        />
      </div>

      <Table
        dataSource={orders}
        columns={columns}
        rowKey="id"
        loading={loading}
        pagination={false}
        scroll={{ x: 'max-content' }}
        onRow={(record) => ({
          onClick: () => handleRowClick(record),
          style: { cursor: 'pointer' },
        })}
        locale={{
          emptyText: (
            <Empty
              description={
                <div>
                  <div style={{ marginBottom: 8 }}>Заказы не найдены</div>
                  <Button 
                    
                    onClick={() => {
                      setSearchQuery('');
                      setAppliedFilters({});
                      setPage(1);
                    }}
                  >
                    Сбросить фильтры
                  </Button>
                </div>
              }
            />
          )
        }}
      />

      <OrderDetailsDrawer
        selectedOrder={selectedOrder}
        drawerVisible={drawerVisible}
        onClose={closeDrawer}
      />
    </div>
  );
};

export default UserHome;