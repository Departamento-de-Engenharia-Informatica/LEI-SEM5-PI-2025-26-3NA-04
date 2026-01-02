import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PrivacyPolicyService,
  PrivacyPolicyDto,
  CreatePrivacyPolicyDto
} from '../../services/privacy-policy';

@Component({
  selector: 'app-privacy-policy-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './privacy-policy-admin.html',
  styleUrl: './privacy-policy-admin.css',
})
export class PrivacyPolicyAdmin implements OnInit {
  versions: PrivacyPolicyDto[] = [];
  activePolicy: PrivacyPolicyDto | null = null;
  selectedVersion: PrivacyPolicyDto | null = null;

  showCreateForm = false;
  showPreview = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  newPolicy: CreatePrivacyPolicyDto = {
    content: '',
    effectiveDate: new Date().toISOString().split('T')[0]
  };

  constructor(private privacyPolicyService: PrivacyPolicyService) { }

  ngOnInit(): void {
    this.loadVersions();
    this.loadActivePolicy();
  }

  loadVersions(): void {
    this.loading = true;
    this.error = null;

    this.privacyPolicyService.getAllVersions().subscribe({
      next: (data) => {
        this.versions = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load versions: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  loadActivePolicy(): void {
    this.privacyPolicyService.getActive().subscribe({
      next: (data) => {
        this.activePolicy = data;
      },
      error: (err) => {
        console.error('Failed to load active policy:', err);
      }
    });
  }

  openCreateForm(): void {
    this.showCreateForm = true;
    this.showPreview = false;
    this.resetForm();
  }

  closeCreateForm(): void {
    this.showCreateForm = false;
    this.showPreview = false;
    this.resetForm();
  }

  resetForm(): void {
    this.newPolicy = {
      content: '',
      effectiveDate: new Date().toISOString().split('T')[0]
    };
    this.error = null;
    this.successMessage = null;
  }

  previewPolicy(): void {
    if (!this.newPolicy.content.trim()) {
      this.error = 'Please enter policy content before previewing.';
      return;
    }
    this.showPreview = true;
  }

  publishNewVersion(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.newPolicy.content.trim()) {
      this.error = 'Policy content is required.';
      return;
    }

    if (!this.newPolicy.effectiveDate) {
      this.error = 'Effective date is required.';
      return;
    }

    if (!confirm('Are you sure you want to publish this new version? The current active version will be archived.')) {
      return;
    }

    this.loading = true;

    this.privacyPolicyService.publishNewVersion(this.newPolicy).subscribe({
      next: (created) => {
        this.successMessage = `Privacy Policy version ${created.version} published successfully!`;
        this.loadVersions();
        this.loadActivePolicy();
        this.loading = false;
        this.closeCreateForm();
        setTimeout(() => {
          this.successMessage = null;
        }, 5000);
      },
      error: (err) => {
        this.error = 'Failed to publish new version: ' + (err.error?.message || err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  viewVersion(version: PrivacyPolicyDto): void {
    this.selectedVersion = version;
  }

  closeVersionView(): void {
    this.selectedVersion = null;
  }
}

