import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { 
  DockService, 
  DockDto,
  CreateDockDto,
  UpdateDockDto 
} from '../../services/dock';

@Component({
  selector: 'app-docks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './docks.html',
  styleUrl: './docks.css',
})
export class Docks implements OnInit {
  docks: DockDto[] = [];
  selectedDock: DockDto | null = null;
  
  showCreateForm = false;
  showEditForm = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  
  newDock: CreateDockDto = {
    dockName: '',
    dockLength: 0,
    dockDraft: 0
  };

  editDock: UpdateDockDto = {
    id: '',
    dockName: '',
    dockLength: 0,
    dockDraft: 0
  };

  vesselFilterLength: number = 0;
  vesselFilterDraft: number = 0;
  showVesselFilter = false;

  constructor(private dockService: DockService) {}

  ngOnInit(): void {
    this.loadDocks();
  }

  loadDocks(): void {
    this.loading = true;
    this.error = null;
    
    this.dockService.getAll().subscribe({
      next: (data) => {
        this.docks = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load docks: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  selectDock(dock: DockDto): void {
    this.selectedDock = dock;
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

  openEditForm(dock: DockDto): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.editDock = {
      id: dock.id,
      dockName: dock.dockName,
      dockLength: dock.dockLength,
      dockDraft: dock.dockDraft
    };
  }

  closeEditForm(): void {
    this.showEditForm = false;
    this.editDock = {
      id: '',
      dockName: '',
      dockLength: 0,
      dockDraft: 0
    };
  }

  resetCreateForm(): void {
    this.newDock = {
      dockName: '',
      dockLength: 0,
      dockDraft: 0
    };
    this.error = null;
  }

  createDock(): void {
    this.error = null;
    this.successMessage = null;
    
    if (!this.validateCreateForm()) {
      return;
    }

    this.loading = true;

    this.dockService.create(this.newDock).subscribe({
      next: (created) => {
        this.successMessage = 'Dock created successfully!';
        this.docks.push(created);
        this.loading = false;
        this.closeCreateForm();
        
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to create dock: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  updateDock(): void {
    this.error = null;
    this.successMessage = null;
    
    if (!this.validateEditForm()) {
      return;
    }

    this.loading = true;

    this.dockService.update(this.editDock.id, this.editDock).subscribe({
      next: (updated) => {
        this.successMessage = 'Dock updated successfully!';
        const index = this.docks.findIndex(d => d.id === updated.id);
        if (index !== -1) {
          this.docks[index] = updated;
        }
        if (this.selectedDock?.id === updated.id) {
          this.selectedDock = updated;
        }
        this.loading = false;
        this.closeEditForm();
        
        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to update dock: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  deleteDock(id: string): void {
    if (confirm('Are you sure you want to delete this dock?')) {
      this.loading = true;
      this.error = null;

      this.dockService.delete(id).subscribe({
        next: () => {
          this.successMessage = 'Dock deleted successfully!';
          this.docks = this.docks.filter(d => d.id !== id);
          if (this.selectedDock?.id === id) {
            this.selectedDock = null;
          }
          this.loading = false;
          
          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to delete dock: ' + (err.error?.error || err.message);
          this.loading = false;
        }
      });
    }
  }

  toggleVesselFilter(): void {
    this.showVesselFilter = !this.showVesselFilter;
    if (!this.showVesselFilter) {
      this.loadDocks();
    }
  }

  filterByVesselRequirements(): void {
    if (this.vesselFilterLength <= 0 || this.vesselFilterDraft <= 0) {
      this.error = 'Please enter valid vessel length and draft';
      return;
    }

    this.loading = true;
    this.error = null;

    this.dockService.getDocksCapableOfVessel(
      this.vesselFilterLength, 
      this.vesselFilterDraft
    ).subscribe({
      next: (data) => {
        this.docks = data;
        this.loading = false;
        if (data.length === 0) {
          this.error = 'No docks found that can accommodate this vessel';
        }
      },
      error: (err) => {
        this.error = 'Failed to filter docks: ' + (err.error?.error || err.message);
        this.loading = false;
      }
    });
  }

  validateCreateForm(): boolean {
    if (!this.newDock.dockName || this.newDock.dockName.trim() === '') {
      this.error = 'Dock name is required';
      return false;
    }
    if (this.newDock.dockLength <= 0) {
      this.error = 'Dock length must be greater than zero';
      return false;
    }
    if (this.newDock.dockDraft <= 0) {
      this.error = 'Dock draft must be greater than zero';
      return false;
    }
    return true;
  }

  validateEditForm(): boolean {
    if (this.editDock.dockName && this.editDock.dockName.trim() === '') {
      this.error = 'Dock name cannot be empty';
      return false;
    }
    if (this.editDock.dockLength !== undefined && this.editDock.dockLength <= 0) {
      this.error = 'Dock length must be greater than zero';
      return false;
    }
    if (this.editDock.dockDraft !== undefined && this.editDock.dockDraft <= 0) {
      this.error = 'Dock draft must be greater than zero';
      return false;
    }
    return true;
  }

  canAccommodateVessel(dock: DockDto, vesselLength: number, vesselDraft: number): boolean {
    return dock.dockLength >= vesselLength && dock.dockDraft >= vesselDraft;
  }
}