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
    // Lógica para detetar os parâmetros de ativação no URL
    this.route.queryParams.subscribe(params => {
      // Verifica se o URL contém ?activated=true e um email
      const isActivated = params['activated'] === 'true';
      const email = params['email'];

      if (isActivated && email) {
        this.activateUser(email);
      }
    });
  }
  
  /**
   * NOVO MÉTODO: Faz a chamada ao Auth Service para ativar o utilizador e trata a resposta.
   * @param email O email a ativar.
   */
  activateUser(email: string): void {
    this.activationStatus = 'activating'; // Status: A processar

    this.authService.activateUserByEmail(email).subscribe({
      next: () => {
        // Sucesso na ativação
        this.activationStatus = 'success';
        this.clearUrlParams();
      },
      error: (err) => {
        // Falha na ativação
        console.error('Falha ao ativar o utilizador:', err);
        this.activationStatus = 'error';
        this.clearUrlParams();
      }
    });
  }

  clearUrlParams(): void {
    // Navega para a mesma rota, mas com queryParams nulos para limpar o URL
    this.router.navigate([], {
      queryParams: { activated: null, email: null },
      queryParamsHandling: 'merge'
    });
  }

  login(): void {
    this.authService.login();
  }
}