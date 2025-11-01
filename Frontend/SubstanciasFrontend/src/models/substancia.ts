
export interface SubstanciaPropriedadeValor {
  propriedadeId: number;
  valueType: number;
  boolValue?: boolean;
  decimalValue?: number;
}

export interface Substancia {
  id?: number;
  codigo: string;
  nome: string;
  descricao?: string;
  notas?: string;
  categoriaId: number;
  propriedades: SubstanciaPropriedadeValor[];
}
