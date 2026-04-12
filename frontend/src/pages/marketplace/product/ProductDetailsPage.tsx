import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Alert,
  Button,
  Card,
  Col,
  Descriptions,
  Empty,
  Image,
  Row,
  Skeleton,
  Space,
  Tag,
  Typography,
} from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import {
  MarketplaceProductDetails,
  MarketplaceProductImage,
  MarketplaceProductVariant,
} from '../../../types/marketplace';

const { Title, Text, Paragraph } = Typography;

const formatMoney = (amount: number, currency: string) =>
  new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);

type AttributeGroup = {
  attributeDefinitionId: string;
  name: string;
  values: { attributeOptionId: string; value: string }[];
};

const getMainImage = (images: MarketplaceProductImage[]) =>
  images.find((image) => image.isMain) ?? images[0] ?? null;

const buildAttributeGroups = (variants: MarketplaceProductVariant[]): AttributeGroup[] => {
  const groups = new Map<string, AttributeGroup>();

  variants.flatMap((variant) => variant.attributes).forEach((attribute) => {
    if (!groups.has(attribute.attributeDefinitionId)) {
      groups.set(attribute.attributeDefinitionId, {
        attributeDefinitionId: attribute.attributeDefinitionId,
        name: attribute.name,
        values: [],
      });
    }

    const group = groups.get(attribute.attributeDefinitionId)!;

    if (!group.values.some((value) => value.attributeOptionId === attribute.attributeOptionId)) {
      group.values.push({
        attributeOptionId: attribute.attributeOptionId,
        value: attribute.value,
      });
    }
  });

  return Array.from(groups.values());
};

const variantMatchesSelection = (
  variant: MarketplaceProductVariant,
  selectedOptions: Record<string, string>
) => {
  return Object.entries(selectedOptions).every(([attributeDefinitionId, attributeOptionId]) =>
    variant.attributes.some((attribute) =>
      attribute.attributeDefinitionId === attributeDefinitionId &&
      attribute.attributeOptionId === attributeOptionId));
};

