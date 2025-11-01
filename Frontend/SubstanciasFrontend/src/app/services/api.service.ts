import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export abstract class ApiService<T> {
  constructor(protected http: HttpClient) {}

  /** Concatena baseUrl + path evitando // duplicada */
  protected url(path: string) {
    return `${environment.apiUrl}/${path}`.replace(/([^:]\/)\/+/g, '$1');
  }

  /** Opções padrão — importantíssimo manter observe:'body' */
  protected options(extra: {
    params?: Record<string, any> | HttpParams;
    headers?: Record<string, string>;
    withCredentials?: boolean;
  } = {}) {
    const { params, headers, withCredentials } = extra;

    return {
      observe: 'body' as const,
      responseType: 'json' as const,
      withCredentials: withCredentials ?? false,
      params,
      headers
    };
  }

  getAll(path: string, params?: any) {
    return this.http.get<T[]>(this.url(path), this.options({ params }));
  }

  getById(path: string, id: number) {
    return this.http.get<T>(this.url(`${path}/${id}`), this.options());
  }

  create(path: string, body: any) {
    return this.http.post<T>(this.url(path), body, this.options());
  }

  update(path: string, id: number, body: any) {
    return this.http.put<T>(this.url(`${path}/${id}`), body, this.options());
  }

  delete(path: string, id: number) {
    return this.http.delete<void>(this.url(`${path}/${id}`), this.options());
  }
}
