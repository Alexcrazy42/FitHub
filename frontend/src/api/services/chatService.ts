import { useMemo } from "react";
import { IChatResponse, ICreateChatRequest, IInitiatorAndTargetUserRequest } from "../../types/messaging";
import { ApiResponse, ApiService } from "../ApiService";

export class ChatService {
    private apiService : ApiService;

    constructor(apiService: ApiService) {
        this.apiService = apiService;
    }

    public async getChat(id: string) : Promise<ApiResponse<IChatResponse>> {
        return this.apiService.get<IChatResponse>(`/v1/chats/${id}`);
    }

    public async createChat(request: ICreateChatRequest) : Promise<ApiResponse<IChatResponse>> {
        return this.apiService.post<IChatResponse>('/v1/chats', request)
    }

    public async inviteToChat(request: IInitiatorAndTargetUserRequest) : Promise<ApiResponse<never>> {
        return this.apiService.post<never>('/v1/chats/invite', request);
    }

    public async excludeFromChat(request: IInitiatorAndTargetUserRequest) : Promise<ApiResponse<never>> {
        return this.apiService.post<never>('/v1/chats/exclude', request);
    }
} 

export const useChatService = (apiService: ApiService) => {
    const chatService = useMemo(() => {
        return new ChatService(apiService);
    }, [apiService]);
    
    return chatService;
};