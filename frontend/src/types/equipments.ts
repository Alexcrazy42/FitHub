export enum ComplexityType {
  Low = "Low",
  Medium = "Medium",
  Hard = "Hard",
}

export interface IEquipmentForm {
  name: string;
  description: string;
  additionalDescription: string;
  type: ComplexityType;
}

export const complexityTypeOptions : { label: string; value: ComplexityType }[] = [
  { label: "Легкий", value: ComplexityType.Low },
  { label: "Средний", value: ComplexityType.Medium },
  { label: "Тяжелый", value: ComplexityType.Hard },
];



export interface IBrandResponse {
  id: string;
  name: string;
  description: string;
}

export interface IEquipmentResponse {
  id: string;
  name: string;
  description: string;
  additionalDescroption: string | null;
  instructionAddBefore?: Date | null;
  isActive: boolean;
  brand: IBrandResponse;
}

export interface ICreateEquipmentRequest {
  brandId?: string | null;
  name?: string | null;
  description?: string | null;
  additionalDescroption?: string | null;
  instructionAddBefore?: Date | null;
  isActive?: boolean | null;
}