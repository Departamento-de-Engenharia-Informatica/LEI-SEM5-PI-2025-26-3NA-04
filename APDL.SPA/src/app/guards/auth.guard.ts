import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { combineLatest } from 'rxjs';
import { map, filter, take } from 'rxjs/operators';
import { Auth } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  return combineLatest([
    authService.isLoading$,
    authService.currentUser$
  ]).pipe(
    filter(([isLoading, user]) => !isLoading), 
    take(1),
    map(([isLoading, user]) => {
      if (!user) {
        console.log('Not authenticated - redirecting to login');
        router.navigate(['/login']);
        return false;
      }

      console.log('User authenticated:', user.email);
      return true;
    })
  );
};