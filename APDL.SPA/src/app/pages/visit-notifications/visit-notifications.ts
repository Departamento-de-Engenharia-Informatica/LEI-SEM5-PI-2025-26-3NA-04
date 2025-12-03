import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  Notification,
  VesselVisitNotificationDto,
  CreateVesselVisitNotificationDto
} from '../../services/notification';

@Component({
  selector: 'app-visit-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './visit-notifications.html',
  styleUrl: './visit-notifications.css',
})
export class VisitNotifications implements OnInit {
  notifications: VesselVisitNotificationDto[] = [];
  filteredNotifications: VesselVisitNotificationDto[] = [];
  selectedNotification: VesselVisitNotificationDto | null = null;

  showCreateForm = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  filterStatus = 'All';
  statusOptions = ['All', 'InProgress', 'Submitted', 'Approved', 'Rejected'];

  newNotification: CreateVesselVisitNotificationDto = {
    vesselId: '',
    shippingAgentId: '',
    expectedArrival: '',
    expectedDeparture: '',
    cargoType: '',
    cargoVolume: 0,
    specialHandlingRequirements: '',
    captainName: '',
    totalCrewCount: 1,
    safetyCrewOfficerNames: []
  };

  newSafetyOfficerName = '';

  constructor(
    private notificationService: Notification
  ) { }

  ngOnInit(): void {
    this.loadNotifications();
    this.setShippingAgentId();
  }

  setShippingAgentId(): void {
    // TODO: Get the current user's shipping agent ID from auth service
    // For now, you'll need to implement this based on your auth system
    // Example: this.newNotification.shippingAgentId = this.authService.getCurrentUserShippingAgentId();

    // Placeholder - replace with actual implementation
    const currentUser = this.getCurrentUser();
    if (currentUser?.shippingAgentId) {
      this.newNotification.shippingAgentId = currentUser.shippingAgentId;
    }
  }

  getCurrentUser(): any {
    // TODO: Replace with actual auth service call
    // This should return the authenticated user with their shipping agent ID
    return null;
  }

  loadNotifications(): void {
    this.loading = true;
    this.error = null;

    this.notificationService.getAll().subscribe({
      next: (data) => {
        this.notifications = data;
        this.applyFilter();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load notifications: ' + err.message;
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (this.filterStatus === 'All') {
      this.filteredNotifications = this.notifications;
    } else {
      this.filteredNotifications = this.notifications.filter(
        n => n.status === this.filterStatus
      );
    }
  }

  onFilterChange(): void {
    this.applyFilter();
  }

  selectNotification(notification: VesselVisitNotificationDto): void {
    this.selectedNotification = notification;
  }

  openCreateForm(): void {
    this.showCreateForm = true;
    this.resetForm();
  }

  closeCreateForm(): void {
    this.showCreateForm = false;
    this.resetForm();
  }

  resetForm(): void {
    this.newNotification = {
      vesselId: '',
      shippingAgentId: this.newNotification.shippingAgentId, // Keep the shipping agent ID
      expectedArrival: '',
      expectedDeparture: '',
      cargoType: '',
      cargoVolume: 0,
      specialHandlingRequirements: '',
      captainName: '',
      totalCrewCount: 1,
      safetyCrewOfficerNames: []
    };
    this.newSafetyOfficerName = '';
    this.error = null;
  }

  addSafetyOfficerToForm(): void {
    if (this.newSafetyOfficerName.trim()) {
      if (!this.newNotification.safetyCrewOfficerNames) {
        this.newNotification.safetyCrewOfficerNames = [];
      }

      if (!this.newNotification.safetyCrewOfficerNames.includes(this.newSafetyOfficerName.trim())) {
        this.newNotification.safetyCrewOfficerNames.push(this.newSafetyOfficerName.trim());
        this.newSafetyOfficerName = '';
      } else {
        this.error = 'Safety officer already added';
      }
    }
  }

  removeSafetyOfficerFromForm(name: string): void {
    if (this.newNotification.safetyCrewOfficerNames) {
      this.newNotification.safetyCrewOfficerNames =
        this.newNotification.safetyCrewOfficerNames.filter(n => n !== name);
    }
  }

  createNotification(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.validateForm()) {
      return;
    }

    this.loading = true;

    this.notificationService.create(this.newNotification).subscribe({
      next: (created) => {
        this.successMessage = 'Notification created successfully!';
        this.notifications.push(created);
        this.applyFilter();
        this.loading = false;
        this.closeCreateForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to create notification: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  validateForm(): boolean {
    if (!this.newNotification.vesselId) {
      this.error = 'Vessel ID is required';
      return false;
    }
    if (!this.newNotification.shippingAgentId) {
      this.error = 'Shipping Agent ID is required';
      return false;
    }
    if (!this.newNotification.expectedArrival) {
      this.error = 'Expected arrival date is required';
      return false;
    }
    if (!this.newNotification.expectedDeparture) {
      this.error = 'Expected departure date is required';
      return false;
    }
    if (new Date(this.newNotification.expectedDeparture) <= new Date(this.newNotification.expectedArrival)) {
      this.error = 'Expected departure must be after expected arrival';
      return false;
    }
    if (!this.newNotification.cargoType) {
      this.error = 'Cargo type is required';
      return false;
    }
    if (this.newNotification.cargoVolume < 0) {
      this.error = 'Cargo volume cannot be negative';
      return false;
    }
    if (!this.newNotification.captainName) {
      this.error = 'Captain name is required';
      return false;
    }
    if (this.newNotification.totalCrewCount < 1) {
      this.error = 'Total crew count must be at least 1';
      return false;
    }
    return true;
  }

  submitNotification(id: string): void {
    if (confirm('Are you sure you want to submit this notification for review?')) {
      this.loading = true;
      this.error = null;

      this.notificationService.submit(id).subscribe({
        next: () => {
          this.successMessage = 'Notification submitted successfully!';
          this.loadNotifications();
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to submit notification: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  deleteNotification(id: string): void {
    if (confirm('Are you sure you want to delete this notification?')) {
      this.loading = true;
      this.error = null;

      this.notificationService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Notification deleted successfully!';
          this.notifications = this.notifications.filter(n => n.id !== id);
          this.applyFilter();
          this.selectedNotification = null;
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete notification: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'InProgress':
        return 'status-in-progress';
      case 'Submitted':
        return 'status-submitted';
      case 'Approved':
        return 'status-approved';
      case 'Rejected':
        return 'status-rejected';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }
}