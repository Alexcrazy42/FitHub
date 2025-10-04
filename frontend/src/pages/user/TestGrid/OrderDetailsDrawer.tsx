// components/OrderDetailsDrawer.tsx
import React from 'react';
import {
  Drawer,
  Descriptions,
  Tag,
  Space,
  Badge,
  Button,
  Typography,
} from 'antd';
import dayjs from 'dayjs';
import { useNavigate } from 'react-router';
import { Order } from './Home';
import { paymentMethodLabels } from './paymentMethodLabels';
import { statusLabels } from './statusLabels';

const { Text } = Typography;

interface OrderDetailsDrawerProps {
  selectedOrder: Order | null;
  drawerVisible: boolean;
  onClose: () => void;
}

export const OrderDetailsDrawer: React.FC<OrderDetailsDrawerProps> = ({
  selectedOrder,
  drawerVisible,
  onClose,
}) => {
  const navigate = useNavigate();

  if (!selectedOrder) return null;

  return (
    <Drawer
      title={
        <Space>
          <Text strong>Заказ №{selectedOrder.orderNumber}</Text>
          <Badge
            status={
              selectedOrder.status === 'delivered'
                ? 'success'
                : selectedOrder.status === 'cancelled'
                ? 'error'
                : 'processing'
            }
            text={statusLabels[selectedOrder.status]}
          />
        </Space>
      }
      width={600}
      onClose={onClose}
      open={drawerVisible}
      destroyOnClose
    >
      <div className="space-y-6">
        {/* Основная информация */}
        <Descriptions
          title="Общая информация"
          column={1}
          bordered
          size="small"
        >
          <Descriptions.Item label="Номер заказа">
            {selectedOrder.orderNumber}
          </Descriptions.Item>
          <Descriptions.Item label="Статус">
            <Tag
              color={
                selectedOrder.status === 'delivered'
                  ? 'green'
                  : selectedOrder.status === 'cancelled'
                  ? 'red'
                  : selectedOrder.status === 'pending'
                  ? 'orange'
                  : 'blue'
              }
            >
              {statusLabels[selectedOrder.status]}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="Сумма заказа">
            <Text strong>
              {selectedOrder.totalAmount} {selectedOrder.currency}
            </Text>
          </Descriptions.Item>
          <Descriptions.Item label="Количество товаров">
            {selectedOrder.itemsCount} шт.
          </Descriptions.Item>
          <Descriptions.Item label="Дата создания">
            {dayjs(selectedOrder.createdAt).format('DD.MM.YYYY HH:mm:ss')}
          </Descriptions.Item>
          <Descriptions.Item label="Последнее обновление">
            {dayjs(selectedOrder.updatedAt).format('DD.MM.YYYY HH:mm:ss')}
          </Descriptions.Item>
        </Descriptions>

        {/* Контактная информация */}
        <Descriptions title="Контакты" column={1} bordered size="small">
          <Descriptions.Item label="Покупатель">
            {selectedOrder.customerName}
          </Descriptions.Item>
          <Descriptions.Item label="Email">
            <a href={`mailto:${selectedOrder.email}`}>{selectedOrder.email}</a>
          </Descriptions.Item>
          <Descriptions.Item label="Телефон">
            <a href={`tel:${selectedOrder.phone}`}>{selectedOrder.phone}</a>
          </Descriptions.Item>
        </Descriptions>

        {/* Доставка и оплата */}
        <Descriptions title="Доставка и оплата" column={1} bordered size="small">
          <Descriptions.Item label="Адрес доставки">
            {selectedOrder.shippingAddress}
          </Descriptions.Item>
          <Descriptions.Item label="Способ оплаты">
            {paymentMethodLabels[selectedOrder.paymentMethod]}
          </Descriptions.Item>
        </Descriptions>

        {/* Примечания */}
        {selectedOrder.notes && (
          <Descriptions title="Примечания" column={1} bordered size="small">
            <Descriptions.Item label="Комментарий">
              {selectedOrder.notes}
            </Descriptions.Item>
          </Descriptions>
        )}

        {/* Дополнительные метаданные */}
        <Descriptions title="Техническая информация" column={1} bordered size="small">
          <Descriptions.Item label="ID заказа (внутренний)">
            <Text copyable>{selectedOrder.id}</Text>
          </Descriptions.Item>
          <Descriptions.Item label="Валюта">
            {selectedOrder.currency}
          </Descriptions.Item>
        </Descriptions>

        <div className="mt-6 pt-4 border-t">
          <Button
            type="primary"
            block
            onClick={() => {
              onClose();
              navigate(`/user/home/${selectedOrder.id}/order`);
            }}
          >
            Перейти к заказу
          </Button>
        </div>
      </div>
    </Drawer>
  );
};