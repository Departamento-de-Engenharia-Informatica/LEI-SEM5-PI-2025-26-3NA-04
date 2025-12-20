import { Router } from 'express';
import { authenticate } from '../middleware/auth.middleware';
import {
  getAllIncidents,
  getIncidentById,
  getActiveIncidents,
  getResolvedIncidents,
  filterIncidentsBySeverity,
  filterIncidentsByDateRange,
  filterIncidentsByVVE,
  getIncidentsAffectingVVE,
  createIncident,
  updateIncident,
  resolveIncident,
  deleteIncident,
  searchIncidents,
  getIncidentsByUser,
} from '../controllers/incident.controller';
import {
  validateCreateIncident,
  validateUpdateIncident,
  validateIdParam,
  validateSeverityQuery,
  validateDateRangeQuery,
  validateVVEIdQuery,
} from '../validators/incident.validator';

const router = Router();

// All routes require authentication
router.use(authenticate);

// Get routes
router.get('/', getAllIncidents);
router.get('/active', getActiveIncidents);
router.get('/resolved', getResolvedIncidents);
router.get('/search', searchIncidents);
router.get('/by-user', getIncidentsByUser);
router.get('/filter/severity', validateSeverityQuery, filterIncidentsBySeverity);
router.get('/filter/date-range', validateDateRangeQuery, filterIncidentsByDateRange);
router.get('/filter/vve', validateVVEIdQuery, filterIncidentsByVVE);
router.get('/affecting-vve', validateVVEIdQuery, getIncidentsAffectingVVE);
router.get('/:id', validateIdParam, getIncidentById);

// Post routes
router.post('/', validateCreateIncident, createIncident);

// Put routes
router.put('/:id', validateUpdateIncident, updateIncident);
router.put('/:id/resolve', validateIdParam, resolveIncident);

// Delete routes
router.delete('/:id', validateIdParam, deleteIncident);

export default router;


