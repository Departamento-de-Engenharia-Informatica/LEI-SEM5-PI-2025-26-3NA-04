import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Auth } from '../../services/auth';
import { User } from '../../models/user';
import { TranslatePipe, TranslateDirective } from "@ngx-translate/core";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TranslateDirective],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnInit, OnDestroy {
  currentUser: User | null = null;
  private userSubscription?: Subscription;

  constructor(private authService: Auth) { }

  ngOnInit(): void {
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      console.log('Header: User updated to', user?.name);
    });
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }

  getUserDisplayName(): string {
    if (!this.currentUser) return 'Guest';
    return this.currentUser.name;
  }

  getUserRole(): string {
    if (!this.currentUser) return '';
    return this.currentUser.role;
  }

  logout(): void {
    this.authService.logout();
  }
}
