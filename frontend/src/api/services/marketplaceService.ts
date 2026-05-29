import { ApiService, ApiResponse } from '../ApiService';
import { ListResponse } from '../../types/common';
import {
  CheckoutReservation,
  CreateCheckoutReservationRequest,
  Delivery,
  MarketplaceCatalogSearchRequest,
  MarketplaceCatalogSearchResponse,
  MarketplaceOrder,
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

  const getOrder = (orderId: string): Promise<ApiResponse<MarketplaceOrder>> => {
    return apiService.get<MarketplaceOrder>(`/v1/marketplace/orders/${orderId}`);
  };

  const getMyOrders = (
    pageNumber: number,
    pageSize: number
  ): Promise<ApiResponse<ListResponse<MarketplaceOrder>>> => {
    return apiService.get<ListResponse<MarketplaceOrder>>(
      `/v1/marketplace/orders/my?PageNumber=${pageNumber}&PageSize=${pageSize}`
    );
  };

  const getOrderDelivery = (orderId: string): Promise<ApiResponse<Delivery>> => {
    return apiService.get<Delivery>(`/v1/marketplace/orders/${orderId}/delivery`);
  };

  const getDeliveries = (
    pageNumber: number,
    pageSize: number
  ): Promise<ApiResponse<ListResponse<Delivery>>> => {
    return apiService.get<ListResponse<Delivery>>(
      `/v1/marketplace/deliveries?PageNumber=${pageNumber}&PageSize=${pageSize}`
    );
  };

  return {
    searchProducts,
    getProduct,
    createReservation,
    getReservation,
    createPaymentIntent,
    getOrder,
    getMyOrders,
    getOrderDelivery,
    getDeliveries,
  };
};

export const useMarketplaceApi = createMarketplaceApi;
