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