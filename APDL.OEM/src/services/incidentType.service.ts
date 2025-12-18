import { IncidentTypeModel, IIncidentType } from '../models/IncidentType';

export class IncidentTypeService {
  // Get all incident types
  async getAll(): Promise<IIncidentType[]> {
    return await IncidentTypeModel.find().populate('parentId', 'code name').sort({ code: 1 });
  }

  // Get incident type by ID
  async getById(id: string): Promise<IIncidentType | null> {
    return await IncidentTypeModel.findById(id).populate('parentId', 'code name');
  }

  // Get root incident types (no parent)
  async getRootTypes(): Promise<IIncidentType[]> {
    return await IncidentTypeModel.find({ parentId: null }).sort({ code: 1 });
  }

  // Get children of a specific incident type
  async getChildren(parentId: string): Promise<IIncidentType[]> {
    return await IncidentTypeModel.find({ parentId }).sort({ code: 1 });
  }

  // Get full hierarchy
  async getHierarchy(): Promise<any[]> {
    const rootTypes = await this.getRootTypes();
    
    const buildHierarchy = async (parent: IIncidentType): Promise<any> => {
      const children = await this.getChildren(parent._id.toString());
      return {
        ...parent.toObject(),
        children: await Promise.all(children.map(child => buildHierarchy(child))),
      };
    };

    return await Promise.all(rootTypes.map(root => buildHierarchy(root)));
  }

  // Create new incident type
  async create(data: Partial<IIncidentType>): Promise<IIncidentType> {
    // Validate parent exists if parentId is provided
    if (data.parentId) {
      const parent = await IncidentTypeModel.findById(data.parentId);
      if (!parent) {
        throw new Error('Parent incident type not found');
      }
    }

    // Check if code already exists
    if (data.code) {
      const existing = await IncidentTypeModel.findOne({ code: data.code });
      if (existing) {
        throw new Error(`Incident type with code ${data.code} already exists`);
      }
    }

    const incidentType = new IncidentTypeModel(data);
    return await incidentType.save();
  }

  // Update incident type
  async update(id: string, data: Partial<IIncidentType>): Promise<IIncidentType | null> {
    // Validate parent exists if parentId is being updated
    if (data.parentId) {
      // Prevent self-reference
      if (data.parentId.toString() === id) {
        throw new Error('An incident type cannot be its own parent');
      }

      const parent = await IncidentTypeModel.findById(data.parentId);
      if (!parent) {
        throw new Error('Parent incident type not found');
      }

      // Prevent circular references (parent cannot be a descendant)
      const isDescendant = await this.isDescendantOf(data.parentId.toString(), id);
      if (isDescendant) {
        throw new Error('Cannot set parent to a descendant (circular reference)');
      }
    }

    // Check if code is being changed and if it already exists
    if (data.code) {
      const existing = await IncidentTypeModel.findOne({ 
        code: data.code,
        _id: { $ne: id }
      });
      if (existing) {
        throw new Error(`Incident type with code ${data.code} already exists`);
      }
    }

    return await IncidentTypeModel.findByIdAndUpdate(id, data, { 
      new: true,
      runValidators: true
    }).populate('parentId', 'code name');
  }

  // Delete incident type
  async delete(id: string): Promise<boolean> {
    // Check if this incident type has children
    const children = await IncidentTypeModel.find({ parentId: id });
    if (children.length > 0) {
      throw new Error('Cannot delete incident type that has children. Delete children first.');
    }

    // TODO: Check if this incident type is being used by any incidents
    // This would require checking the Incident collection once it's implemented

    const result = await IncidentTypeModel.findByIdAndDelete(id);
    return !!result;
  }

  // Helper: Check if targetId is a descendant of ancestorId
  private async isDescendantOf(targetId: string, ancestorId: string): Promise<boolean> {
    const target = await IncidentTypeModel.findById(targetId);
    if (!target || !target.parentId) {
      return false;
    }

    if (target.parentId.toString() === ancestorId) {
      return true;
    }

    return await this.isDescendantOf(target.parentId.toString(), ancestorId);
  }

  // Search incident types by name
  async search(query: string): Promise<IIncidentType[]> {
    return await IncidentTypeModel.find({
      $text: { $search: query }
    }).populate('parentId', 'code name');
  }

  // Filter by severity
  async filterBySeverity(severity: string): Promise<IIncidentType[]> {
    return await IncidentTypeModel.find({ severity }).populate('parentId', 'code name').sort({ code: 1 });
  }
}

