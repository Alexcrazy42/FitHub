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