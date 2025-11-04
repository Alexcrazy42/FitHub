export interface IGymResponse {
    id: string;
    name: string;
    description: string;
    imageFileId: string;
}

export interface ICreateGymRequest {
    name: string;
    description: string;
}

export interface IUpdateGymRequest {
    id: string;
    name: string;
    description: string;
}

export interface IGymZoneResponse {
    id: string;
    name: string;
    description: string;
}

export interface ICreateGymZoneRequest {
    name: string;
    description: string;
}

export interface IUpdateGymZoneRequest {
    id: string;
    name: string;
    description: string;
}