import { Schema, model, Document } from 'mongoose';

export interface IIncidentType extends Document {
  code: string;
  name: string;
  description: string;
  severity: 'Minor' | 'Major' | 'Critical';
  parentId?: Schema.Types.ObjectId | null;
  createdAt: Date;
  updatedAt: Date;
}

const IncidentTypeSchema = new Schema<IIncidentType>(
  {
    code: {
      type: String,
      required: [true, 'Code is required'],
      unique: true,
      trim: true,
      uppercase: true,
    },
    name: {
      type: String,
      required: [true, 'Name is required'],
      trim: true,
    },
    description: {
      type: String,
      required: [true, 'Description is required'],
      trim: true,
    },
    severity: {
      type: String,
      enum: {
        values: ['Minor', 'Major', 'Critical'],
        message: '{VALUE} is not a valid severity level',
      },
      required: [true, 'Severity is required'],
    },
    parentId: {
      type: Schema.Types.ObjectId,
      ref: 'IncidentType',
      default: null,
    },
  },
  {
    timestamps: true,
    versionKey: false,
  }
);

// Indexes for better query performance
IncidentTypeSchema.index({ code: 1 });
IncidentTypeSchema.index({ parentId: 1 });
IncidentTypeSchema.index({ name: 'text' }); // Text search on name

// Virtual for getting children
IncidentTypeSchema.virtual('children', {
  ref: 'IncidentType',
  localField: '_id',
  foreignField: 'parentId',
});

export const IncidentTypeModel = model<IIncidentType>('IncidentType', IncidentTypeSchema);

