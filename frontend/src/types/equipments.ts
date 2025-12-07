import { IGymResponse } from "../types/gyms"

export enum ComplexityType {
  Low = "Low",
  Medium = "Medium",
  Hard = "Hard",
}

export enum EquipmentCondition {
  Operational = "Operational",
  Maintenance = "Maintenance",
  UnderRepair = "UnderRepair"
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

export const equipmentConditionsOptions : {label: string; value : EquipmentCondition}[] = [
  { label: "Готов к использованию", value: EquipmentCondition.Operational },
  { label: "На профилактике", value: EquipmentCondition.Maintenance },
  { label: "На ремонте", value: EquipmentCondition.UnderRepair },
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

export interface IGymEquipmentResponse {
  id: string;
  equipment: IEquipmentResponse,
  gym: IGymResponse,
  isActive: boolean,
  openedAt: Date | null;
  condition: EquipmentCondition
}

export interface IAddOrUpdateGymEquipmentRequest {
  equipmentId: string;
  gymId: string;
  isActive: boolean;
  openedAt: Date | null;
  condition: EquipmentCondition;
}