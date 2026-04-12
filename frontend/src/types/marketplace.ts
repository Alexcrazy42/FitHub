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
  facets?: Record<string, string[]> | null;
  sort?: string | null;
  pageNumber: number;
  pageSize: number;
}

export interface MarketplaceCatalogSearchResponse {
  items: MarketplaceProductListItem[];
  productCount: number;
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
}

export interface MarketplaceError {
  code: string;
  message: string;
  details?: Record<string, string> | null;
}
