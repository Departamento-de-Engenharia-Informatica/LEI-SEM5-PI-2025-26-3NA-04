import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IncidentService,
  IncidentDto,
  CreateIncidentDto,
  UpdateIncidentDto,
  IncidentFilterDto
} from '../../services/incident';
import { IncidentTypeService, IncidentTypeDto } from '../../services/incident-type';
import { VesselVisitExecutionService, VesselVisitExecutionDto } from '../../services/vessel-visit-execution';

@Component({
  selector: 'app-incidents',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './incidents.html',
  styleUrl: './incidents.css',
})
export class Incidents implements OnInit {
  incidents: IncidentDto[] = [];
  filteredIncidents: IncidentDto[] = [];
  selectedIncident: IncidentDto | null = null;

  showCreateForm = false;
  showEditForm = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  incidentTypes: IncidentTypeDto[] = [];
  executions: VesselVisitExecutionDto[] = [];

  filters: IncidentFilterDto = {
    startDate: '',
    endDate: '',
    severity: undefined,
    status: undefined,
    incidentTypeId: undefined
  };

  newIncident: CreateIncidentDto = {
    incidentTypeId: '',
    startTime: '',
    severity: 'Minor',
    description: '',
    affectedVVEIds: [],
    affectsAllOngoingVVEs: false,
    affectsAllUpcomingVVEs: false
  };

  editIncident: UpdateIncidentDto = {
    endTime: '',
    severity: 'Minor',
    description: '',
    affectedVVEIds: [],
    affectsAllOngoingVVEs: false,
    affectsAllUpcomingVVEs: false,
    status: 'active'
  };

  selectedVveId = '';

  constructor(
    private incidentService: IncidentService,
    private incidentTypeService: IncidentTypeService,
    private executionService: VesselVisitExecutionService
  ) { }

  ngOnInit(): void {
    this.loadIncidents();
    this.loadIncidentTypes();
    this.loadExecutions();
  }

  loadIncidents(): void {
    this.loading = true;
    this.error = null;

    this.incidentService.getAll(this.filters).subscribe({
      next: (data) => {
        this.incidents = data;
        this.filteredIncidents = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load incidents: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  loadIncidentTypes(): void {
    this.incidentTypeService.getAll().subscribe({
      next: (data) => {
        this.incidentTypes = data;
      },
      error: (err) => {
        console.error('Failed to load incident types:', err);
      }
    });
  }

  loadExecutions(): void {
    this.executionService.getAll().subscribe({
      next: (data) => {
        this.executions = data;
      },
      error: (err) => {
        console.error('Failed to load executions:', err);
      }
    });
  }

  applyFilters(): void {
    this.loadIncidents();
  }

  clearFilters(): void {
    this.filters = {
      startDate: '',
      endDate: '',
      severity: undefined,
      status: undefined,
      incidentTypeId: undefined
    };
    this.loadIncidents();
  }

  selectIncident(incident: IncidentDto): void {
    this.selectedIncident = incident;
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

  openEditForm(incident: IncidentDto): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.selectedIncident = incident;
    this.editIncident = {
      endTime: incident.endTime || '',
      severity: incident.severity,
      description: incident.description,
      affectedVVEIds: [...incident.affectedVVEIds],
      affectsAllOngoingVVEs: incident.affectsAllOngoingVVEs,
      affectsAllUpcomingVVEs: incident.affectsAllUpcomingVVEs,
      status: incident.status
    };
  }

  closeEditForm(): void {
    this.showEditForm = false;
    this.editIncident = {
      endTime: '',
      severity: 'Minor',
      description: '',
      affectedVVEIds: [],
      affectsAllOngoingVVEs: false,
      affectsAllUpcomingVVEs: false,
      status: 'active'
    };
  }

  resetCreateForm(): void {
    this.newIncident = {
      incidentTypeId: '',
      startTime: '',
      severity: 'Minor',
      description: '',
      affectedVVEIds: [],
      affectsAllOngoingVVEs: false,
      affectsAllUpcomingVVEs: false
    };
    this.selectedVveId = '';
    this.error = null;
  }

  addVveToForm(): void {
    if (this.selectedVveId && !this.newIncident.affectedVVEIds?.includes(this.selectedVveId)) {
      if (!this.newIncident.affectedVVEIds) {
        this.newIncident.affectedVVEIds = [];
      }
      this.newIncident.affectedVVEIds.push(this.selectedVveId);
      this.selectedVveId = '';
    }
  }

  removeVveFromForm(vveId: string): void {
    if (this.newIncident.affectedVVEIds) {
      this.newIncident.affectedVVEIds = this.newIncident.affectedVVEIds.filter(id => id !== vveId);
    }
  }

  createIncident(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.validateCreateForm()) {
      return;
    }

    this.loading = true;

    this.incidentService.create(this.newIncident).subscribe({
      next: (created) => {
        this.successMessage = 'Incident created successfully!';
        this.incidents.push(created);
        this.filteredIncidents = [...this.incidents];
        this.loading = false;
        this.closeCreateForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to create incident: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  updateIncident(): void {
    if (!this.selectedIncident) return;

    this.error = null;
    this.successMessage = null;

    this.loading = true;

    this.incidentService.update(this.selectedIncident._id, this.editIncident).subscribe({
      next: (updated) => {
        this.successMessage = 'Incident updated successfully!';
        const index = this.incidents.findIndex(i => i._id === updated._id);
        if (index !== -1) {
          this.incidents[index] = updated;
        }
        if (this.selectedIncident?._id === updated._id) {
          this.selectedIncident = updated;
        }
        this.filteredIncidents = [...this.incidents];
        this.loading = false;
        this.closeEditForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to update incident: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  deleteIncident(id: string): void {
    if (confirm('Are you sure you want to delete this incident?')) {
      this.loading = true;
      this.error = null;

      this.incidentService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Incident deleted successfully!';
          this.incidents = this.incidents.filter(i => i._id !== id);
          this.filteredIncidents = [...this.incidents];
          if (this.selectedIncident?._id === id) {
            this.selectedIncident = null;
          }
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete incident: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  validateCreateForm(): boolean {
    if (!this.newIncident.incidentTypeId) {
      this.error = 'Incident type is required';
      return false;
    }
    if (!this.newIncident.startTime) {
      this.error = 'Start time is required';
      return false;
    }
    if (!this.newIncident.description || this.newIncident.description.trim() === '') {
      this.error = 'Description is required';
      return false;
    }
    return true;
  }

  getSeverityClass(severity: string): string {
    switch (severity) {
      case 'Minor':
        return 'severity-minor';
      case 'Major':
        return 'severity-major';
      case 'Critical':
        return 'severity-critical';
      default:
        return '';
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'active':
        return 'status-active';
      case 'resolved':
        return 'status-resolved';
      default:
        return '';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }

  formatDuration(minutes: number | undefined): string {
    if (!minutes) return 'N/A';
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins}m`;
  }
}

