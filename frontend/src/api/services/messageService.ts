import { useMemo } from "react";
import { ApiResponse, ApiService } from "../ApiService";
import { IChatMessageResponse, ICreateMessageRequest, IGetMessagesRequest, IMessageReadRequest, IMessageResponse, IUpdateMessageRequest } from "../../types/messaging";
import { IPagedRequest, ListResponse } from "../../types/common";

export class MessageService {
    private apiService : ApiService;

    constructor(apiService : ApiService) {
        this.apiService = apiService;
    }

    public async getMessages(
        query: IGetMessagesRequest, 
        paged: IPagedRequest
    ): Promise<ApiResponse<ListResponse<IMessageResponse>>> {
        const params = {
            chatId: query.chatId,
            IsDescending: query.isDescending?.toString(),
            From: query.from?.toISOString(),
            FromUnread: query.fromUnread?.toString(),
            PageNumber: paged.PageNumber.toString(),
            PageSize: paged.PageSize.toString(),
        };

        const filteredParams = Object.fromEntries(
            Object.entries(params).filter(([_, value]) => value !== undefined && value !== null)
        );

        const searchParams = new URLSearchParams(filteredParams);
        
        const response = await this.apiService.get<ListResponse<IMessageResponse>>(
            `/v1/messages?${searchParams.toString()}`
        );
        
        return response;
    }

    public async createMessage(request : ICreateMessageRequest) : Promise<ApiResponse<IMessageResponse>> {
        return this.apiService.post<IMessageResponse>('/v1/messages', request);
    }

    public async readMessages(request : IMessageReadRequest) : Promise<ApiResponse<never>> {
        return this.apiService.post('/v1/messages/read', request);
    }

    public async updateMessage(id: string, request: IUpdateMessageRequest) : Promise<ApiResponse<IMessageResponse>> {
        return this.apiService.put(`v1/messages/${id}`, request);
    }

    public async deleteMessage(id : string) : Promise<ApiResponse<never>> {
        return this.apiService.delete(`v1/messages/${id}`);
    }

    public async getChatMessagesList(page: number, size: number) : Promise<ApiResponse<ListResponse<IChatMessageResponse>>> {
        const searchParams = new URLSearchParams();
        searchParams.append('PageNumber', page.toString());
        searchParams.append('PageSize', size.toString());

        return this.apiService.get<ListResponse<IChatMessageResponse>>(`/v1/chat-messages/list?${searchParams.toString()}`);
    }
}

export const useMessageService = (apiService : ApiService) => {
    const messageService = useMemo(() => {
        return new MessageService(apiService);
    }, [apiService]);

    return messageService;
}