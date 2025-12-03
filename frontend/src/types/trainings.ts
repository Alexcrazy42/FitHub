import { Dayjs } from "dayjs";
import { IGymResponse } from "./gyms";
import { ITrainerResponse, IVisitorResponse } from "./users";

export interface IMuscleGroup {
    id: string;
    name: string;
    imageId: string | null;
    parentId: string | null;
    childrens: IMuscleGroup[];
}

export interface ICreateMuscleGroup {
    name: string;
    parentId: string | null;
}

export interface ITrainingType {
    id: string;
    name: string;
}

export interface ICreateTrainingType {
    name: string;
}

export interface IBaseGroupTraining {
    id: string;
    name: string | null;
    description : string | null;
    durationInMinutes : number | null;
    complexity : number | null;
    isActive: boolean | null;
    trainingTypes : ITrainingType[];
    photoId : string | null;
}


export interface ICreateBaseGroupTraining {
    name: string;
    description : string;
    durationInMinutes : number;
    complexity : number;
    isActive : boolean;
    trainingTypeIds : string[]
}

export interface IAttachPhoto {
    baseGroupTrainingId : string;
    fileId : string;
}

export interface IAddOrUpdateGroupTrainingRequest {
    baseGroupTrainingId: string | null;
    gymId: string | null;
    trainerId: string | null;
    startTime: Date | null;
    date: Dayjs | null;
    endTime: Date | null;
    isActive: boolean | null;
}

export interface IGroupTrainingResponse {
    id: string;
    baseGroupTraining: IBaseGroupTraining;
    gym: IGymResponse;
    trainer: ITrainerResponse;
    participants: IVisitorResponse[];
    startTime: Date;
    endTime: Date;
    isActive: boolean;
}