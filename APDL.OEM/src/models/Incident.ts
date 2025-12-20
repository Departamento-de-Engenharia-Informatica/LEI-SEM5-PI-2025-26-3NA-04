import { Schema, model, Document } from 'mongoose';

export interface IIncident extends Document {
  incidentTypeId: Schema.Types.ObjectId;
  startTime: Date;
  endTime: Date | null; // null = active incident
  duration: number | null; // computed in minutes when endTime is set
  severity: 'Minor' | 'Major' | 'Critical';
  description: string;
  responsibleUser: string; // email or user ID from Auth0 token
  affectedVVEIds: string[]; // Array of VVE IDs (strings for now)
  affectsAllOngoingVVEs: boolean; // If true, affects all ongoing VVEs
  affectsAllUpcomingVVEs: boolean; // If true, affects all upcoming VVEs
  status: 'active' | 'resolved'; // computed: active if endTime is null
  createdAt: Date;
  updatedAt: Date;
}

const IncidentSchema = new Schema<IIncident>(
  {
    incidentTypeId: {
      type: Schema.Types.ObjectId,
      ref: 'IncidentType',
      required: [true, 'Incident type is required'],
    },
    startTime: {
      type: Date,
      required: [true, 'Start time is required'],
    },
    endTime: {
      type: Date,
      default: null,
    },
    duration: {
      type: Number,
      default: null, // Will be computed when endTime is set
    },
    severity: {
      type: String,
      enum: {
        values: ['Minor', 'Major', 'Critical'],
        message: '{VALUE} is not a valid severity level',
      },
      required: [true, 'Severity is required'],
    },
    description: {
      type: String,
      required: [true, 'Description is required'],
      trim: true,
      maxlength: [2000, 'Description cannot exceed 2000 characters'],
    },
    responsibleUser: {
      type: String,
      required: [true, 'Responsible user is required'],
      trim: true,
    },
    affectedVVEIds: {
      type: [String],
      default: [],
    },
    affectsAllOngoingVVEs: {
      type: Boolean,
      default: false,
    },
    affectsAllUpcomingVVEs: {
      type: Boolean,
      default: false,
    },
    status: {
      type: String,
      enum: ['active', 'resolved'],
      default: 'active',
    },
  },
  {
    timestamps: true,
    versionKey: false,
  }
);

// Indexes for better query performance
IncidentSchema.index({ incidentTypeId: 1 });
IncidentSchema.index({ startTime: 1 });
IncidentSchema.index({ endTime: 1 });
IncidentSchema.index({ status: 1 });
IncidentSchema.index({ severity: 1 });
IncidentSchema.index({ responsibleUser: 1 });
IncidentSchema.index({ affectedVVEIds: 1 });

// Text search index for description
IncidentSchema.index({ description: 'text' });

// Virtual for status (computed from endTime)
IncidentSchema.virtual('computedStatus').get(function() {
  return this.endTime === null ? 'active' : 'resolved';
});

// Pre-save middleware to compute duration and status
IncidentSchema.pre('save', function(next) {
  // Compute status based on endTime
  if (this.endTime === null) {
    this.status = 'active';
    this.duration = null;
  } else {
    this.status = 'resolved';
    // Calculate duration in minutes
    if (this.startTime && this.endTime) {
      const diffMs = this.endTime.getTime() - this.startTime.getTime();
      this.duration = Math.round(diffMs / (1000 * 60)); // Convert to minutes
    }
  }
  next();
});

export const IncidentModel = model<IIncident>('Incident', IncidentSchema);

