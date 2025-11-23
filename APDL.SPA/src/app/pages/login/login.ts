import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit{
  public activationStatus: 'pending' | 'activating' | 'success' | 'error' = 'pending';
  constructor(private authService: Auth, private route: ActivatedRoute,private router: Router) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      // Verifica se o URL contém ?activated=true e um email
      const isActivated = params['resetSuccess'] === 'true';
      const email = params['email'];

      if (isActivated && email) {
        this.activateUser(email);
      }
    });
  }

  activateUser(email: string): void {
    this.activationStatus = 'activating';

    this.authService.activateUserByEmail(email).subscribe({
      next: () => {
        this.activationStatus = 'success';
        this.clearUrlParams();
      },
      error: (err) => {
        console.error('Falha ao ativar o utilizador:', err);
        this.activationStatus = 'error';
        this.clearUrlParams();
      }
    });
  }

  clearUrlParams(): void {
    this.router.navigate([], {
      queryParams: { activated: null, email: null },
      queryParamsHandling: 'merge'
    });
  }

  login(): void {
    this.authService.login();
  }
}