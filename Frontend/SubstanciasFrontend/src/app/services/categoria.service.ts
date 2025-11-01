import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Categoria } from '../../models/categoria';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class CategoriaService extends ApiService<Categoria> {
  private endpoint = 'api/categorias';

  listar(): Observable<Categoria[]> {
    return this.getAll(this.endpoint);
  }

  criar(data: Categoria): Observable<Categoria> {
    return this.create(this.endpoint, data);
  }

  deletar(id: number): Observable<void> {
    return this.delete(this.endpoint, id);
  }
}
