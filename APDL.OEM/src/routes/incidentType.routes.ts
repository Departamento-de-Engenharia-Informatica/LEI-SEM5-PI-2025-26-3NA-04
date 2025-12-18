import { Router } from 'express';
import { authenticate } from '../middleware/auth.middleware';
import {
  getAllIncidentTypes,
  getIncidentTypeById,
  getRootIncidentTypes,
  getIncidentTypeHierarchy,
  getIncidentTypeChildren,
  createIncidentType,
  updateIncidentType,
  deleteIncidentType,
  searchIncidentTypes,
  filterIncidentTypesBySeverity,
} from '../controllers/incidentType.controller';
import {
  validateCreateIncidentType,
  validateUpdateIncidentType,
  validateIdParam,
  validateSearchQuery,
  validateSeverityQuery,
} from '../validators/incidentType.validator';

const router = Router();

// All routes require authentication
router.use(authenticate);

// Get routes
router.get('/', getAllIncidentTypes);
router.get('/root', getRootIncidentTypes);
router.get('/hierarchy', getIncidentTypeHierarchy);
router.get('/search', validateSearchQuery, searchIncidentTypes);
router.get('/filter', validateSeverityQuery, filterIncidentTypesBySeverity);
router.get('/:id', validateIdParam, getIncidentTypeById);
router.get('/:id/children', validateIdParam, getIncidentTypeChildren);

// Post routes
router.post('/', validateCreateIncidentType, createIncidentType);

// Put routes
router.put('/:id', validateUpdateIncidentType, updateIncidentType);

// Delete routes
router.delete('/:id', validateIdParam, deleteIncidentType);

export default router;

