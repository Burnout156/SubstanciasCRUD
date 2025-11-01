export enum PropertyValueType {
  Boolean = 0,
  Decimal = 1
}

export interface Propriedade {
  id?: number;
  nome: string;
  valueType: PropertyValueType;
}
