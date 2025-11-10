import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User, UserRole } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private mockUsers: User[] = [
    {
      id: '1',
      name: 'João Silva',
      email: 'joao.silva@apdl.pt',
      role: UserRole.SHIPPING_AGENT,
      isAuthenticated: true
    },
    {
      id: '2',
      name: 'Maria Santos',
      email: 'maria.santos@apdl.pt',
      role: UserRole.LOGISTICS_OPERATOR,
      isAuthenticated: true
    },
    {
      id: '3',
      name: 'Carlos Ferreira',
      email: 'carlos.ferreira@apdl.pt',
      role: UserRole.PORT_AUTHORITY,
      isAuthenticated: true
    },
    {
      id: '4',
      name: 'Admin User',
      email: 'admin@apdl.pt',
      role: UserRole.ADMIN,
      isAuthenticated: true
    }
  ];

  private currentUserSubject = new BehaviorSubject<User | null>(this.mockUsers[0]);
  public currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();

  constructor() {

  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getCurrentUserRole(): UserRole | null {
    const user = this.getCurrentUser();
    return user ? user.role : null;
  }

  isAuthenticated(): boolean {
    const user = this.getCurrentUser();
    return user !== null && user.isAuthenticated;
  }

  hasRole(role: UserRole): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole === role;
  }

  hasAnyRole(roles: UserRole[]): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole !== null && roles.includes(currentRole);
  }

  mockLogin(userId: string): void {
    const user = this.mockUsers.find(u => u.id === userId);
    if (user) {
      this.currentUserSubject.next(user);
      console.log('Logged in as:', user.name, '-', user.role);
    }
  }

  logout(): void {
    this.currentUserSubject.next(null);
    console.log('Logged out');
  }

  getMockUsers(): User[] {
    return this.mockUsers;
  }
}
