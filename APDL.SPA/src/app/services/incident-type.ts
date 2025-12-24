import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OemApiService } from './oem-api';

export interface IncidentTypeDto {
  _id: string;
  code: string;
  name: string;
  description?: string;
  severity: 'Minor' | 'Major' | 'Critical';
  parentId?: string;
  children?: IncidentTypeDto[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateIncidentTypeDto {
  name: string;
  description?: string;
  severity: 'Minor' | 'Major' | 'Critical';
  parentId?: string;
}

export interface UpdateIncidentTypeDto {
  name?: string;
  description?: string;
  severity?: 'Minor' | 'Major' | 'Critical';
  parentId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class IncidentTypeService {
  private readonly endpoint = 'incident-types';

  constructor(private apiService: OemApiService) { }

  getAll(): Observable<IncidentTypeDto[]> {
    return this.apiService.get<IncidentTypeDto[]>(this.endpoint);
  }

  getById(id: string): Observable<IncidentTypeDto> {
    return this.apiService.get<IncidentTypeDto>(`${this.endpoint}/${id}`);
  }

  getHierarchy(): Observable<IncidentTypeDto[]> {
    return this.apiService.get<IncidentTypeDto[]>(`${this.endpoint}/hierarchy`);
  }

  create(dto: CreateIncidentTypeDto): Observable<IncidentTypeDto> {
    return this.apiService.post<IncidentTypeDto>(this.endpoint, dto);
  }

  update(id: string, dto: UpdateIncidentTypeDto): Observable<IncidentTypeDto> {
    return this.apiService.put<IncidentTypeDto>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}

