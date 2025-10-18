export interface IPresignedUrlResponse {
    fileId: string;
    url: string;
    objectKey: string;
}

export enum EntityType {
    Gym = "Gym"
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