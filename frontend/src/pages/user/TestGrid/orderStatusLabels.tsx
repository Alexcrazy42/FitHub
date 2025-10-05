import { OrderStatus } from "./TestOrderGrid";

export const orderStatusLabels: Record<OrderStatus, string> = {
  Pending: 'В ожидании',
  Confirmed: 'Подтверждён',
  Shipped: 'Отправлен',
  Delivered: 'Доставлен',
  Cancelled: 'Отменён',
};
