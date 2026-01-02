import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface PrivacyPolicyDto {
  id: string;
  version: number;
  content: string;
  effectiveDate: string;
  isActive: boolean;
  createdBy: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreatePrivacyPolicyDto {
  content: string;
  effectiveDate: string;
}

export interface UpdatePrivacyPolicyDto {
  content: string;
}

export interface AcknowledgePrivacyPolicyDto {
  version: number;
}

@Injectable({
  providedIn: 'root'
})
export class PrivacyPolicyService {
  private readonly endpoint = 'PrivacyPolicy';

  constructor(private apiService: ApiService) { }

  getActive(): Observable<PrivacyPolicyDto> {
    return this.apiService.get<PrivacyPolicyDto>(`${this.endpoint}/active`);
  }

  getAllVersions(): Observable<PrivacyPolicyDto[]> {
    return this.apiService.get<PrivacyPolicyDto[]>(`${this.endpoint}/versions`);
  }

  getByVersion(version: number): Observable<PrivacyPolicyDto> {
    return this.apiService.get<PrivacyPolicyDto>(`${this.endpoint}/versions/${version}`);
  }

  publishNewVersion(dto: CreatePrivacyPolicyDto): Observable<PrivacyPolicyDto> {
    return this.apiService.post<PrivacyPolicyDto>(this.endpoint, dto);
  }

  update(id: string, dto: UpdatePrivacyPolicyDto): Observable<PrivacyPolicyDto> {
    return this.apiService.put<PrivacyPolicyDto>(`${this.endpoint}/${id}`, dto);
  }

  acknowledge(version: number): Observable<void> {
    return this.apiService.post<void>(`${this.endpoint}/acknowledge`, { version });
  }
}

