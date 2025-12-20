import { Response } from 'express';
import { AuthRequest } from '../middleware/auth.middleware';
import { IncidentService } from '../services/incident.service';
import { validationResult } from 'express-validator';

const incidentService = new IncidentService();

// Get all incidents
export const getAllIncidents = async (req: AuthRequest, res: Response) => {
  try {
    const incidents = await incidentService.getAll();
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get incident by ID
export const getIncidentById = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const incident = await incidentService.getById(id);

    if (!incident) {
      return res.status(404).json({ error: 'Incident not found' });
    }

    res.json(incident);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get active incidents
export const getActiveIncidents = async (req: AuthRequest, res: Response) => {
  try {
    const incidents = await incidentService.getActiveIncidents();
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get resolved incidents
export const getResolvedIncidents = async (req: AuthRequest, res: Response) => {
  try {
    const incidents = await incidentService.getResolvedIncidents();
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Filter by severity
export const filterIncidentsBySeverity = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { severity } = req.query;
    const incidents = await incidentService.filterBySeverity(severity as string);
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Filter by date range
export const filterIncidentsByDateRange = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { startDate, endDate } = req.query;
    
    if (!startDate || !endDate) {
      return res.status(400).json({ error: 'Both startDate and endDate are required' });
    }

    const incidents = await incidentService.filterByDateRange(
      new Date(startDate as string),
      new Date(endDate as string)
    );
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Filter by VVE ID
export const filterIncidentsByVVE = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { vveId } = req.query;
    const incidents = await incidentService.filterByVVEId(vveId as string);
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get incidents affecting a specific VVE
export const getIncidentsAffectingVVE = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { vveId } = req.query;
    const incidents = await incidentService.getIncidentsAffectingVVE(vveId as string);
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Create new incident
export const createIncident = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    // Set responsible user from token if not provided
    // Prefer email, fallback to sub (Auth0 user ID), then 'unknown'
    // Debug: uncomment to see what's in the token
    // console.log('req.user:', req.user);
    // console.log('req.auth:', req.auth);
    
    const responsibleUser = req.body.responsibleUser || 
                           req.user?.email || 
                           req.user?.sub || 
                           req.auth?.email ||
                           req.auth?.sub ||
                           'unknown';
    
    const incidentData = {
      ...req.body,
      responsibleUser,
    };

    const incident = await incidentService.create(incidentData);
    res.status(201).json(incident);
  } catch (error) {
    const statusCode = 
      error instanceof Error && error.message.includes('not found') ? 404 :
      error instanceof Error && error.message.includes('must affect') ? 400 : 500;
    
    res.status(statusCode).json({
      error: statusCode === 404 ? 'Not Found' : statusCode === 400 ? 'Bad Request' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Update incident
export const updateIncident = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const incident = await incidentService.update(id, req.body);

    if (!incident) {
      return res.status(404).json({ error: 'Incident not found' });
    }

    res.json(incident);
  } catch (error) {
    const statusCode = 
      error instanceof Error && error.message.includes('not found') ? 404 :
      error instanceof Error && error.message.includes('must be after') ? 400 :
      error instanceof Error && error.message.includes('must affect') ? 400 : 500;
    
    res.status(statusCode).json({
      error: statusCode === 404 ? 'Not Found' : statusCode === 400 ? 'Bad Request' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Resolve incident
export const resolveIncident = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const { endTime } = req.body;

    const incident = await incidentService.resolveIncident(
      id,
      endTime ? new Date(endTime) : undefined
    );

    if (!incident) {
      return res.status(404).json({ error: 'Incident not found' });
    }

    res.json(incident);
  } catch (error) {
    const statusCode = 
      error instanceof Error && error.message.includes('already resolved') ? 400 :
      error instanceof Error && error.message.includes('must be after') ? 400 : 500;
    
    res.status(statusCode).json({
      error: statusCode === 400 ? 'Bad Request' : 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Delete incident
export const deleteIncident = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { id } = req.params;
    const deleted = await incidentService.delete(id);

    if (!deleted) {
      return res.status(404).json({ error: 'Incident not found' });
    }

    res.status(204).send();
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Search incidents
export const searchIncidents = async (req: AuthRequest, res: Response) => {
  try {
    const { q } = req.query;
    if (!q || typeof q !== 'string') {
      return res.status(400).json({ error: 'Search query is required' });
    }

    const incidents = await incidentService.search(q);
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

// Get incidents by responsible user
export const getIncidentsByUser = async (req: AuthRequest, res: Response) => {
  try {
    const userEmail = req.user?.email || req.query.userEmail;
    if (!userEmail || typeof userEmail !== 'string') {
      return res.status(400).json({ error: 'User email is required' });
    }

    const incidents = await incidentService.getByResponsibleUser(userEmail);
    res.json(incidents);
  } catch (error) {
    res.status(500).json({
      error: 'Internal server error',
      message: error instanceof Error ? error.message : 'Unknown error',
    });
  }
};

