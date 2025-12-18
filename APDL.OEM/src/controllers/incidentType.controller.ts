import { Response } from 'express';
import { AuthRequest } from '../middleware/auth.middleware';
import { IncidentTypeService } from '../services/incidentType.service';
import { validationResult } from 'express-validator';

const incidentTypeService = new IncidentTypeService();

// Get all incident types
export const getAllIncidentTypes = async (req: AuthRequest, res: Response) => {
  try {
    const incidentTypes = await incidentTypeService.getAll();
    res.json(incidentTypes);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get incident type by ID
export const getIncidentTypeById = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const incidentType = await incidentTypeService.getById(id);

    if (!incidentType) {
      return res.status(404).json({ error: 'Incident type not found' });
    }

    res.json(incidentType);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get root incident types (no parent)
export const getRootIncidentTypes = async (req: AuthRequest, res: Response) => {
  try {
    const incidentTypes = await incidentTypeService.getRootTypes();
    res.json(incidentTypes);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get incident type hierarchy
export const getIncidentTypeHierarchy = async (req: AuthRequest, res: Response) => {
  try {
    const hierarchy = await incidentTypeService.getHierarchy();
    res.json(hierarchy);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get children of a specific incident type
export const getIncidentTypeChildren = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const children = await incidentTypeService.getChildren(id);
    res.json(children);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Create new incident type
export const createIncidentType = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const incidentType = await incidentTypeService.create(req.body);
    res.status(201).json(incidentType);
  } catch (error) {
    const statusCode = error instanceof Error && error.message.includes('already exists') ? 409 : 500;
    res.status(statusCode).json({
      error: statusCode === 409 ? 'Conflict' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Update incident type
export const updateIncidentType = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const incidentType = await incidentTypeService.update(id, req.body);

    if (!incidentType) {
      return res.status(404).json({ error: 'Incident type not found' });
    }

    res.json(incidentType);
  } catch (error) {
    const statusCode = 
      error instanceof Error && error.message.includes('circular') ? 400 :
      error instanceof Error && error.message.includes('already exists') ? 409 : 500;
    
    res.status(statusCode).json({
      error: statusCode === 400 ? 'Bad Request' : statusCode === 409 ? 'Conflict' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Delete incident type
export const deleteIncidentType = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const deleted = await incidentTypeService.delete(id);

    if (!deleted) {
      return res.status(404).json({ error: 'Incident type not found' });
    }

    res.status(204).send();
  } catch (error) {
    const statusCode = error instanceof Error && error.message.includes('has children') ? 400 : 500;
    res.status(statusCode).json({
      error: statusCode === 400 ? 'Bad Request' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Search incident types
export const searchIncidentTypes = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { q } = req.query;
    const incidentTypes = await incidentTypeService.search(q as string);
    res.json(incidentTypes);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Filter by severity
export const filterIncidentTypesBySeverity = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { severity } = req.query;
    const incidentTypes = await incidentTypeService.filterBySeverity(severity as string);
    res.json(incidentTypes);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

