import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth0 = inject(AuthService);

  const isApiRequest = req.url.includes(environment.apiUrl) || 
                       req.url.includes('localhost:5001') || 
                       req.url.includes('10.9.11.75');

  if (!isApiRequest) {
    return next(req);
  }

  return auth0.getAccessTokenSilently({
    authorizationParams: {
      audience: environment.auth0.authorizationParams.audience,
      scope: 'openid profile email offline_access'
    }
  }).pipe(
    switchMap(token => {
      if (!token) {
        console.warn('No token available for request:', req.url);
        return next(req);
      }

      const clonedRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      
      console.log('Adding token to request:', req.url);
      return next(clonedRequest);
    }),
    catchError(err => {
      console.error('Error getting access token for request:', req.url, err);
      return throwError(() => err);
    })
  );
};