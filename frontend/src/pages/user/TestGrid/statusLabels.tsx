import { Order } from "./Home";

export const statusLabels: Record<Order['status'], string> = {
  pending: 'В ожидании',
  confirmed: 'Подтверждён',
  shipped: 'Отправлен',
  delivered: 'Доставлен',
  cancelled: 'Отменён',
};
