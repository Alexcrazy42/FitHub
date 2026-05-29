import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Alert,
  Button,
  Card,
  Checkbox,
  Col,
  Divider,
  Empty,
  Image,
  Input,
  InputNumber,
  Pagination,
  Row,
  Select,
  Skeleton,
  Space,
  Tag,
  Typography,
} from 'antd';
import { ShoppingCartOutlined } from '@ant-design/icons';
import { useApiService } from '../../../api/useApiService';
import { createMarketplaceApi } from '../../../api/services/marketplaceService';
import {
  MarketplaceCatalogSearchRequest,
  MarketplaceCategoryFacetValue,
  MarketplaceFacet,
  MarketplaceProductListItem,
} from '../../../types/marketplace';

const { Title, Text } = Typography;

const PAGE_SIZE = 12;
const FACET_PARAM_PREFIX = 'facet.';

const sortOptions = [
  { label: 'Популярные', value: 'popular' },
  { label: 'Новые', value: 'newest' },
  { label: 'Сначала дешевле', value: 'price_asc' },
  { label: 'Сначала дороже', value: 'price_desc' },
];

const formatMoney = (amount: number, currency: string) =>
  new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount);

const getNumberParam = (searchParams: URLSearchParams, key: string): number | null => {
  const value = searchParams.get(key);
  if (!value) {
    return null;
  }

  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : null;
};

const getFacetParams = (searchParams: URLSearchParams): Record<string, string[]> => {
  const facets: Record<string, string[]> = {};

  searchParams.forEach((value, key) => {
    if (!key.startsWith(FACET_PARAM_PREFIX)) {
      return;
    }

    const attributeDefinitionId = key.slice(FACET_PARAM_PREFIX.length);
    const optionIds = value
      .split(',')
      .map((optionId) => optionId.trim())
      .filter(Boolean);

    if (attributeDefinitionId && optionIds.length > 0) {
      facets[attributeDefinitionId] = optionIds;
    }
  });

  return facets;
};

