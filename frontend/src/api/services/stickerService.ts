import { useMemo } from 'react';
import { ApiResponse, ApiService } from '../ApiService';
import { IStickerGroupResponse, IStickerResponse } from '../../types/stickers';
import { ListResponse } from '../../types/common';

export class StickerService {
  constructor(private apiService: ApiService) {}

  getGroups(): Promise<ApiResponse<ListResponse<IStickerGroupResponse>>> {
    return this.apiService.get('/v1/sticker-groups');
  }

  getStickersByGroup(groupId: string): Promise<ApiResponse<ListResponse<IStickerResponse>>> {
    return this.apiService.get(`/v1/sticker-groups/${groupId}/stickers`);
  }

  getStickers(page: number, pageSize: number): Promise<ApiResponse<ListResponse<IStickerResponse>>> {
    return this.apiService.get(`/v1/stickers?pageNumber=${page}&pageSize=${pageSize}`);
  }

  createGroup(name: string): Promise<ApiResponse<IStickerGroupResponse>> {
    return this.apiService.post('/v1/sticker-groups', { name });
  }

  updateGroup(id: string, name: string): Promise<ApiResponse<IStickerGroupResponse>> {
    return this.apiService.put(`/v1/sticker-groups/${id}`, { name });
  }

  activateGroup(id: string): Promise<ApiResponse<IStickerGroupResponse>> {
    return this.apiService.post(`/v1/sticker-groups/${id}/activate`);
  }

  deleteGroup(id: string): Promise<ApiResponse<never>> {
    return this.apiService.delete(`/v1/sticker-groups/${id}`);
  }

  addSticker(groupId: string, fileId: string, name: string): Promise<ApiResponse<IStickerResponse>> {
    return this.apiService.post('/v1/stickers', { groupId, fileId, name });
  }

  updateStickerName(id: string, name: string): Promise<ApiResponse<IStickerResponse>> {
    return this.apiService.put(`/v1/stickers/${id}/name`, { name });
  }

  updateStickerPhoto(id: string, newFileId: string): Promise<ApiResponse<IStickerResponse>> {
    return this.apiService.put(`/v1/stickers/${id}/photo`, { newFileId });
  }

  removeSticker(id: string): Promise<ApiResponse<never>> {
    return this.apiService.delete(`/v1/stickers/${id}`);
  }
}

export const useStickerService = (apiService: ApiService) =>
  useMemo(() => new StickerService(apiService), [apiService]);
