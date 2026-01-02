import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService as Auth0Service } from '@auth0/auth0-angular';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { User, UserRole } from '../models/user';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiService } from './api';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  // private mockUsers: User[] = [
  //   {
  //     id: '1',
  //     name: 'João Silva',
  //     email: 'joao.silva@apdl.pt',
  //     role: UserRole.SHIPPING_AGENT,
  //     isAuthenticated: true
  //   },
  //   {
  //     id: '2',
  //     name: 'Maria Santos',
  //     email: 'maria.santos@apdl.pt',
  //     role: UserRole.LOGISTICS_OPERATOR,
  //     isAuthenticated: true
  //   },
  //   {
  //     id: '3',
  //     name: 'Carlos Ferreira',
  //     email: 'carlos.ferreira@apdl.pt',
  //     role: UserRole.PORT_AUTHORITY,
  //     isAuthenticated: true
  //   },
  //   {
  //     id: '4',
  //     name: 'Admin User',
  //     email: 'admin@apdl.pt',
  //     role: UserRole.ADMIN,
  //     isAuthenticated: true
  //   }
  // ];

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();


  private isLoadingSubject = new BehaviorSubject<boolean>(true);
  public isLoading$: Observable<boolean> = this.isLoadingSubject.asObservable();

  constructor(private auth0: Auth0Service,
    private router: Router,
    private http: HttpClient,
    private apiService: ApiService
  ) {
    this.initialize();
  }

  private initialize(): void {
    this.isLoadingSubject.next(true);

    this.auth0.isAuthenticated$.subscribe(isAuthenticated => {
      if (isAuthenticated) {
        console.log("User authenticated with auth0");
        this.loadUserRoleFromBackend();
      } else {
        console.log("User not authenticated");
        this.currentUserSubject.next(null);
        this.isLoadingSubject.next(false);
      }
    })
  }

  login(): void {
    console.log('Going to auth0 login');
    this.auth0.loginWithRedirect();
  }

  logout(): void {
    console.log("Logging out");
    this.currentUserSubject.next(null);
    this.auth0.logout({
      logoutParams: {
        returnTo: window.location.origin + '/login'
      }
    });
  }

  private async loadUserRoleFromBackend(): Promise<void> {
    try {
      const auth0User = await firstValueFrom(this.auth0.user$);

      if (!auth0User) {
        throw new Error("No Auth0 user data");
      }

      console.log('Auth0 user:', auth0User.email);
      console.log('GEtting role from backend');

      const token = await firstValueFrom(
        this.auth0.getAccessTokenSilently({
          authorizationParams: {
            audience: environment.auth0.authorizationParams.audience,
            scope: 'openid profile email offline_access'
          },
        })
      );

      const url = `${environment.apiUrl}/auth/whoami`;
      console.log("Requesting:", url);

      console.log("Token:", token);
      console.log("Token length:", token?.length);
      console.log("Full headers:", { Authorization: `Bearer ${token}` });

      const user = await firstValueFrom(
        this.http.get<User>(url, {
          headers: { Authorization: `Bearer ${token}` }
        })
      );

      if (!user || !user.role) {
        console.error('User has no assigned role');
        throw new Error('User has no assigned role');
      }

      console.log('User loaded from backend');
      this.currentUserSubject.next(user);
      this.isLoadingSubject.next(false);

      const currentUrl = this.router.url;
      if (currentUrl === '/login') {
        console.log("Current url:", currentUrl);
        console.log('Navigating to /home from:', currentUrl);
        this.router.navigate(['/home']);
      }


    } catch (error) {
      console.error('Error loading user from backed:', error);
      this.currentUserSubject.next(null);
      this.isLoadingSubject.next(false);
    }
  }
    activateUser(token: string): Observable<any> {
      const url = 'https://localhost:5001/api/auth/activate';
      return this.http.post(url, { token });
    }
    activateUserByEmail(email: string): Observable<any> {
    return this.apiService.post('auth/activate-user', { email });
  }


  refreshUser(): Promise<void> {
    return this.loadUserRoleFromBackend();
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getCurrentUserRole(): UserRole | null {
    const user = this.getCurrentUser();
    return user ? user.role : null;
  }

  // isAuthenticated(): boolean {
  //   const user = this.getCurrentUser();
  //   return user !== null && user.isAuthenticated;
  // }

  hasRole(role: UserRole): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole === role;
  }

  hasAnyRole(roles: UserRole[]): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole !== null && roles.includes(currentRole);
  }

  // mockLogin(userId: string): void {
  //   const user = this.mockUsers.find(u => u.id === userId);
  //   if (user) {
  //     this.currentUserSubject.next(user);
  //     console.log('Logged in as:', user.name, '-', user.role);
  //   }
  // }

  // logout(): void {
  //   this.currentUserSubject.next(null);
  //   console.log('Logged out');
  // }

  // getMockUsers(): User[] {
  //   return this.mockUsers;
  // }
}
