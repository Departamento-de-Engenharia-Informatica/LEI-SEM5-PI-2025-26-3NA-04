import { Document, Schema, Model, model } from 'mongoose';

// Base schema options
export const baseSchemaOptions = {
  timestamps: true, // Adds createdAt and updatedAt
  versionKey: false, // Removes __v field
};

// Base model interface
export interface BaseDocument extends Document {
  createdAt: Date;
  updatedAt: Date;
}

// Helper function to create models with base options
export function createModel<T extends BaseDocument>(
  name: string,
  schema: Schema<T>
): Model<T> {
  schema.set('timestamps', true);
  return model<T>(name, schema);
}

