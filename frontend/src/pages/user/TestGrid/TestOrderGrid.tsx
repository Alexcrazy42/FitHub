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
  Empty,
  message
} from 'antd';
import { SettingOutlined, SortAscendingOutlined } from '@ant-design/icons';
import { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { OrderDetailsDrawer } from './OrderDetailsDrawer';
import { orderStatusLabels } from './orderStatusLabels';
import { paymentMethodLabels } from './paymentMethodLabels';
import { useApiService } from '../../../api/useApiService';

const { Option } = Select;

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

export type Currency = 'USD' | 'EUR' | 'RUB';

export type PaymentMethod = 'Card' | 'Paypal' | 'Cash' | 'BankTransfer';

export interface Order {
  id: string;
  orderNumber: string;
  customerName: string;
  email: string;
  phone: string;
  status: OrderStatus;
  totalAmount: number;
  currency: Currency;
  createdAt: string;
  updatedAt: string;
  shippingAddress: string;
  paymentMethod: PaymentMethod;
  itemsCount: number;
  notes: string;
}

// Типы для API ответа
interface OrdersResponse {
  data: Order[];
  total: number;
  page: number;
  pageSize: number;
}

type OrderFilters = {
  status?: OrderStatus[];
  paymentMethod?: PaymentMethod[];
  minAmount?: number;
  maxAmount?: number;
};

type Direction = 'Ascending' | 'Descending';

export const Directions = {
  Asc: 'Ascending' as Direction,
  Desc: 'Descending' as Direction,
} as const;

export const DirectionLabels = {
  [Directions.Asc]: 'По возрастанию',
  [Directions.Desc]: 'По убыванию',
} as const;

type SortConfig = {
  field: keyof Order;
  direction: Direction;
};

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

const TestOrderGrid: React.FC = () => {
  const apiService = useApiService();
  const [orders, setOrders] = useState<Order[]>([]);
  const [gridLoading, setGridLoading] = useState(false);

  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const [tempSearchQuery, setTempSearchQuery] = useState('');
  const [appliedSearchQuery, setAppliedSearchQuery] = useState('');
  
  const [tempFilters, setTempFilters] = useState<OrderFilters>({});
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
    direction: 'Ascending'
  });
  const [appliedSort, setAppliedSort] = useState<SortConfig>({
    field: 'createdAt',
    direction: 'Descending'
  });

  const handleRowClick = (order: Order) => {
    setSelectedOrder(order);
    setDrawerVisible(true);
  };

  const closeDrawer = () => {
    setDrawerVisible(false);
    setSelectedOrder(null);
  };

  const loadOrders = async () => {
    setGridLoading(true);
    
    try {
      // Создаем URLSearchParams вручную
      const searchParams = new URLSearchParams();
      
      // Базовые параметры
      searchParams.append('page', page.toString());
      searchParams.append('pageSize', pageSize.toString());
      searchParams.append('sortBy', appliedSort.field);
      searchParams.append('sortOrder', appliedSort.direction);

      // Поисковый запрос (используем ПРИМЕНЕННЫЙ поиск)
      if (appliedSearchQuery) {
        searchParams.append('q', appliedSearchQuery);
      }

      // Статусы (повторяем параметр для каждого значения)
      if (appliedFilters.status && appliedFilters.status.length > 0) {
        appliedFilters.status.forEach(status => {
          searchParams.append('status', status.charAt(0).toUpperCase() + status.slice(1));
        });
      }

      // Способы оплаты (повторяем параметр для каждого значения)
      if (appliedFilters.paymentMethod && appliedFilters.paymentMethod.length > 0) {
        appliedFilters.paymentMethod.forEach(method => {
          searchParams.append('paymentMethod', method);
        });
      }

      // Суммы
      if (appliedFilters.minAmount !== undefined) {
        searchParams.append('minAmount', appliedFilters.minAmount.toString());
      }
      if (appliedFilters.maxAmount !== undefined) {
        searchParams.append('maxAmount', appliedFilters.maxAmount.toString());
      }

      console.log('Final URL:', `/Test?${searchParams.toString()}`);

      // Вызов API с уже сериализованными параметрами
      const response = await apiService.get<OrdersResponse>(`/Test?${searchParams.toString()}`);

      if (response.success && response.data) {
        setOrders(response.data.data);
        setTotal(response.data.total);
      } else {
        message.error(response.error?.detail || 'Ошибка при загрузке заказов');
        setOrders([]);
        setTotal(0);
      }
    } catch (error) {
      console.error('Error loading orders:', error);
      message.error('Произошла ошибка при загрузке данных');
      setOrders([]);
      setTotal(0);
    } finally {
      setGridLoading(false);
    }
  };

  // 🔥 ИСПРАВЛЕНИЕ: Загружаем данные только при изменении ПРИМЕНЕННЫХ параметров
  useEffect(() => {
    loadOrders();
  }, [page, pageSize, appliedSort, appliedFilters, appliedSearchQuery]);

  const toggleColumn = (key: keyof Order) => {
    setVisibleColumns((prev) => ({ ...prev, [key]: !prev[key] }));
  };

  const handleAmountChange = (field: 'minAmount' | 'maxAmount', value: number | null) => {
    setTempFilters((prev) => ({ ...prev, [field]: value ?? undefined }));
  };

  const applyFiltersAndSort = () => {
    setAppliedSort(tempSort);
    setAppliedFilters(tempFilters);
    setAppliedSearchQuery(tempSearchQuery);
    setPage(1);
  };

  const resetFilters = () => {
    setTempSearchQuery('');
    setAppliedSearchQuery('');
    setTempFilters({});
    setAppliedFilters({});
    setPage(1);
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
            Pending: 'orange',
            Confirmed: 'blue',
            Shipped: 'geekblue',
            Delivered: 'green',
            Cancelled: 'red',
          };
          return <Tag color={colorMap[status]}>{orderStatusLabels[status]}</Tag>;
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
        render: (notes: string) => notes || '-',
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
            value={tempSearchQuery} // 🔥 Используем временное значение
            onChange={(e) => setTempSearchQuery(e.target.value)} // 🔥 Только обновляем временное состояние
            onPressEnter={applyFiltersAndSort} // Поиск по Enter
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
              value={tempFilters.minAmount ?? null} // 🔥 Используем временное значение
              onChange={(val) => handleAmountChange('minAmount', val)} // 🔥 Только обновляем временное состояние
              className="w-full"
            />
            <InputNumber
              placeholder="До суммы"
              value={tempFilters.maxAmount ?? null} // 🔥 Используем временное значение
              onChange={(val) => handleAmountChange('maxAmount', val)} // 🔥 Только обновляем временное состояние
              className="w-full"
            />
          </div>

          <Select
            mode="multiple"
            placeholder="Статус"
            value={tempFilters.status} // 🔥 Используем временное значение
            onChange={(value) => setTempFilters((prev) => ({ ...prev, status: value }))} // 🔥 Только обновляем временное состояние
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
            value={tempFilters.paymentMethod} // 🔥 Используем временное значение
            onChange={(value) => setTempFilters((prev) => ({ ...prev, paymentMethod: value }))} // 🔥 Только обновляем временное состояние
            allowClear
            className="w-full"
          >
            <Option value="Card">банковская карта</Option>
            <Option value="Paypal">PayPal</Option>
            <Option value="Cash">Наличные</Option>
            <Option value="BankTransfer">Банковский перевод</Option>
          </Select>

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
              onChange={(value) => {
                setTempSort(prev => ({ ...prev, direction: value }))}
              }
              className="w-32"
            >
              {/* <div value="123">123</div> value на div Работает, то есть можно вообще в любой это прокидывать*/} 
              <Option value={Directions.Asc}>
                {DirectionLabels[Directions.Asc]}
              </Option>
              <Option value={Directions.Desc}>
                {DirectionLabels[Directions.Desc]}
              </Option>
            </Select>

            
          </div>
        </div>

        {/* <Select>
              <OptGroup label="Основные">
                <Option value="asc">▲ По возрастанию</Option>
                <Option value="desc">▼ По убыванию</Option>
              </OptGroup>
              <OptGroup label="Дополнительные">
                <Option value="random">🎲 Случайно</Option>
                <Option value="popular">🔥 Популярные</Option>
              </OptGroup>
            </Select> */}

        {/* Кнопки управления */}
        <div className="mt-4 flex justify-end gap-2">
          <Button
            onClick={resetFilters}
          >
            Сбросить
          </Button>
          <Button
            type="primary"
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
        loading={gridLoading}
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
                    onClick={resetFilters}
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

export default TestOrderGrid;