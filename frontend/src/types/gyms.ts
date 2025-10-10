export interface IGymResponse {
    id: string;
    name: string;
    description: string;
    imageUrl: string;
}

export interface IUpdateGymRequest {
    id: string;
    name: string;
    description: string;
}