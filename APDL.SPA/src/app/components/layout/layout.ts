import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Header } from '../header/header';
import { Footer } from '../footer/footer';
import { Auth } from '../../services/auth';
import { MenuItem, MENU_ITEMS } from '../../models/menu';
import { TranslatePipe, TranslateDirective } from "@ngx-translate/core";
import { UserRole } from '../../models/user';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    Header,
    Footer,
    TranslatePipe,
    TranslateDirective
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class Layout implements OnInit, OnDestroy {
  isMenuOpen = false;
  visibleMenuItems: MenuItem[] = [];
  private userSubscription?: Subscription;

  constructor(
    private authService: Auth,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      this.updateVisibleMenuItems();
    });
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }

  private updateVisibleMenuItems(): void {
    const currentRole = this.authService.getCurrentUserRole();
    
    if (!currentRole) {
      this.visibleMenuItems = [];
      return;
    }

    this.visibleMenuItems = MENU_ITEMS.filter(item =>
      item.allowedRoles.includes(currentRole)
    );

    console.log('Visble menu items for', currentRole, ':', this.visibleMenuItems.length);
  }
}