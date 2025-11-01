import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';
import { switchMap } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private keycloakService: KeycloakService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.keycloakService.getToken()).pipe(
      switchMap(token => {
        const cloned = token
          ? req.clone({
              setHeaders: { Authorization: `Bearer ${token}` },
            })
          : req;

        return next.handle(cloned);
      })
    );
  }
}
