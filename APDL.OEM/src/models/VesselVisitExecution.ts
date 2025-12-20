import { Schema, model, Document } from 'mongoose';

export interface IVesselVisitExecution extends Document {
  vveId: string; // Auto-generated VVE identifier (GUID pattern like VVN IDs)
  vvnId: string; // Reference to Vessel Visit Notification ID from ASP.NET backend
  vesselId: string; // Vessel identifier from ASP.NET backend
  actualArrivalTime: Date; // Actual arrival time at the port
  creatorUserId: string; // User ID from Auth0 token
  status: 'In Progress' | 'Completed'; // Status: "In Progress" when created
  actualBerthTime?: Date; // When vessel actually berthed (from 4.1.8)
  actualDockId?: string; // Actual dock used (from 4.1.8)
  actualUnberthTime?: Date; // When vessel left the dock (from 4.1.11)
  actualDepartureTime?: Date; // When vessel exited port limits (from 4.1.11)
  createdAt: Date;
  updatedAt: Date;
}

const VesselVisitExecutionSchema = new Schema<IVesselVisitExecution>(
  {
    vveId: {
      type: String,
      required: [true, 'VVE ID is required'],
      unique: true,
      index: true,
    },
    vvnId: {
      type: String,
      required: [true, 'VVN ID is required'],
      index: true,
    },
    vesselId: {
      type: String,
      required: [true, 'Vessel ID is required'],
      index: true,
    },
    actualArrivalTime: {
      type: Date,
      required: [true, 'Actual arrival time is required'],
      index: true,
    },
    creatorUserId: {
      type: String,
      required: [true, 'Creator user ID is required'],
      index: true,
    },
    status: {
      type: String,
      enum: ['In Progress', 'Completed'],
      default: 'In Progress',
      required: true,
    },
    actualBerthTime: {
      type: Date,
      default: null,
    },
    actualDockId: {
      type: String,
      default: null,
    },
    actualUnberthTime: {
      type: Date,
      default: null,
    },
    actualDepartureTime: {
      type: Date,
      default: null,
    },
  },
  {
    timestamps: true,
    collection: 'vessel_visit_executions',
  }
);

// Indexes for common queries
VesselVisitExecutionSchema.index({ vvnId: 1 });
VesselVisitExecutionSchema.index({ vesselId: 1 });
VesselVisitExecutionSchema.index({ status: 1 });
VesselVisitExecutionSchema.index({ actualArrivalTime: 1 });
VesselVisitExecutionSchema.index({ creatorUserId: 1 });

export const VesselVisitExecution = model<IVesselVisitExecution>(
  'VesselVisitExecution',
  VesselVisitExecutionSchema
);

