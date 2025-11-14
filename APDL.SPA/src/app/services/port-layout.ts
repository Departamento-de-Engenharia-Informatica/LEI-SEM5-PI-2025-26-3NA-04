import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface Position {
  x: number;
  y: number;
  z: number;
}

export interface Dimensions {
  width: number;
  height: number;
  depth: number;
}

export interface STSCrane {
  id: string;
  position: Position;
  height: number;
}

export interface Dock {
  id: string;
  position: Position;
  dimensions: Dimensions;
  stsCranes: STSCrane[];
}

export interface ContainerYard {
  id: string;
  position: Position;
  dimensions: Dimensions;
  capacity: number;
}

export interface Warehouse {
  id: string;
  position: Position;
  dimensions: Dimensions;
}

export interface PortLayout {
  id: string;
  name: string;
  docks: Dock[];
  containerYards: ContainerYard[];
  warehouses: Warehouse[];
}

@Injectable({
  providedIn: 'root'
})
export class PortLayoutService {
  private apiUrl = 'https://localhost:5001/api/portlayout';

  constructor(private http: HttpClient) { }

  getPortLayout(layoutId?: string): Observable<PortLayout> {
    const url = layoutId ? `${this.apiUrl}/${layoutId}` : this.apiUrl;
    return this.http.get<PortLayout>(url);
  }

  getAvailableLayouts(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/list`);
  }
}