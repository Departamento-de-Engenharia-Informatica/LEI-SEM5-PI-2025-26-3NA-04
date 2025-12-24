import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IncidentTypeService,
  IncidentTypeDto,
  CreateIncidentTypeDto,
  UpdateIncidentTypeDto
} from '../../services/incident-type';

@Component({
  selector: 'app-incident-types',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './incident-types.html',
  styleUrl: './incident-types.css',
})
export class IncidentTypes implements OnInit {
  incidentTypes: IncidentTypeDto[] = [];
  hierarchicalTypes: IncidentTypeDto[] = [];
  selectedType: IncidentTypeDto | null = null;

  showCreateForm = false;
  showEditForm = false;
  showHierarchy = true;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  newType: CreateIncidentTypeDto = {
    name: '',
    description: '',
    severity: 'Minor',
    parentId: undefined
  };

  editType: UpdateIncidentTypeDto = {
    name: '',
    description: '',
    severity: 'Minor',
    parentId: undefined
  };

  constructor(private incidentTypeService: IncidentTypeService) { }

  ngOnInit(): void {
    this.loadIncidentTypes();
  }

  loadIncidentTypes(): void {
    this.loading = true;
    this.error = null;

    if (this.showHierarchy) {
      this.incidentTypeService.getHierarchy().subscribe({
        next: (data) => {
          this.hierarchicalTypes = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load incident types: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    } else {
      this.incidentTypeService.getAll().subscribe({
        next: (data) => {
          this.incidentTypes = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load incident types: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  toggleView(): void {
    this.showHierarchy = !this.showHierarchy;
    this.loadIncidentTypes();
  }

  selectType(type: IncidentTypeDto): void {
    this.selectedType = type;
  }

  openCreateForm(parentId?: string): void {
    this.showCreateForm = true;
    this.showEditForm = false;
    this.resetCreateForm();
    if (parentId) {
      this.newType.parentId = parentId;
    }
  }

  closeCreateForm(): void {
    this.showCreateForm = false;
    this.resetCreateForm();
  }

  openEditForm(type: IncidentTypeDto): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.selectedType = type;
    this.editType = {
      name: type.name,
      description: type.description,
      severity: type.severity,
      parentId: type.parentId
    };
  }

  closeEditForm(): void {
    this.showEditForm = false;
    this.editType = {
      name: '',
      description: '',
      severity: 'Minor',
      parentId: undefined
    };
  }

  resetCreateForm(): void {
    this.newType = {
      name: '',
      description: '',
      severity: 'Minor',
      parentId: undefined
    };
    this.error = null;
  }

  createType(): void {
    this.error = null;
    this.successMessage = null;

    if (!this.validateCreateForm()) {
      return;
    }

    this.loading = true;

    this.incidentTypeService.create(this.newType).subscribe({
      next: (created) => {
        this.successMessage = 'Incident type created successfully!';
        this.loadIncidentTypes();
        this.loading = false;
        this.closeCreateForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to create incident type: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  updateType(): void {
    if (!this.selectedType) return;

    this.error = null;
    this.successMessage = null;

    this.loading = true;

    this.incidentTypeService.update(this.selectedType._id, this.editType).subscribe({
      next: (updated) => {
        this.successMessage = 'Incident type updated successfully!';
        this.loadIncidentTypes();
        this.loading = false;
        this.closeEditForm();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to update incident type: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  deleteType(id: string): void {
    if (confirm('Are you sure you want to delete this incident type? This will also delete all child types.')) {
      this.loading = true;
      this.error = null;

      this.incidentTypeService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Incident type deleted successfully!';
          this.loadIncidentTypes();
          if (this.selectedType?._id === id) {
            this.selectedType = null;
          }
          this.loading = false;

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete incident type: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  validateCreateForm(): boolean {
    if (!this.newType.name || this.newType.name.trim() === '') {
      this.error = 'Name is required';
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

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString();
  }
}