export const ProductDetailsPage: React.FC = () => {
  const { productId } = useParams();
  const navigate = useNavigate();
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [product, setProduct] = useState<MarketplaceProductDetails | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reservationError, setReservationError] = useState<string | null>(null);
  const [reservationPending, setReservationPending] = useState(false);
  const [selectedImage, setSelectedImage] = useState<MarketplaceProductImage | null>(null);
  const [selectedOptions, setSelectedOptions] = useState<Record<string, string>>({});

  useEffect(() => {
    let isCurrent = true;

    const loadProduct = async () => {
      if (!productId) {
        setError('Не найден идентификатор товара.');
        return;
      }

      setLoading(true);
      setError(null);

      const response = await marketplaceApi.getProduct(productId);

      if (!isCurrent) {
        return;
      }

      if (!response.success || !response.data) {
        setProduct(null);
        setError(response.error?.detail ?? 'Не удалось загрузить товар.');
        setLoading(false);
        return;
      }

      setProduct(response.data);
      setSelectedImage(getMainImage(response.data.images));
      setSelectedOptions({});
      setLoading(false);
    };

    loadProduct();

    return () => {
      isCurrent = false;
    };
  }, [marketplaceApi, productId]);

  const attributeGroups = useMemo(() => buildAttributeGroups(product?.variants ?? []), [product]);
  const selectedVariant = useMemo(() => {
    if (!product || attributeGroups.length === 0) {
      return product?.variants.find((variant) => variant.isAvailable) ?? product?.variants[0] ?? null;
    }

    if (Object.keys(selectedOptions).length !== attributeGroups.length) {
      return null;
    }

    return product.variants.find((variant) => variantMatchesSelection(variant, selectedOptions)) ?? null;
  }, [attributeGroups, product, selectedOptions]);

  const isOptionAvailable = (
    attributeDefinitionId: string,
    attributeOptionId: string
  ) => {
    const nextSelection = {
      ...selectedOptions,
      [attributeDefinitionId]: attributeOptionId,
    };

    return product?.variants.some((variant) =>
      variant.isAvailable &&
      variantMatchesSelection(variant, nextSelection)) ?? false;
  };

  const handleCreateReservation = async () => {
    if (!selectedVariant?.isAvailable || reservationPending) {
      return;
    }

    setReservationPending(true);
    setReservationError(null);

    const idempotencyKey = crypto.randomUUID?.() ?? `${selectedVariant.id}-${Date.now()}`;
    const response = await marketplaceApi.createReservation({
      productVariantId: selectedVariant.id,
      quantity: 1,
      idempotencyKey,
    });

    setReservationPending(false);

    if (!response.success || !response.data) {
      setReservationError(response.error?.detail ?? 'Не удалось зарезервировать товар.');
      return;
    }

    navigate(`/gym-admin/marketplace/checkout/${response.data.reservationId}`);
  };

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
        <Alert type="error" message="Товар недоступен" description={error} />
      </div>
    );
  }

  if (!product) {
    return (
      <div className="container mx-auto p-6">
        <Empty description="Товар не найден." />
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <Row gutter={[24, 24]}>
        <Col xs={24} lg={14}>
          <Space direction="vertical" size="middle" className="w-full">
            <div>
              <Title level={2}>{product.name}</Title>
              {product.brandName && <Text type="secondary">{product.brandName}</Text>}
            </div>

            {selectedImage ? (
              <Image
                src={selectedImage.url}
                alt={selectedImage.altText ?? product.name}
                width="100%"
                height={420}
                style={{ objectFit: 'cover' }}
              />
            ) : (
              <div className="flex h-[420px] items-center justify-center bg-gray-100">
                <ShoppingCartOutlined className="text-5xl text-gray-400" />
              </div>
            )}

            {product.images.length > 1 && (
              <Space wrap>
                {product.images.map((image) => (
                  <Image
                    key={image.fileId}
                    preview={false}
                    src={image.url}
                    alt={image.altText ?? product.name}
                    width={96}
                    height={72}
                    style={{ cursor: 'pointer', objectFit: 'cover' }}
                    onClick={() => setSelectedImage(image)}
                  />
                ))}
              </Space>
            )}

            {product.description && <Paragraph>{product.description}</Paragraph>}
          </Space>
        </Col>

        <Col xs={24} lg={10}>
          <Card>
            <Space direction="vertical" size="large" className="w-full">
              <Space direction="vertical" size="small">
                <Title level={3}>
                  {selectedVariant
                    ? formatMoney(selectedVariant.price.amount, selectedVariant.price.currency)
                    : 'Выберите вариант'}
                </Title>
                {selectedVariant?.compareAtPrice && (
                  <Text delete type="secondary">
                    {formatMoney(selectedVariant.compareAtPrice.amount, selectedVariant.compareAtPrice.currency)}
                  </Text>
                )}
                {selectedVariant && (
                  <Tag color={selectedVariant.isAvailable ? 'green' : 'default'}>
                    {selectedVariant.isAvailable ? 'В наличии' : 'Недоступно'}
                  </Tag>
                )}
              </Space>

              {attributeGroups.map((group) => (
                <Space direction="vertical" size="small" className="w-full" key={group.attributeDefinitionId}>
                  <Text strong>{group.name}</Text>
                  <Space wrap>
                    {group.values.map((value) => {
                      const checked = selectedOptions[group.attributeDefinitionId] === value.attributeOptionId;
                      const available = isOptionAvailable(group.attributeDefinitionId, value.attributeOptionId);

                      return (
                        <Button
                          key={value.attributeOptionId}
                          type={checked ? 'primary' : 'default'}
                          disabled={!available && !checked}
                          onClick={() => setSelectedOptions((current) => ({
                            ...current,
                            [group.attributeDefinitionId]: value.attributeOptionId,
                          }))}
                        >
                          {value.value}
                        </Button>
                      );
                    })}
                  </Space>
                </Space>
              ))}

              {selectedVariant && (
                <Descriptions column={1} size="small">
                  <Descriptions.Item label="SKU">{selectedVariant.sku}</Descriptions.Item>
                  <Descriptions.Item label="Остаток">
                    {selectedVariant.stock?.availableQuantity ?? 0}
                  </Descriptions.Item>
                </Descriptions>
              )}

              {reservationError && (
                <Alert type="error" showIcon message={reservationError} />
              )}

              <Button
                type="primary"
                size="large"
                loading={reservationPending}
                disabled={!selectedVariant?.isAvailable || reservationPending}
                onClick={handleCreateReservation}
                block
              >
                Купить
              </Button>
              {!selectedVariant?.isAvailable && (
                <Text type="secondary">Покупка доступна только для активного варианта с ценой и остатком.</Text>
              )}
            </Space>
          </Card>
        </Col>
      </Row>
    </div>
  );
};
