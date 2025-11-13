import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './access-denied.html',
  styleUrl: './access-denied.css'
})
export class AccessDenied {
  constructor(public authService: Auth) {}
}