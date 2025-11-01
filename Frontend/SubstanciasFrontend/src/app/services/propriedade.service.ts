import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { Propriedade } from '../../models/propriedade';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class PropriedadeService extends ApiService<Propriedade> {
  private endpoint = 'api/propriedades';

  listar(): Observable<Propriedade[]> {
    return this.getAll(this.endpoint);
  }

  criar(data: Propriedade): Observable<Propriedade> {
    return this.create(this.endpoint, data);
  }

  deletar(id: number): Observable<void> {
    return this.delete(this.endpoint, id);
  }
}
