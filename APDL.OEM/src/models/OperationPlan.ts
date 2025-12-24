import { Schema, model, Document } from 'mongoose';

// Schedule entry for a single vessel operation
export interface IVesselScheduleEntry {
  vesselId: string; // Vessel identifier from ASP.NET backend
  vvnId: string; // Vessel Visit Notification ID
  vesselLabel: string; // Short label for Prolog (e.g., "va", "vb")
  arrivalTime: number; // Arrival time in time units (1 unit = 10 minutes)
  unloadStartTime: number; // When unloading starts (time units)
  loadEndTime: number; // When loading ends (time units)
  dockId: string; // Assigned dock ID
  craneId: string; // Assigned crane ID
  staffId: string; // Assigned staff ID
  storageId: string; // Assigned storage ID
  timeFactor: number; // Crane efficiency factor
  delay: number; // Delay in time units (0 if on time)
  unloadTimeUnits: number; // Unload duration in time units
  loadTimeUnits: number; // Load duration in time units
}

export interface IOperationPlan extends Document {
  planId: string; // Auto-generated plan identifier (GUID)
  vvnIds: string[]; // Array of VVN IDs used to generate this plan
  schedule: IVesselScheduleEntry[]; // Vessel operation schedule
  status: 'Generated' | 'Manual' | 'Approved'; // Plan status
  totalDelay: number; // Total delay across all vessels (time units)
  algorithmUsed: string; // Which Prolog algorithm was used (e.g., "optimal_scheduler")
  creatorUserId: string; // User ID from Auth0 token
  notes?: string; // Optional notes about the plan
  createdAt: Date;
  updatedAt: Date;
}

const VesselScheduleEntrySchema = new Schema<IVesselScheduleEntry>(
  {
    vesselId: {
      type: String,
      required: true,
    },
    vvnId: {
      type: String,
      required: true,
    },
    vesselLabel: {
      type: String,
      required: true,
    },
    arrivalTime: {
      type: Number,
      required: true,
    },
    unloadStartTime: {
      type: Number,
      required: true,
    },
    loadEndTime: {
      type: Number,
      required: true,
    },
    dockId: {
      type: String,
      required: true,
    },
    craneId: {
      type: String,
      required: true,
    },
    staffId: {
      type: String,
      required: true,
    },
    storageId: {
      type: String,
      required: true,
    },
    timeFactor: {
      type: Number,
      required: true,
    },
    delay: {
      type: Number,
      default: 0,
    },
    unloadTimeUnits: {
      type: Number,
      required: true,
    },
    loadTimeUnits: {
      type: Number,
      required: true,
    },
  },
  { _id: false }
);

const OperationPlanSchema = new Schema<IOperationPlan>(
  {
    planId: {
      type: String,
      required: [true, 'Plan ID is required'],
      unique: true,
      index: true,
    },
    vvnIds: {
      type: [String],
      required: [true, 'VVN IDs are required'],
      index: true,
    },
    schedule: {
      type: [VesselScheduleEntrySchema],
      required: [true, 'Schedule is required'],
    },
    status: {
      type: String,
      enum: ['Generated', 'Manual', 'Approved'],
      default: 'Generated',
      required: true,
      index: true,
    },
    totalDelay: {
      type: Number,
      default: 0,
    },
    algorithmUsed: {
      type: String,
      required: true,
    },
    creatorUserId: {
      type: String,
      required: [true, 'Creator user ID is required'],
      index: true,
    },
    notes: {
      type: String,
      default: null,
    },
  },
  {
    timestamps: true,
    collection: 'operation_plans',
  }
);

// Indexes for common queries
OperationPlanSchema.index({ vvnIds: 1 });
OperationPlanSchema.index({ status: 1 });
OperationPlanSchema.index({ creatorUserId: 1 });
OperationPlanSchema.index({ createdAt: -1 });

export const OperationPlan = model<IOperationPlan>(
  'OperationPlan',
  OperationPlanSchema
);

