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
} from 'antd';
import { SettingOutlined } from '@ant-design/icons';
import { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import { FilterValue, SorterResult } from 'antd/es/table/interface';
import { AnyObject } from 'antd/es/_util/type';
import dayjs from 'dayjs';

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

// Мок-данные (без изменений)
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

// Маппинг для русских названий колонок
const columnLabels: Record<keyof Order, string> = {
  id: 'ID',
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
  paymentMethod: 'Способ оплаты',
  itemsCount: 'Товаров',
  notes: 'Примечания',
};

// Локализация статусов
const statusLabels: Record<Order['status'], string> = {
  pending: 'В ожидании',
  confirmed: 'Подтверждён',
  shipped: 'Отправлен',
  delivered: 'Доставлен',
  cancelled: 'Отменён',
};

// Локализация способов оплаты
const paymentMethodLabels: Record<Order['paymentMethod'], string> = {
  card: 'Карта',
  paypal: 'PayPal',
  cash: 'Наличные',
  bank_transfer: 'Банковский перевод',
};

// Мок-АПИ (без изменений)
const fetchOrders = ({
  page,
  pageSize,
  sortField,
  sortOrder,
  searchQuery,
  filters,
}: {
  page: number;
  pageSize: number;
  sortField?: keyof Order;
  sortOrder?: 'ascend' | 'descend';
  searchQuery?: string;
  filters?: OrderFilters;
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

      if (sortField) {
        filtered.sort((a, b) => {
          const aVal = a[sortField];
          const bVal = b[sortField];

          if (typeof aVal === 'string' && typeof bVal === 'string') {
            return sortOrder === 'descend'
              ? bVal.localeCompare(aVal)
              : aVal.localeCompare(bVal);
          }
          if (typeof aVal === 'number' && typeof bVal === 'number') {
            return sortOrder === 'descend' ? bVal - aVal : aVal - bVal;
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
  const [sortField, setSortField] = useState<keyof Order | undefined>(undefined);
  const [sortOrder, setSortOrder] = useState<'ascend' | 'descend' | undefined>(undefined);
  const [searchQuery, setSearchQuery] = useState('');
  const [filters, setFilters] = useState<OrderFilters>({});
  const [visibleColumns, setVisibleColumns] = useState<Record<keyof Order, boolean>>({
    id: true,
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

  const loadOrders = async () => {
    setLoading(true);
    const response = await fetchOrders({
      page,
      pageSize,
      sortField,
      sortOrder,
      searchQuery,
      filters,
    });
    setOrders(response.data);
    setTotal(response.total);
    setLoading(false);
  };

  useEffect(() => {
    loadOrders();
  }, [page, pageSize, sortField, sortOrder, searchQuery, filters]);

  const handleTableChange = (
    _pagination: TablePaginationConfig,
    filters: Record<string, FilterValue | null>,
    sorter: SorterResult<AnyObject> | SorterResult<AnyObject>[]
  ) => {
    const sort = Array.isArray(sorter) ? sorter[0] : sorter;
    if (sort.field) {
      setSortField(sort.field as keyof Order);
      setSortOrder(sort.order);
    } else {
      setSortField(undefined);
      setSortOrder(undefined);
    }

    setFilters((prev) => ({
      ...prev,
      status: (filters.status as Order['status'][]) || undefined,
      paymentMethod: (filters.paymentMethod as Order['paymentMethod'][]) || undefined,
    }));
  };

  const toggleColumn = (key: keyof Order) => {
    setVisibleColumns((prev) => ({ ...prev, [key]: !prev[key] }));
  };

  const handleAmountChange = (field: 'minAmount' | 'maxAmount', value: number | null) => {
    setFilters((prev) => ({ ...prev, [field]: value ?? undefined }));
  };

  const columns: ColumnsType<Order> = useMemo(() => {
    const allColumns: ColumnsType<Order> = [
      {
        title: 'ID',
        dataIndex: 'id',
        key: 'id',
        sorter: true,
        width: 120,
      },
      {
        title: 'Номер заказа',
        dataIndex: 'orderNumber',
        key: 'orderNumber',
        sorter: true,
        width: 140,
      },
      {
        title: 'Покупатель',
        dataIndex: 'customerName',
        key: 'customerName',
        sorter: true,
        width: 160,
      },
      {
        title: 'Email',
        dataIndex: 'email',
        key: 'email',
        sorter: true,
        width: 200,
      },
      {
        title: 'Телефон',
        dataIndex: 'phone',
        key: 'phone',
        sorter: true,
        width: 140,
      },
      {
        title: 'Статус',
        dataIndex: 'status',
        key: 'status',
        sorter: true,
        filters: [
          { text: 'В ожидании', value: 'pending' },
          { text: 'Подтверждён', value: 'confirmed' },
          { text: 'Отправлен', value: 'shipped' },
          { text: 'Доставлен', value: 'delivered' },
          { text: 'Отменён', value: 'cancelled' },
        ],
        onFilter: () => true,
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
        sorter: true,
        width: 120,
        render: (amount: number, record) => `${amount} ${record.currency}`,
      },
      {
        title: 'Создан',
        dataIndex: 'createdAt',
        key: 'createdAt',
        sorter: true,
        width: 160,
        render: (date: string) => dayjs(date).format('DD.MM.YYYY HH:mm'),
      },
      {
        title: 'Обновлён',
        dataIndex: 'updatedAt',
        key: 'updatedAt',
        sorter: true,
        width: 160,
        render: (date: string) => dayjs(date).format('DD.MM.YYYY HH:mm'),
      },
      {
        title: 'Адрес доставки',
        dataIndex: 'shippingAddress',
        key: 'shippingAddress',
        sorter: true,
        width: 200,
      },
      {
        title: 'Способ оплаты',
        dataIndex: 'paymentMethod',
        key: 'paymentMethod',
        sorter: true,
        filters: [
          { text: 'Карта', value: 'card' },
          { text: 'PayPal', value: 'paypal' },
          { text: 'Наличные', value: 'cash' },
          { text: 'Банковский перевод', value: 'bank_transfer' },
        ],
        onFilter: () => true,
        width: 140,
        render: (method: Order['paymentMethod']) => paymentMethodLabels[method],
      },
      {
        title: 'Товаров',
        dataIndex: 'itemsCount',
        key: 'itemsCount',
        sorter: true,
        width: 80,
      },
      {
        title: 'Примечания',
        dataIndex: 'notes',
        key: 'notes',
        sorter: true,
        width: 200,
      },
    ];

    return allColumns.filter((col) => visibleColumns[col.key as keyof Order]);
  }, [visibleColumns]);

  return (
    <div className="p-4 max-w-7xl mx-auto">
      <Card className="mb-4">
        <div className="flex flex-wrap gap-4 items-end justify-between">
          {/* Поиск */}
          <Input
            placeholder="Поиск по заказам..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            allowClear
            style={{ width: 300 }}
          />

          {/* Кнопка управления колонками */}
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
              value={filters.minAmount ?? null}
              onChange={(val) => handleAmountChange('minAmount', val)}
              className="w-full"
            />
            <InputNumber
              placeholder="До суммы"
              value={filters.maxAmount ?? null}
              onChange={(val) => handleAmountChange('maxAmount', val)}
              className="w-full"
            />
          </div>

          <Select
            mode="multiple"
            placeholder="Статус"
            value={filters.status}
            onChange={(value) => setFilters((prev) => ({ ...prev, status: value }))}
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
            value={filters.paymentMethod}
            onChange={(value) => setFilters((prev) => ({ ...prev, paymentMethod: value }))}
            allowClear
            className="w-full"
          >
            <Option value="card">Карта</Option>
            <Option value="paypal">PayPal</Option>
            <Option value="cash">Наличные</Option>
            <Option value="bank_transfer">Банковский перевод</Option>
          </Select>
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
          showQuickJumper
          showTotal={(total) => `Всего: ${total} заказов`}
        />
      </div>

      {/* Таблица */}
      <Table
        dataSource={orders}
        columns={columns}
        rowKey="id"
        loading={loading}
        pagination={false}
        onChange={handleTableChange}
        scroll={{ x: 'max-content' }}
      />
    </div>
  );
};

export default UserHome;