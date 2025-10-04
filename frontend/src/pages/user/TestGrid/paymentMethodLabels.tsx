import { Order } from './Home';


export const paymentMethodLabels: Record<Order['paymentMethod'], string> = {
  card: 'Карта',
  paypal: 'PayPal',
  cash: 'Наличные',
  bank_transfer: 'Банковский перевод',
};
