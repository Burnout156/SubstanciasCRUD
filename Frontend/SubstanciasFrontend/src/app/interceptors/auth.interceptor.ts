// app/core/auth.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpHandler, HttpInterceptor, HttpRequest
} from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../environments/environment';

const API_BASE = environment.apiUrl.replace(/\/+$/, ''); // normaliza

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private keycloak: KeycloakService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Só intercepta chamadas para a sua API
    const url = req.url;
    const isApiCall = url.startsWith(API_BASE);

    if (!isApiCall) {
      return next.handle(req);
    }

    return from(this.keycloak.updateToken(30)).pipe( // renova se faltar <30s
      switchMap(() => from(this.keycloak.getToken())),
      switchMap(token => {
        if (!token) {
          // sem token -> deixa seguir (backend vai 401 e você vê no log)
          return next.handle(req);
        }

        const cloned = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });

        // DEBUG opcional:
        // console.log('➡️  Enviando para API com Authorization Bearer.');

        return next.handle(cloned);
      })
    );
    }
}
