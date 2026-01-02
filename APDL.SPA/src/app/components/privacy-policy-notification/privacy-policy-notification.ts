import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';
import { Auth } from '../../services/auth';
import { PrivacyPolicyService } from '../../services/privacy-policy';
import { User } from '../../models/user';

@Component({
  selector: 'app-privacy-policy-notification',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './privacy-policy-notification.html',
  styleUrl: './privacy-policy-notification.css',
})
export class PrivacyPolicyNotification implements OnInit, OnDestroy {
  show = false;
  currentVersion = 0;
  private userSubscription?: Subscription;

  constructor(
    private authService: Auth,
    private privacyPolicyService: PrivacyPolicyService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      console.log('PrivacyPolicyNotification - User updated:', {
        hasUser: !!user,
        notificationPending: user?.privacyPolicyNotificationPending,
        currentVersion: user?.currentPrivacyPolicyVersion
      });
      
      if (user && user.privacyPolicyNotificationPending) {
        this.show = true;
        this.currentVersion = user.currentPrivacyPolicyVersion || 0;
        console.log('PrivacyPolicyNotification - Showing notification for version:', this.currentVersion);
      } else {
        this.show = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }

  onViewPolicy(): void {
    this.router.navigate(['/privacy-policy']);
  }

  onAcknowledge(): void {
    if (this.currentVersion === 0) {
      return;
    }

    this.privacyPolicyService.acknowledge(this.currentVersion).subscribe({
      next: async () => {
        await this.authService.refreshUser();
        this.show = false;
      },
      error: (err) => {
        console.error('Failed to acknowledge privacy policy:', err);
      }
    });
  }
}

