import { useMemo } from "react";
import { ApiResponse, ApiService } from "../ApiService";
import { IGetUsersRequest } from "../../types/users";
import { IPagedRequest, ListResponse } from "../../types/common";
import { UserResponse } from "../../types/auth";

export class UserService {
    private apiService : ApiService;

    constructor(apiService: ApiService) {
        this.apiService = apiService;
    }

    public async getMe(): Promise<ApiResponse<UserResponse>> {
        return this.apiService.get<UserResponse>('/v1/users/me');
    }

    public async getUser(id: string): Promise<ApiResponse<UserResponse>> {
        return this.apiService.get<UserResponse>(`/v1/users/${id}`);
    }

    public async updateProfile(name: string, surname: string): Promise<ApiResponse<void>> {
        return this.apiService.patch<void>('/v1/users/me/profile', { name, surname });
    }

    public async changePassword(oldPassword: string, newPassword: string): Promise<ApiResponse<void>> {
        return this.apiService.post<void>('/v1/users/me/change-password', { oldPassword, newPassword });
    }

    public async getUsers(query: IGetUsersRequest, paged: IPagedRequest) : Promise<ApiResponse<ListResponse<UserResponse>>> {
        const searchParams = new URLSearchParams();
        searchParams.append('PartName', query.partName ?? "");
        searchParams.append('PageNumber', paged.PageNumber.toString());
        searchParams.append('PageSize', paged.PageSize.toString());

        return this.apiService.get<ListResponse<UserResponse>>(`/v1/users?${searchParams.toString()}`);
    }
}

export const useUserService = (apiService : ApiService) => {
    const messageService = useMemo(() => {
        return new UserService(apiService);
    }, [apiService]);

    return messageService;
}