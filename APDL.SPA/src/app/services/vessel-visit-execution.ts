import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OemApiService } from './oem-api';

export interface VesselVisitExecutionDto {
  _id: string;
  vvnId: string;
  vesselId: string;
  actualArrivalTime: string;
  creatorUserId: string;
  status: 'In Progress' | 'Completed';
  actualBerthTime?: string;
  actualDockId?: string;
  actualUnberthTime?: string;
  actualDepartureTime?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateVesselVisitExecutionDto {
  vvnId: string;
  vesselId: string;
  actualArrivalTime: string;
}

export interface UpdateVesselVisitExecutionDto {
  status?: 'In Progress' | 'Completed';
  actualBerthTime?: string;
  actualDockId?: string;
  actualUnberthTime?: string;
  actualDepartureTime?: string;
}

@Injectable({
  providedIn: 'root'
})
export class VesselVisitExecutionService {
  private readonly endpoint = 'vessel-visit-executions';

  constructor(private apiService: OemApiService) { }

  getAll(): Observable<VesselVisitExecutionDto[]> {
    return this.apiService.get<VesselVisitExecutionDto[]>(this.endpoint);
  }

  getById(id: string): Observable<VesselVisitExecutionDto> {
    return this.apiService.get<VesselVisitExecutionDto>(`${this.endpoint}/${id}`);
  }

  create(dto: CreateVesselVisitExecutionDto): Observable<VesselVisitExecutionDto> {
    return this.apiService.post<VesselVisitExecutionDto>(this.endpoint, dto);
  }

  update(id: string, dto: UpdateVesselVisitExecutionDto): Observable<VesselVisitExecutionDto> {
    return this.apiService.put<VesselVisitExecutionDto>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}

