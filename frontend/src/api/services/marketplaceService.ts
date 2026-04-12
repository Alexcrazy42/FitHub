import { ApiService, ApiResponse } from '../ApiService';
import {
  CheckoutReservation,
  CreateCheckoutReservationRequest,
  MarketplaceCatalogSearchRequest,
  MarketplaceCatalogSearchResponse,
  MarketplacePaymentIntent,
  MarketplaceProductDetails,
} from '../../types/marketplace';

export const createMarketplaceApi = (apiService: ApiService) => {
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

  const getReservation = (reservationId: string): Promise<ApiResponse<CheckoutReservation>> => {
    return apiService.get<CheckoutReservation>(`/v1/marketplace/checkout/reservations/${reservationId}`);
  };

  const createPaymentIntent = (reservationId: string): Promise<ApiResponse<MarketplacePaymentIntent>> => {
    return apiService.post<MarketplacePaymentIntent>(
      `/v1/marketplace/checkout/reservations/${reservationId}/payment-intent`
    );
  };

  return {
    searchProducts,
    getProduct,
    createReservation,
    getReservation,
    createPaymentIntent,
  };
};

export const useMarketplaceApi = createMarketplaceApi;
