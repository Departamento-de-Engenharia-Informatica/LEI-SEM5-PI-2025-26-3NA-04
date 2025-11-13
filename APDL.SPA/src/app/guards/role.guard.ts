import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { combineLatest } from 'rxjs';
import { map, filter, take } from 'rxjs/operators';
import { Auth } from '../services/auth';
import { UserRole } from '../models/user';

export const roleGuard = (allowedRoles: UserRole[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(Auth);
    const router = inject(Router);

    return combineLatest([
      authService.isLoading$,
      authService.currentUser$
    ]).pipe(
      filter(([isLoading, user]) => !isLoading),
      map(([isLoading, user]) => {
        if (!user) {
          console.log('Not authenticated - redirecting to login');
          router.navigate(['/login']);
          return false;
        }

        const hasRole = allowedRoles.includes(user.role as UserRole);
        
        if (!hasRole) {
          console.log(`Access denied for ${user.email} (${user.role}) to ${state.url}`);
          console.log(`Required roles: ${allowedRoles.join(', ')}`);
          router.navigate(['/access-denied']);
          return false;
        }

        console.log(`Access granted for ${user.email} (${user.role}) to ${state.url}`);
        return true;
      })
    );
  };
};