export const MarketplaceCatalogPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const apiService = useApiService();
  const marketplaceApi = useMemo(() => createMarketplaceApi(apiService), [apiService]);

  const [items, setItems] = useState<MarketplaceProductListItem[]>([]);
  const [categories, setCategories] = useState<MarketplaceCategoryFacetValue[]>([]);
  const [facets, setFacets] = useState<MarketplaceFacet[]>([]);
  const [productCount, setProductCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reloadKey, setReloadKey] = useState(0);

  const searchText = searchParams.get('q') ?? '';
  const [localSearchText, setLocalSearchText] = useState(searchText);
  const sort = searchParams.get('sort') ?? 'popular';
  const pageNumber = Math.max(Number(searchParams.get('page') ?? '1'), 1);
  const categoryId = searchParams.get('categoryId');
  const minPrice = getNumberParam(searchParams, 'minPrice');
  const maxPrice = getNumberParam(searchParams, 'maxPrice');
  const inStock = searchParams.get('inStock') === 'true';
  const selectedFacets = useMemo(() => getFacetParams(searchParams), [searchParams]);

  const updateParams = useCallback((values: Record<string, string | number | boolean | null | undefined>) => {
    const next = new URLSearchParams(searchParams);

    Object.entries(values).forEach(([key, value]) => {
      if (value === null || value === undefined || value === '' || value === false) {
        next.delete(key);
        return;
      }

      next.set(key, String(value));
    });

    setSearchParams(next);
  }, [searchParams, setSearchParams]);

  const toggleFacet = useCallback((attributeDefinitionId: string, attributeOptionId: string) => {
    const selectedOptions = new Set(selectedFacets[attributeDefinitionId] ?? []);

    if (selectedOptions.has(attributeOptionId)) {
      selectedOptions.delete(attributeOptionId);
    } else {
      selectedOptions.add(attributeOptionId);
    }

    updateParams({
      [`${FACET_PARAM_PREFIX}${attributeDefinitionId}`]: Array.from(selectedOptions).join(','),
      page: 1,
    });
  }, [selectedFacets, updateParams]);

  const resetFilters = useCallback(() => {
    const next = new URLSearchParams(searchParams);
    next.delete('q');
    next.delete('categoryId');
    next.delete('minPrice');
    next.delete('maxPrice');
    next.delete('inStock');
    next.delete('page');

    Array.from(next.keys())
      .filter((key) => key.startsWith(FACET_PARAM_PREFIX))
      .forEach((key) => next.delete(key));

    setSearchParams(next);
  }, [searchParams, setSearchParams]);

  useEffect(() => {
    setLocalSearchText(searchText);
  }, [searchText]);

  useEffect(() => {
    const timerId = window.setTimeout(() => {
      const normalizedSearchText = localSearchText.trim();

      if (normalizedSearchText === searchText) {
        return;
      }

      updateParams({ q: normalizedSearchText, page: 1 });
    }, 400);

    return () => window.clearTimeout(timerId);
  }, [localSearchText, searchText, updateParams]);

  useEffect(() => {
    let isCurrent = true;

    const request: MarketplaceCatalogSearchRequest = {
      categoryId,
      searchText: searchText || null,
      minPrice,
      maxPrice,
      inStock: inStock || null,
      facets: Object.keys(selectedFacets).length > 0 ? selectedFacets : null,
      sort,
      pageNumber,
      pageSize: PAGE_SIZE,
    };

    const loadProducts = async () => {
      setLoading(true);
      setError(null);

      const response = await marketplaceApi.searchProducts(request);

      if (!isCurrent) {
        return;
      }

      if (!response.success || !response.data) {
        setItems([]);
        setCategories([]);
        setFacets([]);
        setProductCount(0);
        setError(response.error?.detail ?? 'Не удалось загрузить каталог.');
        setLoading(false);
        return;
      }

      setItems(response.data.items);
      setCategories(response.data.categories);
      setFacets(response.data.facets);
      setProductCount(response.data.productCount);
      setLoading(false);
    };

    loadProducts();

    return () => {
      isCurrent = false;
    };
  }, [marketplaceApi, categoryId, searchText, minPrice, maxPrice, inStock, selectedFacets, sort, pageNumber, reloadKey]);

  const openProduct = (productId: string) => {
    navigate(`/gym-admin/marketplace/products/${productId}`);
  };

  return (
    <div className="container mx-auto p-6">
      <Space direction="vertical" size="large" className="w-full">
        <div>
          <Title level={2}>Магазин</Title>
          <Text type="secondary">Каталог товаров для зала.</Text>
        </div>

        <Space wrap size="middle" className="w-full">
          <Input
            allowClear
            placeholder="Название товара"
            value={localSearchText}
            onChange={(event) => setLocalSearchText(event.target.value)}
            style={{ width: 280 }}
          />
          <Select
            value={sort}
            options={sortOptions}
            onChange={(value) => updateParams({ sort: value, page: 1 })}
            style={{ width: 180 }}
          />
          <InputNumber
            min={0}
            placeholder="Цена от"
            value={minPrice}
            onChange={(value) => updateParams({ minPrice: value, page: 1 })}
          />
          <InputNumber
            min={0}
            placeholder="Цена до"
            value={maxPrice}
            onChange={(value) => updateParams({ maxPrice: value, page: 1 })}
          />
          <Checkbox
            checked={inStock}
            onChange={(event) => updateParams({ inStock: event.target.checked, page: 1 })}
          >
            В наличии
          </Checkbox>
          <Button onClick={resetFilters}>Сбросить</Button>
        </Space>

        {error && (
          <Alert
            type="error"
            message="Каталог недоступен"
            description={error}
            action={<Button onClick={() => setReloadKey((value) => value + 1)}>Повторить</Button>}
          />
        )}

        <Row gutter={[24, 24]} align="top">
          <Col xs={24} lg={6}>
            <Space direction="vertical" size="middle" className="w-full">
              <Title level={4}>Фильтры</Title>
              {categories.length > 0 && (
                <div>
                  <Space direction="vertical" size="small" className="w-full">
                    <Text strong>Категория</Text>
                    {categories.map((category) => {
                      const checked = category.categoryId === categoryId;
                      const disabled = category.count === 0 && !checked;

                      return (
                        <Checkbox
                          key={category.categoryId}
                          checked={checked}
                          disabled={disabled}
                          onChange={() => updateParams({ categoryId: checked ? null : category.categoryId, page: 1 })}
                        >
                          {category.name} <Text type="secondary">({category.count})</Text>
                        </Checkbox>
                      );
                    })}
                  </Space>
                  <Divider />
                </div>
              )}

              {facets.length > 0 ? (
                facets.map((facet) => (
                  <div key={facet.attributeDefinitionId}>
                    <Space direction="vertical" size="small" className="w-full">
                      <Text strong>{facet.name}</Text>
                      {facet.values.map((value) => {
                        const checked = selectedFacets[facet.attributeDefinitionId]?.includes(value.attributeOptionId) ?? false;
                        const disabled = value.count === 0 && !checked;

                        return (
                          <Checkbox
                            key={value.attributeOptionId}
                            checked={checked}
                            disabled={disabled}
                            onChange={() => toggleFacet(facet.attributeDefinitionId, value.attributeOptionId)}
                          >
                            {value.value} <Text type="secondary">({value.count})</Text>
                          </Checkbox>
                        );
                      })}
                    </Space>
                    <Divider />
                  </div>
                ))
              ) : (
                <Text type="secondary">Фасеты появятся после загрузки каталога.</Text>
              )}
            </Space>
          </Col>

          <Col xs={24} lg={18}>
            {loading ? (
              <Row gutter={[16, 16]}>
                {Array.from({ length: 6 }, (_, index) => (
                  <Col xs={24} sm={12} xl={8} key={index}>
                    <Card>
                      <Skeleton active />
                    </Card>
                  </Col>
                ))}
              </Row>
            ) : items.length > 0 ? (
              <Row gutter={[16, 16]}>
                {items.map((product) => (
                  <Col xs={24} sm={12} xl={8} key={product.id}>
                    <Card
                      hoverable
                      onClick={() => openProduct(product.id)}
                      cover={
                        product.mainImage ? (
                          <Image
                            preview={false}
                            src={product.mainImage.url}
                            alt={product.mainImage.altText ?? product.name}
                            height={180}
                            style={{ objectFit: 'cover' }}
                          />
                        ) : (
                          <div className="flex h-[180px] items-center justify-center bg-gray-100">
                            <ShoppingCartOutlined className="text-4xl text-gray-400" />
                          </div>
                        )
                      }
                    >
                      <Space direction="vertical" size="small" className="w-full">
                        <Text strong>{product.name}</Text>
                        {product.brandName && <Text type="secondary">{product.brandName}</Text>}
                        <Text>{formatMoney(product.priceFrom.amount, product.priceFrom.currency)}</Text>
                        <Tag color={product.isAvailable ? 'green' : 'default'}>
                          {product.isAvailable ? 'В наличии' : 'Нет в наличии'}
                        </Tag>
                      </Space>
                    </Card>
                  </Col>
                ))}
              </Row>
            ) : (
              <Empty
                image={<ShoppingCartOutlined className="text-4xl text-gray-400" />}
                description="Товаров по этим условиям нет."
              />
            )}
          </Col>
        </Row>

        {productCount > PAGE_SIZE && (
          <Pagination
            current={pageNumber}
            pageSize={PAGE_SIZE}
            total={productCount}
            showSizeChanger={false}
            onChange={(page) => updateParams({ page })}
          />
        )}
      </Space>
    </div>
  );
};
