import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  OperationPlanService,
  OperationPlanDto,
  CreateOperationPlanDto,
  UpdateOperationPlanDto,
  VesselScheduleEntry
} from '../../services/operation-plan';
import { Notification } from '../../services/notification';

@Component({
  selector: 'app-operation-plans',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './operation-plans.html',
  styleUrl: './operation-plans.css',
})
export class OperationPlans implements OnInit {
  plans: OperationPlanDto[] = [];
  filteredPlans: OperationPlanDto[] = [];
  selectedPlan: OperationPlanDto | null = null;

  showGenerateForm = false;
  showEditForm = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  filterStatus = 'All';
  statusOptions = ['All', 'Generated', 'Manual', 'Approved'];

  availableVVNs: any[] = [];
  newPlan: CreateOperationPlanDto = {
    vvnIds: [],
    algorithm: 'original',
    notes: ''
  };

  editPlan: UpdateOperationPlanDto = {
    schedule: [],
    status: 'Generated',
    notes: ''
  };

  selectedVvnId = '';

  constructor(
    private operationPlanService: OperationPlanService,
    private notificationService: Notification
  ) { }

  ngOnInit(): void {
    this.loadPlans();
    this.loadAvailableVVNs();
  }

  loadPlans(): void {
    this.loading = true;
    this.error = null;

    this.operationPlanService.getAll().subscribe({
      next: (data) => {
        this.plans = data;
        this.applyFilter();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load operation plans: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  loadAvailableVVNs(): void {
    this.notificationService.getByStatus('Approved').subscribe({
      next: (data) => {
        this.availableVVNs = data;
        if (data.length === 0) {
          console.warn('No approved VVNs found. You need to have at least one VVN with status "Approved" to generate an operation plan.');
        }
      },
      error: (err) => {
        console.error('Failed to load VVNs by status, trying to load all and filter client-side:', err);
        this.loadAllVVNsAndFilter();
      }
    });
  }

  loadAllVVNsAndFilter(): void {
    this.notificationService.getAll().subscribe({
      next: (data) => {
        this.availableVVNs = data.filter(vvn => vvn.status === 'Approved');
        if (this.availableVVNs.length === 0) {
          if (!this.error) {
            this.error = 'No approved VVNs found. Please approve a Vessel Visit Notification first.';
          }
        } else {
          this.error = null;
        }
      },
      error: (err) => {
        console.error('Failed to load VVNs:', err);
        if (!this.error) {
          this.error = 'Failed to load available VVNs: ' + (err.error?.error || err.message) + '. Make sure you have approved VVNs in the system.';
        }
      }
    });
  }

  applyFilter(): void {
    if (this.filterStatus === 'All') {
      this.filteredPlans = this.plans;
    } else {
      this.filteredPlans = this.plans.filter(
        p => p.status === this.filterStatus
      );
    }
  }

  onFilterChange(): void {
    this.applyFilter();
  }

  selectPlan(plan: OperationPlanDto): void {
    this.selectedPlan = plan;
  }

  openGenerateForm(): void {
    this.showGenerateForm = true;
    this.showEditForm = false;
    this.resetGenerateForm();
  }

  closeGenerateForm(): void {
    this.showGenerateForm = false;
    this.resetGenerateForm();
  }

  openEditForm(plan: OperationPlanDto): void {
    this.showEditForm = true;
    this.showGenerateForm = false;
    this.selectedPlan = plan;
    this.editPlan = {
      schedule: JSON.parse(JSON.stringify(plan.schedule)),
      status: plan.status,
      notes: plan.notes || ''
    };
  }

  closeEditForm(): void {
    this.showEditForm = false;
    this.editPlan = {
      schedule: [],
      status: 'Generated',
      notes: ''
    };
  }

  resetGenerateForm(): void {
    this.newPlan = {
      vvnIds: [],
      algorithm: 'original',
      notes: ''
    };
    this.selectedVvnId = '';
    this.error = null;
  }

  addVvnToForm(): void {
    if (this.selectedVvnId && !this.newPlan.vvnIds.includes(this.selectedVvnId)) {
      this.newPlan.vvnIds.push(this.selectedVvnId);
      this.selectedVvnId = '';
    }
  }

  removeVvnFromForm(vvnId: string): void {
    this.newPlan.vvnIds = this.newPlan.vvnIds.filter(id => id !== vvnId);
  }

  generatePlan(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.validateGenerateForm()) {
      return;
    }

    this.loading = true;

    this.operationPlanService.generate(this.newPlan).subscribe({
      next: (created) => {
        this.successMessage = 'Operation plan generated successfully!';
        this.plans.push(created);
        this.applyFilter();
        this.loading = false;
        this.closeGenerateForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to generate plan: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  updatePlan(): void {
    if (!this.selectedPlan) return;

    this.error = null;
    this.successMessage = null;

    if (!this.validateEditForm()) {
      return;
    }

    this.loading = true;

    this.operationPlanService.update(this.selectedPlan.planId, this.editPlan).subscribe({
      next: (updated) => {
        this.successMessage = 'Operation plan updated successfully!';
        const index = this.plans.findIndex(p => p.planId === updated.planId);
        if (index !== -1) {
          this.plans[index] = updated;
        }
        if (this.selectedPlan?.planId === updated.planId) {
          this.selectedPlan = updated;
        }
        this.applyFilter();
        this.loading = false;
        this.closeEditForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to update plan: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  deletePlan(id: string): void {
    if (confirm('Are you sure you want to delete this operation plan?')) {
      this.loading = true;
      this.error = null;

      this.operationPlanService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Operation plan deleted successfully!';
          this.plans = this.plans.filter(p => p.planId !== id);
          this.applyFilter();
          if (this.selectedPlan?.planId === id) {
            this.selectedPlan = null;
          }
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete plan: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  validateGenerateForm(): boolean {
    if (this.newPlan.vvnIds.length === 0) {
      this.error = 'At least one VVN ID is required';
      return false;
    }
    return true;
  }

  validateEditForm(): boolean {
    if (this.editPlan.schedule && this.editPlan.schedule.length === 0) {
      this.error = 'Schedule cannot be empty';
      return false;
    }
    return true;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Generated':
        return 'status-generated';
      case 'Manual':
        return 'status-manual';
      case 'Approved':
        return 'status-approved';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }

  formatTimeUnits(units: number): string {
    const hours = Math.floor(units * 10 / 60);
    const minutes = (units * 10) % 60;
    return `${hours}h ${minutes}m`;
  }
}

