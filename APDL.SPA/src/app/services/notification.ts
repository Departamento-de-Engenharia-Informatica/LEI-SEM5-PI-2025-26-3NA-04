import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface VesselVisitNotificationDto {
    id: string;
    vesselId: string;
    shippingAgentId: string;
    expectedArrival: string;
    expectedDeparture: string;
    cargoType: string;
    cargoVolume: number;
    specialHandlingRequirements?: string;
    captainName: string;
    totalCrewCount: number;
    safetyCrewOfficers: SafetyOfficerDto[];
    cargoManifestIds: string[];
    status: string;
    assignedDockId?: string;
    rejectionReason?: string;
    createdAt: string;
    submittedAt?: string;
    reviewedAt?: string;
}

export interface SafetyOfficerDto {
    name: string;
}

export interface CreateVesselVisitNotificationDto {
    vesselId: string;
    shippingAgentId: string;
    expectedArrival: string;
    expectedDeparture: string;
    cargoType: string;
    cargoVolume: number;
    specialHandlingRequirements?: string;
    captainName: string;
    totalCrewCount: number;
    safetyCrewOfficerNames?: string[];
}

export interface UpdateVesselVisitNotificationDto {
    id: string;
    cargoType?: string;
    cargoVolume?: number;
    specialHandlingRequirements?: string;
    captainName?: string;
    totalCrewCount?: number;
    expectedArrival?: string;
    expectedDeparture?: string;
}

@Injectable({
    providedIn: 'root'
})
    export class Notification {
        private readonly endpoint = 'VesselVisitNotifications';

        constructor(private apiService: ApiService) { }

    getAll(): Observable<VesselVisitNotificationDto[]> {
        return this.apiService.get<VesselVisitNotificationDto[]>(this.endpoint);
    }

    getById(id: string): Observable<VesselVisitNotificationDto> {
        return this.apiService.get<VesselVisitNotificationDto>(`${this.endpoint}/${id}`);
    }

    getByStatus(status: string): Observable<VesselVisitNotificationDto[]> {
        return this.apiService.get<VesselVisitNotificationDto[]>(`${this.endpoint}/status/${status}`);
    }

    getPending(): Observable<VesselVisitNotificationDto[]> {
        return this.apiService.get<VesselVisitNotificationDto[]>(`${this.endpoint}/pending`);
    }

    create(dto: CreateVesselVisitNotificationDto): Observable<VesselVisitNotificationDto> {
        return this.apiService.post<VesselVisitNotificationDto>(this.endpoint, dto);
    }

    update(id: string, dto: UpdateVesselVisitNotificationDto): Observable<VesselVisitNotificationDto> {
        return this.apiService.put<VesselVisitNotificationDto>(`${this.endpoint}/${id}`, dto);
    }

    submit(id: string): Observable<void> {
        return this.apiService.post<void>(`${this.endpoint}/${id}/submit`, {});
    }

    addSafetyOfficer(id: string, officerName: string): Observable<void> {
        return this.apiService.post<void>(`${this.endpoint}/${id}/safety-officers`, { officerName });
    }

    removeSafetyOfficer(id: string, officerName: string): Observable<void> {
        return this.apiService.delete<void>(`${this.endpoint}/${id}/safety-officers/${officerName}`);
    }

    delete(id: string): Observable<void> {
        return this.apiService.delete<void>(`${this.endpoint}/${id}`);
    }
}