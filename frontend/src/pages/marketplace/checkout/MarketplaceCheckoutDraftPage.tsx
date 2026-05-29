import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Alert,
  Button,
  Card,
  Col,
  Divider,
  Image,
  Result,
  Row,
  Skeleton,
  Space,
  Spin,
  Statistic,
  Tag,
  Typography,
} from 'antd';
import { CheckCircleOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import { CheckoutReservation, MarketplacePaymentIntent } from '../../../types/marketplace';

const { Paragraph, Text, Title } = Typography;

const formatMoney = (amount: number, currency: string) =>
  new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);

const getRemainingSeconds = (expiresAt: string) => {
  const expiresAtMs = new Date(expiresAt).getTime();
  const diffMs = expiresAtMs - Date.now();
  return Math.max(0, Math.ceil(diffMs / 1000));
};

const formatTimer = (seconds: number) => {
  const minutes = Math.floor(seconds / 60).toString().padStart(2, '0');
  const restSeconds = (seconds % 60).toString().padStart(2, '0');
  return `${minutes}:${restSeconds}`;
};

export const MarketplaceCheckoutDraftPage: React.FC = () => {
  const { reservationId } = useParams();
  const navigate = useNavigate();
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [reservation, setReservation] = useState<CheckoutReservation | null>(null);
  const [payment, setPayment] = useState<MarketplacePaymentIntent | null>(null);
  const [remainingSeconds, setRemainingSeconds] = useState(0);
  const [loading, setLoading] = useState(false);
  const [paymentPending, setPaymentPending] = useState(false);
  const [paymentChecking, setPaymentChecking] = useState(false);
  const [successOrderId, setSuccessOrderId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [paymentError, setPaymentError] = useState<string | null>(null);

  useEffect(() => {
    let isCurrent = true;

    const loadReservation = async () => {
      if (!reservationId) {
        setError('Не найден идентификатор резерва.');
        return;
      }

      setLoading(true);
      setError(null);

      const response = await marketplaceApi.getReservation(reservationId);

      if (!isCurrent) {
        return;
      }

      if (!response.success || !response.data) {
        setReservation(null);
        setError(response.error?.detail ?? 'Не удалось загрузить резерв.');
        setLoading(false);
        return;
      }

      setReservation(response.data);
      setRemainingSeconds(getRemainingSeconds(response.data.expiresAt));
      setLoading(false);
    };

    loadReservation();

    return () => {
      isCurrent = false;
    };
  }, [marketplaceApi, reservationId]);

  useEffect(() => {
    if (!reservation || reservation.status !== 'Active') {
      return;
    }

    const intervalId = window.setInterval(() => {
      setRemainingSeconds(getRemainingSeconds(reservation.expiresAt));
    }, 1000);

    return () => window.clearInterval(intervalId);
  }, [reservation]);

  useEffect(() => {
    if (!payment || !reservationId || successOrderId) {
      return;
    }

    if (payment.paymentStatus === 'Paid' && payment.order) {
      setSuccessOrderId(payment.order.orderId);
      return;
    }

    if (payment.paymentStatus === 'Failed' || payment.paymentStatus === 'Expired') {
      return;
    }

    let isCurrent = true;

    const pollPayment = async () => {
      setPaymentChecking(true);
      const response = await marketplaceApi.createPaymentIntent(reservationId);

      if (!isCurrent) {
        return;
      }

      setPaymentChecking(false);

      if (!response.success || !response.data) {
        setPaymentError(response.error?.detail ?? 'Не удалось обновить статус оплаты.');
        return;
      }

      setPayment(response.data);
      setReservation(response.data.reservation);

      if (response.data.order) {
        setSuccessOrderId(response.data.order.orderId);
      }
    };

    const intervalId = window.setInterval(pollPayment, 3000);

    return () => {
      isCurrent = false;
      window.clearInterval(intervalId);
    };
  }, [marketplaceApi, payment, reservationId, successOrderId]);

  useEffect(() => {
    if (!successOrderId) {
      return;
    }

    const timeoutId = window.setTimeout(() => {
      navigate(`/gym-admin/marketplace/orders/${successOrderId}`);
    }, 1800);

    return () => window.clearTimeout(timeoutId);
  }, [navigate, successOrderId]);

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
          title="Резерв недоступен"
          subTitle={error}
          extra={<Button onClick={() => navigate('/gym-admin/marketplace/catalog')}>Вернуться в каталог</Button>}
        />
      </div>
    );
  }

  if (successOrderId) {
    return (
      <div className="container mx-auto p-6">
        <style>
          {`
            .marketplace-success-icon {
              animation: marketplace-success-pop 700ms ease-out both, marketplace-success-glow 1300ms ease-in-out infinite;
              color: #16a34a;
              font-size: 76px;
            }

            @keyframes marketplace-success-pop {
              0% { transform: scale(0.4); opacity: 0; }
              70% { transform: scale(1.12); opacity: 1; }
              100% { transform: scale(1); opacity: 1; }
            }

            @keyframes marketplace-success-glow {
              0%, 100% { filter: drop-shadow(0 0 0 rgba(22, 163, 74, 0)); }
              50% { filter: drop-shadow(0 0 18px rgba(22, 163, 74, 0.45)); }
            }
          `}
        </style>
        <Result
          icon={<CheckCircleOutlined className="marketplace-success-icon" />}
          status="success"
          title="Оплата подтверждена"
          subTitle="Заказ создан. Открываем статус заказа."
        />
      </div>
    );
  }

  if (!reservation) {
    return (
      <div className="container mx-auto p-6">
        <Result
          status="warning"
          title="Резерв не найден"
          extra={<Button onClick={() => navigate('/gym-admin/marketplace/catalog')}>Вернуться в каталог</Button>}
        />
      </div>
    );
  }

  const isExpired = reservation.status !== 'Active' || remainingSeconds <= 0;
  const item = reservation.item;
  const totalAmount = item ? item.price.amount * reservation.quantity : 0;
  const paymentStatus = payment?.paymentStatus ?? (isExpired ? 'Expired' : 'AwaitingPayment');

  const handleCreatePaymentIntent = async () => {
    if (!reservation || isExpired || paymentPending) {
      return;
    }

    setPaymentPending(true);
    setPaymentError(null);

    const response = await marketplaceApi.createPaymentIntent(reservation.reservationId);

    setPaymentPending(false);

    if (!response.success || !response.data) {
      setPaymentError(response.error?.detail ?? 'Не удалось подготовить оплату.');
      return;
    }

    setPayment(response.data);
    setReservation(response.data.reservation);

    if (response.data.order) {
      setSuccessOrderId(response.data.order.orderId);
    }
  };

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Оформление заказа</Title>
          <Text type="secondary">Товар закреплен за вами на ограниченное время.</Text>
        </div>

        {isExpired ? (
          <Alert
            type="warning"
            showIcon
            message="Резерв истек"
            description="Создайте новый резерв из карточки товара, чтобы продолжить оформление."
          />
        ) : (
          <Alert
            type="success"
            showIcon
            message="Резерв активен"
            description="Можно переходить к оплате, пока таймер не закончился."
          />
        )}

        {paymentError && (
          <Alert type="error" showIcon message={paymentError} />
        )}

        <Row gutter={[24, 24]} align="stretch">
          <Col xs={24} lg={15}>
            <Card>
              {item ? (
                <Row gutter={[20, 20]}>
                  <Col xs={24} md={9}>
                    {item.image ? (
                      <Image
                        src={item.image.url}
                        alt={item.image.altText ?? item.productName}
                        width="100%"
                        height={260}
                        style={{ objectFit: 'cover' }}
                      />
                    ) : (
                      <div className="flex h-[260px] items-center justify-center bg-gray-100">
                        <ShoppingCartOutlined className="text-5xl text-gray-400" />
                      </div>
                    )}
                  </Col>
                  <Col xs={24} md={15}>
                    <Space direction="vertical" size="middle" className="w-full">
                      <Space direction="vertical" size={2}>
                        {item.brandName && <Text type="secondary">{item.brandName}</Text>}
                        <Title level={3} className="!mb-0">{item.productName}</Title>
                        <Text type="secondary">{item.variantName ?? item.sku}</Text>
                      </Space>

                      {item.attributes.length > 0 && (
                        <Space wrap>
                          {item.attributes.map((attribute) => (
                            <Tag key={`${attribute.attributeDefinitionId}-${attribute.attributeOptionId}`}>
                              {attribute.name}: {attribute.value}
                            </Tag>
                          ))}
                        </Space>
                      )}

                      <Space direction="vertical" size={4}>
                        <Text type="secondary">Цена за единицу</Text>
                        <Title level={3} className="!mb-0">
                          {formatMoney(item.price.amount, item.price.currency)}
                        </Title>
                      </Space>
                    </Space>
                  </Col>
                </Row>
              ) : (
                <Result
                  status="info"
                  title="Резерв создан"
                  subTitle="Данные товара пока недоступны. Обновите страницу или вернитесь в каталог."
                />
              )}
            </Card>
          </Col>

          <Col xs={24} lg={9}>
            <Card>
              <Space direction="vertical" size="large" className="w-full">
                <Space direction="vertical" size="small" className="w-full">
                  <Statistic title="Осталось времени" value={formatTimer(remainingSeconds)} />
                  <Tag color={isExpired ? 'default' : 'green'}>{isExpired ? 'Резерв истек' : 'Резерв активен'}</Tag>
                  <Tag color={paymentStatus === 'Paid' ? 'green' : paymentStatus === 'Failed' ? 'red' : 'blue'}>
                    Оплата: {paymentStatus}
                  </Tag>
                </Space>

                <Divider className="!my-0" />

                <Space direction="vertical" size="small" className="w-full">
                  <div className="flex items-center justify-between gap-4">
                    <Text type="secondary">Количество</Text>
                    <Text strong>{reservation.quantity}</Text>
                  </div>
                  {item && (
                    <div className="flex items-center justify-between gap-4">
                      <Text type="secondary">Итого</Text>
                      <Text strong>{formatMoney(totalAmount, item.price.currency)}</Text>
                    </div>
                  )}
                </Space>

                <Divider className="!my-0" />

                <Space direction="vertical" size="small" className="w-full">
                  {!payment && (
                    <Button
                      type="primary"
                      size="large"
                      loading={paymentPending}
                      disabled={isExpired || paymentPending}
                      onClick={handleCreatePaymentIntent}
                      block
                    >
                      Подготовить оплату
                    </Button>
                  )}
                  {payment && payment.paymentStatus !== 'Paid' && payment.paymentStatus !== 'Failed' && (
                    <Alert
                      type="info"
                      showIcon
                      icon={<Spin size="small" />}
                      message="Оплата обрабатывается"
                      description={paymentChecking ? 'Проверяем статус оплаты.' : 'Статус обновится автоматически.'}
                    />
                  )}
                  {payment?.paymentStatus === 'Paid' && (
                    <Alert type="success" showIcon message="Оплата подтверждена" />
                  )}
                  {payment?.paymentStatus === 'Failed' && (
                    <Alert
                      type="warning"
                      showIcon
                      message="Оплата отклонена"
                      description={payment.failureReason ?? 'Создайте новый резерв, чтобы попробовать снова.'}
                    />
                  )}
                  <Button onClick={() => navigate('/gym-admin/marketplace/catalog')} block>
                    Вернуться в каталог
                  </Button>
                </Space>
              </Space>
            </Card>
          </Col>
        </Row>
      </Space>
    </div>
  );
};
