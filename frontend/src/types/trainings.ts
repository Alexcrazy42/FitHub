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
    id: string | null;
    name: string | null;
}

export interface ICreateTrainingType {
    name: string;
}

export interface IBaseGroupTraining {
    id: string | null;
    name: string | null;
    description : string | null;
    durationInMinutes : number | null;
    complexity : number | null;
    isActive: boolean | null;
    trainingTypes : ITrainingType[];
}


export interface ICreateBaseGroupTraining {
    id: string;
    name: string;
    description : string;
    durationInMinutes : number;
    complexity : number;
    isActive : boolean;
    trainingTypeIds : string[]
}
