export interface IPresignedUrlResponse {
    fileId: string;
    url: string;
    objectKey: string;
}

export enum EntityType {
    Gym = "Gym",
    Equipment = "Equipment"
}

export interface IMakeFilesActiveRequest {
    fileIds: string[],
    entityId: string;
    entityType: EntityType
}

export interface IEntity {
    id: string;
    type: EntityType;
    maxFileCount: number;
}

export interface IFileResponse {
    id: string;
    fileName: string;
}