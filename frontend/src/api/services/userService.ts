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