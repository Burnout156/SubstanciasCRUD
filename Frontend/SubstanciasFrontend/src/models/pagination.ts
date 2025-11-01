export interface PagedResult<T> {
  total: number;
  items: T[];
}

export interface SubstanciaListItem {
  id: number;
  codigo: string;
  nome: string;
  categoriaId: number;
  categoriaNome?: string; // <- ADICIONE
  descricao?: string;
  notas?: string;
}
