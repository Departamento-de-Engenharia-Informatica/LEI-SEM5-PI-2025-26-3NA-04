import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OemApiService } from './oem-api';

export interface IncidentDto {
  _id: string;
  incidentTypeId: string;
  startTime: string;
  endTime?: string;
  duration?: number;
  severity: 'Minor' | 'Major' | 'Critical';
  description: string;
  responsibleUser: string;
  affectedVVEIds: string[];
  affectsAllOngoingVVEs: boolean;
  affectsAllUpcomingVVEs: boolean;
  status: 'active' | 'resolved';
  createdAt: string;
  updatedAt: string;
}

export interface CreateIncidentDto {
  incidentTypeId: string;
  startTime: string;
  endTime?: string;
  severity: 'Minor' | 'Major' | 'Critical';
  description: string;
  affectedVVEIds?: string[];
  affectsAllOngoingVVEs?: boolean;
  affectsAllUpcomingVVEs?: boolean;
}

export interface UpdateIncidentDto {
  endTime?: string;
  severity?: 'Minor' | 'Major' | 'Critical';
  description?: string;
  affectedVVEIds?: string[];
  affectsAllOngoingVVEs?: boolean;
  affectsAllUpcomingVVEs?: boolean;
  status?: 'active' | 'resolved';
}

export interface IncidentFilterDto {
  startDate?: string;
  endDate?: string;
  severity?: 'Minor' | 'Major' | 'Critical';
  status?: 'active' | 'resolved';
  incidentTypeId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class IncidentService {
  private readonly endpoint = 'incidents';

  constructor(private apiService: OemApiService) { }

  getAll(filters?: IncidentFilterDto): Observable<IncidentDto[]> {
    let url = this.endpoint;
    if (filters) {
      const params = new URLSearchParams();
      if (filters.startDate) params.append('startDate', filters.startDate);
      if (filters.endDate) params.append('endDate', filters.endDate);
      if (filters.severity) params.append('severity', filters.severity);
      if (filters.status) params.append('status', filters.status);
      if (filters.incidentTypeId) params.append('incidentTypeId', filters.incidentTypeId);
      const queryString = params.toString();
      if (queryString) {
        url += `?${queryString}`;
      }
    }
    return this.apiService.get<IncidentDto[]>(url);
  }

  getById(id: string): Observable<IncidentDto> {
    return this.apiService.get<IncidentDto>(`${this.endpoint}/${id}`);
  }

  create(dto: CreateIncidentDto): Observable<IncidentDto> {
    return this.apiService.post<IncidentDto>(this.endpoint, dto);
  }

  update(id: string, dto: UpdateIncidentDto): Observable<IncidentDto> {
    return this.apiService.put<IncidentDto>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}

