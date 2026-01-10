import { UserResponse } from "./auth";
import { IGymResponse } from "./gyms";

export interface IGymAdminResponse {
    id: string;
    user: UserResponse;
    gyms: IGymResponse[];
}

export interface ITrainerResponse {
    id: string;
    user: UserResponse;
}

export interface IVisitorResponse {
    id: string;
    user: UserResponse;
}

export interface IGetUsersRequest {
    partName: string | null;
}