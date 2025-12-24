import { OperationPlan, IOperationPlan, IVesselScheduleEntry } from '../models/OperationPlan';
import vesselVisitNotificationService, {
  VesselVisitNotificationDto,
} from './vesselVisitNotification.service';
import prologService, {
  PrologVesselFact,
  PrologScheduleResult,
} from './prolog.service';
import { v4 as uuidv4 } from 'uuid';

export interface CreateOperationPlanDto {
  vvnIds: string[];
  algorithm?: string;
  notes?: string;
}

export interface UpdateOperationPlanDto {
  schedule?: IVesselScheduleEntry[];
  status?: 'Generated' | 'Manual' | 'Approved';
  notes?: string;
}

export class OperationPlanService {
  async generateOperationPlan(
    dto: CreateOperationPlanDto,
    authToken: string,
    creatorUserId: string
  ): Promise<IOperationPlan> {
    const vvns = await this.fetchVVNs(dto.vvnIds, authToken);
    if (vvns.length === 0) {
      throw new Error('No VVNs found with the provided IDs');
    }

    const prologInstalled = await prologService.checkPrologInstalled();
    if (!prologInstalled) {
      throw new Error('SWI-Prolog is not installed');
    }

    const baseTime = this.findBaseTime(vvns);
    const vesselFacts = prologService.convertVVNsToPrologFacts(vvns, baseTime);
    const prologResults = await prologService.executePrologScheduling(vesselFacts);
    const schedule = this.mapPrologResultsToSchedule(prologResults, vesselFacts, vvns);
    const totalDelay = schedule.reduce((sum, entry) => sum + entry.delay, 0);
    const plan = new OperationPlan({
      planId: uuidv4(),
      vvnIds: dto.vvnIds,
      schedule,
      status: 'Generated',
      totalDelay,
      algorithmUsed: dto.algorithm || 'original',
      creatorUserId,
      notes: dto.notes,
    });

    return await plan.save();
  }

  private async fetchVVNs(
    vvnIds: string[],
    authToken: string
  ): Promise<VesselVisitNotificationDto[]> {
    const vvns: VesselVisitNotificationDto[] = [];

    for (const vvnId of vvnIds) {
      const vvn = await vesselVisitNotificationService.getVesselVisitNotificationById(
        authToken,
        vvnId
      );
      if (vvn) {
        vvns.push(vvn);
      }
    }

    return vvns;
  }

  private findBaseTime(vvns: VesselVisitNotificationDto[]): Date {
    const arrivalTimes = vvns.map(
      (vvn) => new Date(vvn.expectedArrival)
    );
    return new Date(Math.min(...arrivalTimes.map((d) => d.getTime())));
  }

  private mapPrologResultsToSchedule(
    prologResults: PrologScheduleResult[],
    vesselFacts: PrologVesselFact[],
    vvns: VesselVisitNotificationDto[]
  ): IVesselScheduleEntry[] {
    const schedule: IVesselScheduleEntry[] = [];

    const labelToVvn = new Map<string, VesselVisitNotificationDto>();
    vesselFacts.forEach((fact, index) => {
      labelToVvn.set(fact.vesselLabel, vvns[index]);
    });

    const labelToFact = new Map<string, PrologVesselFact>();
    vesselFacts.forEach((fact) => {
      labelToFact.set(fact.vesselLabel, fact);
    });
    prologResults.forEach((result) => {
      const vvn = labelToVvn.get(result.vesselLabel);
      const fact = labelToFact.get(result.vesselLabel);

      if (!vvn || !fact) {
        throw new Error(
          `Could not find VVN or fact for vessel label: ${result.vesselLabel}`
        );
      }

      schedule.push({
        vesselId: vvn.vesselId,
        vvnId: vvn.id,
        vesselLabel: result.vesselLabel,
        arrivalTime: fact.arrivalTime,
        unloadStartTime: result.unloadStartTime,
        loadEndTime: result.loadEndTime,
        dockId: result.dockId || 'not_assigned',
        craneId: result.craneId || 'not_assigned',
        staffId: result.staffId || 'not_assigned',
        storageId: result.storageId || 'not_assigned',
        timeFactor: result.timeFactor || 1.0,
        delay: result.delay,
        unloadTimeUnits: fact.unloadTime,
        loadTimeUnits: fact.loadTime,
      });
    });

    return schedule;
  }

  async getAllOperationPlans(): Promise<IOperationPlan[]> {
    return await OperationPlan.find().sort({ createdAt: -1 });
  }

  async getOperationPlanById(planId: string): Promise<IOperationPlan | null> {
    return await OperationPlan.findOne({ planId });
  }

  async updateOperationPlan(
    planId: string,
    dto: UpdateOperationPlanDto
  ): Promise<IOperationPlan | null> {
    const plan = await OperationPlan.findOne({ planId });
    if (!plan) {
      return null;
    }

    if (dto.schedule) {
      this.validateSchedule(dto.schedule);
      dto.status = dto.status || 'Manual';
    }
    if (dto.schedule) {
      plan.schedule = dto.schedule;
      plan.totalDelay = dto.schedule.reduce((sum, entry) => sum + entry.delay, 0);
    }
    if (dto.status) {
      plan.status = dto.status;
    }
    if (dto.notes !== undefined) {
      plan.notes = dto.notes;
    }

    return await plan.save();
  }

  private validateSchedule(schedule: IVesselScheduleEntry[]): void {
    const dockTimeSlots = new Map<string, Set<number>>();

    for (const entry of schedule) {
      const dockKey = entry.dockId;
      if (!dockTimeSlots.has(dockKey)) {
        dockTimeSlots.set(dockKey, new Set());
      }

      const timeSlots = dockTimeSlots.get(dockKey)!;
      
      for (let time = entry.unloadStartTime; time <= entry.loadEndTime; time++) {
        if (timeSlots.has(time)) {
          throw new Error(
            `Dock conflict: ${dockKey} is already occupied at time ${time}`
          );
        }
        timeSlots.add(time);
      }
    }
  }

  async deleteOperationPlan(planId: string): Promise<boolean> {
    const result = await OperationPlan.deleteOne({ planId });
    return result.deletedCount > 0;
  }
}

export default new OperationPlanService();

