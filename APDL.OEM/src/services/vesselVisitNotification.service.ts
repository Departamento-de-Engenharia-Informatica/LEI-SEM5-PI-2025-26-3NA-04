import axios, { AxiosInstance } from 'axios';
import https from 'https';

export interface VesselVisitNotificationDto {
  id: string;
  vesselId: string;
  shippingAgentId: string;
  expectedArrival: string; // ISO date string
  expectedDeparture: string; // ISO date string
  cargoType: string;
  cargoVolume: number;
  specialHandlingRequirements?: string;
  captainName: string;
  totalCrewCount: number;
  safetyCrewOfficers: string[];
  status: string;
  assignedDockId?: string;
  rejectionReason?: string;
  createdAt: string;
  submittedAt?: string;
  reviewedAt?: string;
}

export class VesselVisitNotificationService {
  private client: AxiosInstance;
  private baseUrl: string;

  constructor() {
    this.baseUrl = process.env.ASP_NET_API_URL || 'https://localhost:5001/api';
    
    const httpsAgent = new https.Agent({
      rejectUnauthorized: process.env.NODE_ENV === 'production' ? true : false,
    });
    
    this.client = axios.create({
      baseURL: this.baseUrl,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
      httpsAgent: httpsAgent,
    });
  }

  async getAllVesselVisitNotifications(
    authToken: string
  ): Promise<VesselVisitNotificationDto[]> {
    try {
      const response = await this.client.get<VesselVisitNotificationDto[]>(
        '/VesselVisitNotifications',
        {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        }
      );
      return response.data;
    } catch (error: any) {
      if (axios.isAxiosError(error)) {
        throw new Error(
          `Failed to fetch VVNs from ASP.NET backend: ${error.message}`
        );
      }
      throw error;
    }
  }

  async getVesselVisitNotificationsByStatus(
    authToken: string,
    status: string
  ): Promise<VesselVisitNotificationDto[]> {
    try {
      const response = await this.client.get<VesselVisitNotificationDto[]>(
        `/VesselVisitNotifications/status/${status}`,
        {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        }
      );
      return response.data;
    } catch (error: any) {
      if (axios.isAxiosError(error)) {
        throw new Error(
          `Failed to fetch VVNs by status from ASP.NET backend: ${error.message}`
        );
      }
      throw error;
    }
  }

  async getVesselVisitNotificationById(
    authToken: string,
    vvnId: string
  ): Promise<VesselVisitNotificationDto | null> {
    try {
      const response = await this.client.get<VesselVisitNotificationDto>(
        `/VesselVisitNotifications/${vvnId}`,
        {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        }
      );
      return response.data;
    } catch (error: any) {
      if (axios.isAxiosError(error)) {
        if (error.response?.status === 404) {
          return null;
        }
        if (error.response?.status === 500) {
          throw new Error(
            `ASP.NET backend error (500) for VVN ${vvnId}. Check your ASP.NET backend logs for details.`
          );
        }
        throw new Error(
          `Failed to fetch VVN from ASP.NET backend: ${error.message} (Status: ${error.response?.status || 'unknown'})`
        );
      }
      throw error;
    }
  }
}

export default new VesselVisitNotificationService();

