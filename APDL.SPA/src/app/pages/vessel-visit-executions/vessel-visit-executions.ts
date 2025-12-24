import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  VesselVisitExecutionService,
  VesselVisitExecutionDto,
  CreateVesselVisitExecutionDto,
  UpdateVesselVisitExecutionDto
} from '../../services/vessel-visit-execution';
import { Notification } from '../../services/notification';

@Component({
  selector: 'app-vessel-visit-executions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vessel-visit-executions.html',
  styleUrl: './vessel-visit-executions.css',
})
export class VesselVisitExecutions implements OnInit {
  executions: VesselVisitExecutionDto[] = [];
  filteredExecutions: VesselVisitExecutionDto[] = [];
  selectedExecution: VesselVisitExecutionDto | null = null;

  showCreateForm = false;
  showEditForm = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  filterStatus = 'All';
  statusOptions = ['All', 'In Progress', 'Completed'];

  availableVVNs: any[] = [];
  newExecution: CreateVesselVisitExecutionDto = {
    vvnId: '',
    vesselId: '',
    actualArrivalTime: ''
  };

  editExecution: UpdateVesselVisitExecutionDto = {
    status: 'In Progress',
    actualBerthTime: '',
    actualDockId: '',
    actualUnberthTime: '',
    actualDepartureTime: ''
  };

  constructor(
    private executionService: VesselVisitExecutionService,
    private notificationService: Notification
  ) { }

  ngOnInit(): void {
    this.loadExecutions();
    this.loadAvailableVVNs();
  }

  loadExecutions(): void {
    this.loading = true;
    this.error = null;

    this.executionService.getAll().subscribe({
      next: (data) => {
        this.executions = data;
        this.applyFilter();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load executions: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  loadAvailableVVNs(): void {
    this.notificationService.getByStatus('Approved').subscribe({
      next: (data) => {
        this.availableVVNs = data;
        if (data.length === 0) {
          console.warn('No approved VVNs found. You need to have at least one VVN with status "Approved" to create an execution.');
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
          this.error = 'No approved VVNs found. Please approve a Vessel Visit Notification first.';
        } else {
          this.error = null;
        }
      },
      error: (err) => {
        console.error('Failed to load VVNs:', err);
        this.error = 'Failed to load available VVNs: ' + (err.error?.error || err.message) + '. Make sure you have approved VVNs in the system.';
      }
    });
  }

  applyFilter(): void {
    if (this.filterStatus === 'All') {
      this.filteredExecutions = this.executions;
    } else {
      this.filteredExecutions = this.executions.filter(
        e => e.status === this.filterStatus
      );
    }
  }

  onFilterChange(): void {
    this.applyFilter();
  }

  onVvnChange(): void {
    const selectedVvn = this.availableVVNs.find(v => v.id === this.newExecution.vvnId);
    if (selectedVvn) {
      this.newExecution.vesselId = selectedVvn.vesselId;
    }
  }

  selectExecution(execution: VesselVisitExecutionDto): void {
    this.selectedExecution = execution;
  }

  openCreateForm(): void {
    this.showCreateForm = true;
    this.showEditForm = false;
    this.resetCreateForm();
  }

  closeCreateForm(): void {
    this.showCreateForm = false;
    this.resetCreateForm();
  }

  openEditForm(execution: VesselVisitExecutionDto): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.selectedExecution = execution;
    this.editExecution = {
      status: execution.status,
      actualBerthTime: execution.actualBerthTime || '',
      actualDockId: execution.actualDockId || '',
      actualUnberthTime: execution.actualUnberthTime || '',
      actualDepartureTime: execution.actualDepartureTime || ''
    };
  }

  closeEditForm(): void {
    this.showEditForm = false;
    this.editExecution = {
      status: 'In Progress',
      actualBerthTime: '',
      actualDockId: '',
      actualUnberthTime: '',
      actualDepartureTime: ''
    };
  }

  resetCreateForm(): void {
    this.newExecution = {
      vvnId: '',
      vesselId: '',
      actualArrivalTime: ''
    };
    this.error = null;
  }

  createExecution(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.validateCreateForm()) {
      return;
    }

    this.loading = true;

    this.executionService.create(this.newExecution).subscribe({
      next: (created) => {
        this.successMessage = 'Vessel visit execution created successfully!';
        this.executions.push(created);
        this.applyFilter();
        this.loading = false;
        this.closeCreateForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to create execution: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  updateExecution(): void {
    if (!this.selectedExecution) return;

    this.error = null;
    this.successMessage = null;

    this.loading = true;

    this.executionService.update(this.selectedExecution._id, this.editExecution).subscribe({
      next: (updated) => {
        this.successMessage = 'Execution updated successfully!';
        const index = this.executions.findIndex(e => e._id === updated._id);
        if (index !== -1) {
          this.executions[index] = updated;
        }
        if (this.selectedExecution?._id === updated._id) {
          this.selectedExecution = updated;
        }
        this.applyFilter();
        this.loading = false;
        this.closeEditForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to update execution: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  deleteExecution(id: string): void {
    if (confirm('Are you sure you want to delete this execution?')) {
      this.loading = true;
      this.error = null;

      this.executionService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Execution deleted successfully!';
          this.executions = this.executions.filter(e => e._id !== id);
          this.applyFilter();
          if (this.selectedExecution?._id === id) {
            this.selectedExecution = null;
          }
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete execution: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  validateCreateForm(): boolean {
    if (!this.newExecution.vvnId) {
      this.error = 'VVN ID is required';
      return false;
    }
    if (!this.newExecution.vesselId) {
      this.error = 'Vessel ID is required';
      return false;
    }
    if (!this.newExecution.actualArrivalTime) {
      this.error = 'Actual arrival time is required';
      return false;
    }
    return true;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'In Progress':
        return 'status-in-progress';
      case 'Completed':
        return 'status-completed';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }
}

