import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OemApiService } from './oem-api';

export interface VesselScheduleEntry {
  vesselId: string;
  vvnId: string;
  vesselLabel: string;
  arrivalTime: number;
  unloadStartTime: number;
  loadEndTime: number;
  dockId: string;
  craneId: string;
  staffId: string;
  storageId: string;
  timeFactor: number;
  delay: number;
  unloadTimeUnits: number;
  loadTimeUnits: number;
}

export interface OperationPlanDto {
  _id: string;
  planId: string;
  vvnIds: string[];
  schedule: VesselScheduleEntry[];
  status: 'Generated' | 'Manual' | 'Approved';
  totalDelay: number;
  algorithmUsed: string;
  creatorUserId: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateOperationPlanDto {
  vvnIds: string[];
  algorithm?: string;
  notes?: string;
}

export interface UpdateOperationPlanDto {
  schedule?: VesselScheduleEntry[];
  status?: 'Generated' | 'Manual' | 'Approved';
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class OperationPlanService {
  private readonly endpoint = 'operation-plans';

  constructor(private apiService: OemApiService) { }

  getAll(): Observable<OperationPlanDto[]> {
    return this.apiService.get<OperationPlanDto[]>(this.endpoint);
  }

  getById(id: string): Observable<OperationPlanDto> {
    return this.apiService.get<OperationPlanDto>(`${this.endpoint}/${id}`);
  }

  generate(dto: CreateOperationPlanDto): Observable<OperationPlanDto> {
    return this.apiService.post<OperationPlanDto>(`${this.endpoint}/generate`, dto);
  }

  update(id: string, dto: UpdateOperationPlanDto): Observable<OperationPlanDto> {
    return this.apiService.put<OperationPlanDto>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}

