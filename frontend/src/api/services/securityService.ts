import { useMemo } from 'react';
import { ApiResponse, ApiService } from '../ApiService';

export interface CheckUrlResponse {
  domain: string;
  hasSsl: boolean;
  maliciousCount: number;
  suspiciousCount: number;
  totalEngines: number;
  status: 'Safe' | 'Suspicious' | 'Malicious' | 'Unknown';
}

export class SecurityService {
  private apiService: ApiService;

  constructor(apiService: ApiService) {
    this.apiService = apiService;
  }

  public async checkUrl(url: string): Promise<ApiResponse<CheckUrlResponse>> {
    return this.apiService.post<CheckUrlResponse>('/v1/security/check-url', { url });
  }
}

export const useSecurityService = (apiService: ApiService) => {
  const service = useMemo(() => new SecurityService(apiService), [apiService]);
  return service;
};
