import { VesselVisitExecution, IVesselVisitExecution } from '../models/VesselVisitExecution';
import { v4 as uuidv4 } from 'uuid';

export interface CreateVVEDto {
  vvnId: string;
  vesselId: string;
  actualArrivalTime: Date;
  creatorUserId: string;
}

export interface UpdateVVEDto {
  actualBerthTime?: Date;
  actualDockId?: string;
  actualUnberthTime?: Date;
  actualDepartureTime?: Date;
  status?: 'In Progress' | 'Completed';
}

export class VesselVisitExecutionService {
  // Generate VVE ID following VVN ID pattern (GUID)
  private generateVVEId(): string {
    return uuidv4();
  }

  // Create a new VVE
  async create(data: CreateVVEDto): Promise<IVesselVisitExecution> {
    // Validate that VVN ID and Vessel ID are provided
    if (!data.vvnId || !data.vesselId) {
      throw new Error('VVN ID and Vessel ID are required');
    }

    // Generate VVE ID (GUID pattern like VVN IDs)
    const vveId = this.generateVVEId();

    // Check if VVE already exists for this VVN
    const existingVVE = await VesselVisitExecution.findOne({ vvnId: data.vvnId });
    if (existingVVE) {
      throw new Error('VVE already exists for this VVN');
    }

    // Create new VVE with status "In Progress"
    const vve = new VesselVisitExecution({
      vveId,
      vvnId: data.vvnId,
      vesselId: data.vesselId,
      actualArrivalTime: data.actualArrivalTime,
      creatorUserId: data.creatorUserId,
      status: 'In Progress',
    });

    return await vve.save();
  }

  // Get VVE by ID
  async getById(vveId: string): Promise<IVesselVisitExecution | null> {
    return await VesselVisitExecution.findOne({ vveId });
  }

  // Get VVE by VVN ID
  async getByVVNId(vvnId: string): Promise<IVesselVisitExecution | null> {
    return await VesselVisitExecution.findOne({ vvnId });
  }

  // Get all VVEs
  async getAll(): Promise<IVesselVisitExecution[]> {
    return await VesselVisitExecution.find().sort({ actualArrivalTime: -1 });
  }

  // Get VVEs by vessel ID
  async getByVesselId(vesselId: string): Promise<IVesselVisitExecution[]> {
    return await VesselVisitExecution.find({ vesselId }).sort({ actualArrivalTime: -1 });
  }

  // Get VVEs by status
  async getByStatus(status: 'In Progress' | 'Completed'): Promise<IVesselVisitExecution[]> {
    return await VesselVisitExecution.find({ status }).sort({ actualArrivalTime: -1 });
  }

  // Get VVEs by date range
  async getByDateRange(startDate: Date, endDate: Date): Promise<IVesselVisitExecution[]> {
    return await VesselVisitExecution.find({
      actualArrivalTime: {
        $gte: startDate,
        $lte: endDate,
      },
    }).sort({ actualArrivalTime: -1 });
  }

  // Update VVE
  async update(vveId: string, data: UpdateVVEDto): Promise<IVesselVisitExecution | null> {
    const vve = await VesselVisitExecution.findOne({ vveId });
    if (!vve) {
      return null;
    }

    // Update fields
    if (data.actualBerthTime !== undefined) {
      vve.actualBerthTime = data.actualBerthTime;
    }
    if (data.actualDockId !== undefined) {
      vve.actualDockId = data.actualDockId;
    }
    if (data.actualUnberthTime !== undefined) {
      vve.actualUnberthTime = data.actualUnberthTime;
    }
    if (data.actualDepartureTime !== undefined) {
      vve.actualDepartureTime = data.actualDepartureTime;
    }
    if (data.status !== undefined) {
      vve.status = data.status;
    }

    return await vve.save();
  }

  // Delete VVE
  async delete(vveId: string): Promise<boolean> {
    const result = await VesselVisitExecution.deleteOne({ vveId });
    return result.deletedCount > 0;
  }
}

export const vesselVisitExecutionService = new VesselVisitExecutionService();

