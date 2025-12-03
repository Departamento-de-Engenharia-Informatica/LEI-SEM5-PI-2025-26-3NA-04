import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface DockDto {
  id: string;
  dockName: string;
  dockLength: number;
  dockDraft: number;
  numberOfSTSCranes: number;
  stsCranes?: StsCraneDto[];
  upcomingMaintenances?: MaintenanceScheduleDto[];
}

export interface StsCraneDto {
  id: string;
  craneName: string;
  dockId: string;
  operationalWindow: string;
  capacityContainersPerHour: number;
  status: string;
  requiredOperators: number;
  requiredQualification: string;
  setupTime: string;
}

export interface MaintenanceScheduleDto {
  startDate: string;
  endDate: string;
  description: string;
}

export interface CreateDockDto {
  dockName: string;
  dockLength: number;
  dockDraft: number;
}

export interface UpdateDockDto {
  id: string;
  dockName?: string;
  dockLength?: number;
  dockDraft?: number;
}

@Injectable({
  providedIn: 'root'
})
export class DockService {
  private readonly endpoint = 'Docks';

  constructor(private apiService: ApiService) { }

  getAll(): Observable<DockDto[]> {
    return this.apiService.get<DockDto[]>(this.endpoint);
  }

  getById(id: string): Observable<DockDto> {
    return this.apiService.get<DockDto>(`${this.endpoint}/${id}`);
  }

  getByName(dockName: string): Observable<DockDto> {
    return this.apiService.get<DockDto>(`${this.endpoint}/name/${dockName}`);
  }

  getDocksCapableOfVessel(vesselLength: number, vesselDraft: number): Observable<DockDto[]> {
    return this.apiService.get<DockDto[]>(
      `${this.endpoint}/capable?vesselLength=${vesselLength}&vesselDraft=${vesselDraft}`
    );
  }

  create(dto: CreateDockDto): Observable<DockDto> {
    return this.apiService.post<DockDto>(this.endpoint, dto);
  }

  update(id: string, dto: UpdateDockDto): Observable<DockDto> {
    return this.apiService.put<DockDto>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}