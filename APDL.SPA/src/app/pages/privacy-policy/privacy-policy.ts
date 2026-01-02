import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PrivacyPolicyService, PrivacyPolicyDto } from '../../services/privacy-policy';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-privacy-policy',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './privacy-policy.html',
  styleUrl: './privacy-policy.css',
})
export class PrivacyPolicy implements OnInit {
  policy: PrivacyPolicyDto | null = null;
  loading = false;
  error: string | null = null;
  isAuthenticated = false;

  constructor(
    private privacyPolicyService: PrivacyPolicyService,
    private authService: Auth
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isAuthenticated = user !== null;
    });
    this.loadPolicy();
  }

  loadPolicy(): void {
    this.loading = true;
    this.error = null;

    this.privacyPolicyService.getActive().subscribe({
      next: (data) => {
        this.policy = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load privacy policy: ' + (err.error?.message || err.message);
        this.loading = false;
      }
    });
  }

  goToLogin(): void {
    window.location.href = '/login';
  }
}

