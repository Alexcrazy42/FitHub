import { ApiService, ApiResponse } from '../ApiService';
import {
  CheckoutReservation,
  CreateCheckoutReservationRequest,
  MarketplaceCatalogSearchRequest,
  MarketplaceCatalogSearchResponse,
  MarketplaceProductDetails,
} from '../../types/marketplace';

export const useMarketplaceApi = (apiService: ApiService) => {
  const searchProducts = (
    request: MarketplaceCatalogSearchRequest
  ): Promise<ApiResponse<MarketplaceCatalogSearchResponse>> => {
    return apiService.post<MarketplaceCatalogSearchResponse>(
      '/v1/marketplace/catalog/products/search',
      request
    );
  };

  const getProduct = (productId: string): Promise<ApiResponse<MarketplaceProductDetails>> => {
    return apiService.get<MarketplaceProductDetails>(`/v1/marketplace/catalog/products/${productId}`);
  };

  const createReservation = (
    request: CreateCheckoutReservationRequest
  ): Promise<ApiResponse<CheckoutReservation>> => {
    return apiService.post<CheckoutReservation>('/v1/marketplace/checkout/reservations', request);
  };

  return {
    searchProducts,
    getProduct,
    createReservation,
  };
};
