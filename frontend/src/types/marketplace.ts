export interface MarketplaceMoney {
  amount: number;
  currency: string;
}

export interface MarketplaceStock {
  quantityOnHand: number;
  quantityReserved: number;
  availableQuantity: number;
}

export interface MarketplaceProductImage {
  fileId: string;
  url: string;
  altText: string | null;
  sortOrder: number;
  isMain: boolean;
}

export interface MarketplaceVariantAttribute {
  attributeDefinitionId: string;
  attributeOptionId: string;
  code: string;
  name: string;
  value: string;
}

export interface MarketplaceProductVariant {
  id: string;
  sku: string;
  name: string | null;
  price: MarketplaceMoney;
  compareAtPrice: MarketplaceMoney | null;
  isActive: boolean;
  isAvailable: boolean;
  stock: MarketplaceStock | null;
  attributes: MarketplaceVariantAttribute[];
}

export interface MarketplaceProductListItem {
  id: string;
  name: string;
  slug: string;
  brandName: string | null;
  categoryId: string;
  priceFrom: MarketplaceMoney;
  isAvailable: boolean;
  mainImage: MarketplaceProductImage | null;
}

export interface MarketplaceProductDetails {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  brandName: string | null;
  categoryId: string;
  images: MarketplaceProductImage[];
  variants: MarketplaceProductVariant[];
}

export interface MarketplaceFacetValue {
  attributeOptionId: string;
  value: string;
  count: number;
  selected: boolean;
}

export interface MarketplaceCategoryFacetValue {
  categoryId: string;
  name: string;
  slug: string;
  count: number;
  selected: boolean;
}

export interface MarketplaceFacet {
  attributeDefinitionId: string;
  code: string;
  name: string;
  isPurchaseOption: boolean;
  values: MarketplaceFacetValue[];
}

export interface MarketplaceCatalogSearchRequest {
  categoryId?: string | null;
  searchText?: string | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  inStock?: boolean | null;
  facets?: Record<string, string[]> | null;
  sort?: string | null;
  pageNumber: number;
  pageSize: number;
}

export interface MarketplaceCatalogSearchResponse {
  items: MarketplaceProductListItem[];
  productCount: number;
  categories: MarketplaceCategoryFacetValue[];
  facets: MarketplaceFacet[];
}

export interface CreateCheckoutReservationRequest {
  productVariantId: string;
  quantity: number;
  idempotencyKey: string;
}

export interface CheckoutReservation {
  reservationId: string;
  productVariantId: string;
  quantity: number;
  status: string;
  expiresAt: string;
  item: CheckoutReservationItem | null;
}

export interface CheckoutReservationItem {
  productId: string;
  productName: string;
  brandName: string | null;
  sku: string;
  variantName: string | null;
  price: MarketplaceMoney;
  image: MarketplaceProductImage | null;
  attributes: MarketplaceVariantAttribute[];
}

export interface MarketplacePaymentIntent {
  reservation: CheckoutReservation;
  paymentIntentId: string | null;
  paymentStatus: string;
  amount: MarketplaceMoney;
  failureReason: string | null;
  order: MarketplaceOrder | null;
}

export interface MarketplaceError {
  code: string;
  message: string;
  details?: Record<string, string> | null;
}

export interface MarketplaceOrder {
  orderId: string;
  reservationId: string;
  paymentId: string;
  status: string;
  total: MarketplaceMoney;
  createdAt: string;
  items: MarketplaceOrderItem[];
  statusHistory: MarketplaceOrderStatusHistory[];
}

export interface MarketplaceOrderItem {
  orderItemId: string;
  productId: string;
  productVariantId: string;
  productName: string;
  brandName: string | null;
  sku: string;
  variantName: string | null;
  unitPrice: MarketplaceMoney;
  quantity: number;
  total: MarketplaceMoney;
  image: MarketplaceProductImage | null;
  attributeSummary: string | null;
}

export interface MarketplaceOrderStatusHistory {
  status: string;
  createdAt: string;
  reason: string | null;
}

export interface Delivery {
  deliveryId: string;
  orderId: string;
  status: string;
  courierId: string | null;
  courierName: string | null;
  pickupAddress: string | null;
  dropoffAddress: string | null;
  createdAt: string;
  lastStateChangedAt: string;
  courierAssignmentExpiresAt: string | null;
  events: DeliveryEvent[];
  trackingPoints: DeliveryTrackingPoint[];
  order: MarketplaceOrder | null;
}

export interface DeliveryEvent {
  status: string;
  createdAt: string;
  message: string | null;
}

export interface DeliveryTrackingPoint {
  latitude: number;
  longitude: number;
  createdAt: string;
}
