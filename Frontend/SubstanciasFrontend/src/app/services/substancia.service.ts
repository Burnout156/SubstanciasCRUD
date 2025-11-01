import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ApiService } from './api.service';
import { Substancia } from '../../models/substancia';

// Se quiser tipar a listagem:
export type SubstanciaListItem = {
  id: number;
  codigo: string;
  nome: string;
  descricao?: string;
  notas?: string;
  categoriaId: number;
  categoriaNome: string;
};
export type PagedResult<T> = { total: number; items: T[] };

@Injectable({ providedIn: 'root' })
export class SubstanciaService extends ApiService<Substancia> {
  private endpoint = 'api/substancias';

  constructor(protected override http: HttpClient) { super(http); }

  /** Lista para a tabela: mapeia { total, items } -> items */
  listar() {
    return this.http
      .get<PagedResult<SubstanciaListItem>>(this.url(this.endpoint), this.options())
      .pipe(map(r => r.items ?? []));
  }

  /** Caso queira paginação/busca depois */
  listarPaginado(page = 1, pageSize = 20, search?: string) {
    const params: any = { page, pageSize };
    if (search) params.search = search;

    return this.http.get<PagedResult<SubstanciaListItem>>(
      this.url(this.endpoint),
      this.options({ params })
    );
  }

  buscarPorId(id: number) {
    return this.getById(this.endpoint, id);
  }

  criar(data: Substancia) {
    // Normaliza o código como no backend (evita "Código já existe.")
    const payload = { ...data, codigo: (data.codigo ?? '').trim().toUpperCase() };
    return this.create(this.endpoint, payload);
  }

  atualizar(id: number, data: Partial<Substancia>) {
    if (data.codigo) data.codigo = data.codigo.trim().toUpperCase();
    return this.update(this.endpoint, id, data);
  }

  deletar(id: number) {
    return this.delete(this.endpoint, id);
  }
}
