import { Component, inject, NgZone } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { Auth } from './services/auth';
import {
  TranslateService,
  TranslatePipe,
  TranslateDirective
} from "@ngx-translate/core";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, TranslatePipe, TranslateDirective],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {
  private translate = inject(TranslateService);

  constructor(
    private authService: Auth,
    private ngZone: NgZone,
  ) {
    this.translate.addLangs(['pt', 'en']);
    this.translate.setFallbackLang('en');
    this.translate.use('pt');

    const originalMockLogin = authService.mockLogin.bind(authService);
    const originalLogout = authService.logout.bind(authService);

    authService.mockLogin = (userId: string) => {
      this.ngZone.run(() => originalMockLogin(userId));
    };

    authService.logout = () => {
      this.ngZone.run(() => originalLogout());
    };

    (window as any).authService = authService;

    console.log('Current user:', authService.getCurrentUser());
  }
}