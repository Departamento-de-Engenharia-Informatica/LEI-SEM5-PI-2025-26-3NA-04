import { IncidentModel, IIncident } from '../models/Incident';
import { IncidentTypeModel } from '../models/IncidentType';

export class IncidentService {
  // Get all incidents
  async getAll(): Promise<IIncident[]> {
    return await IncidentModel.find()
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 }); // Most recent first
  }

  // Get incident by ID
  async getById(id: string): Promise<IIncident | null> {
    return await IncidentModel.findById(id).populate('incidentTypeId', 'code name severity');
  }

  // Get active incidents
  async getActiveIncidents(): Promise<IIncident[]> {
    return await IncidentModel.find({ status: 'active' })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Get resolved incidents
  async getResolvedIncidents(): Promise<IIncident[]> {
    return await IncidentModel.find({ status: 'resolved' })
      .populate('incidentTypeId', 'code name severity')
      .sort({ endTime: -1 });
  }

  // Filter by severity
  async filterBySeverity(severity: string): Promise<IIncident[]> {
    return await IncidentModel.find({ severity })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Filter by date range
  async filterByDateRange(startDate: Date, endDate: Date): Promise<IIncident[]> {
    return await IncidentModel.find({
      startTime: { $gte: startDate, $lte: endDate },
    })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Filter by VVE ID
  async filterByVVEId(vveId: string): Promise<IIncident[]> {
    return await IncidentModel.find({
      $or: [
        { affectedVVEIds: vveId },
        { affectsAllOngoingVVEs: true },
        { affectsAllUpcomingVVEs: true },
      ],
    })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Get incidents affecting a specific VVE
  async getIncidentsAffectingVVE(vveId: string): Promise<IIncident[]> {
    return await IncidentModel.find({
      $or: [
        { affectedVVEIds: vveId },
        { affectsAllOngoingVVEs: true },
        { affectsAllUpcomingVVEs: true },
      ],
      status: 'active', // Only active incidents
    })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Create new incident
  async create(data: Partial<IIncident>): Promise<IIncident> {
    // Validate incident type exists
    const incidentType = await IncidentTypeModel.findById(data.incidentTypeId);
    if (!incidentType) {
      throw new Error('Incident type not found');
    }

    // Validate that at least one affect option is selected
    const hasSpecificVVEs = data.affectedVVEIds && data.affectedVVEIds.length > 0;
    const affectsAll = data.affectsAllOngoingVVEs || data.affectsAllUpcomingVVEs;

    if (!hasSpecificVVEs && !affectsAll) {
      throw new Error('Incident must affect at least one VVE or all ongoing/upcoming VVEs');
    }

    // Validate start time is not in the future (or allow it if needed)
    if (data.startTime && data.startTime > new Date()) {
      // Allow future incidents for planning purposes
      // Remove this check if you want to allow future incidents
    }

    // Set initial status
    data.status = data.endTime === null ? 'active' : 'resolved';

    const incident = new IncidentModel(data);
    return await incident.save();
  }

  // Update incident
  async update(id: string, data: Partial<IIncident>): Promise<IIncident | null> {
    const incident = await IncidentModel.findById(id);
    if (!incident) {
      return null;
    }

    // If updating incident type, validate it exists
    if (data.incidentTypeId) {
      const incidentType = await IncidentTypeModel.findById(data.incidentTypeId);
      if (!incidentType) {
        throw new Error('Incident type not found');
      }
    }

    // If setting endTime, compute duration
    if (data.endTime !== undefined) {
      if (data.endTime === null) {
        // Resolving an incident
        data.status = 'active';
        data.duration = null;
      } else {
        // Resolving an incident
        data.status = 'resolved';
        const startTime = data.startTime || incident.startTime;
        const diffMs = data.endTime.getTime() - startTime.getTime();
        data.duration = Math.round(diffMs / (1000 * 60)); // Convert to minutes

        // Validate endTime is after startTime
        if (data.endTime <= startTime) {
          throw new Error('End time must be after start time');
        }
      }
    }

    // Validate that at least one affect option is selected
    if (data.affectedVVEIds !== undefined || data.affectsAllOngoingVVEs !== undefined || data.affectsAllUpcomingVVEs !== undefined) {
      const hasSpecificVVEs = (data.affectedVVEIds || incident.affectedVVEIds).length > 0;
      const affectsAll = (data.affectsAllOngoingVVEs ?? incident.affectsAllOngoingVVEs) || 
                         (data.affectsAllUpcomingVVEs ?? incident.affectsAllUpcomingVVEs);

      if (!hasSpecificVVEs && !affectsAll) {
        throw new Error('Incident must affect at least one VVE or all ongoing/upcoming VVEs');
      }
    }

    return await IncidentModel.findByIdAndUpdate(id, data, {
      new: true,
      runValidators: true,
    }).populate('incidentTypeId', 'code name severity');
  }

  // Resolve incident (set end time)
  async resolveIncident(id: string, endTime?: Date): Promise<IIncident | null> {
    const incident = await IncidentModel.findById(id);
    if (!incident) {
      return null;
    }

    if (incident.status === 'resolved') {
      throw new Error('Incident is already resolved');
    }

    const resolutionTime = endTime || new Date();
    
    if (resolutionTime <= incident.startTime) {
      throw new Error('End time must be after start time');
    }

    return await this.update(id, { endTime: resolutionTime });
  }

  // Delete incident
  async delete(id: string): Promise<boolean> {
    const result = await IncidentModel.findByIdAndDelete(id);
    return !!result;
  }

  // Search incidents by description
  async search(query: string): Promise<IIncident[]> {
    return await IncidentModel.find({
      $text: { $search: query },
    })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }

  // Get incidents by responsible user
  async getByResponsibleUser(userEmail: string): Promise<IIncident[]> {
    return await IncidentModel.find({ responsibleUser: userEmail })
      .populate('incidentTypeId', 'code name severity')
      .sort({ startTime: -1 });
  }
}

