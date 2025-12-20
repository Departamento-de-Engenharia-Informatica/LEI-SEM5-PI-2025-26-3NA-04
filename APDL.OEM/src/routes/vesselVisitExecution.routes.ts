import { Router } from 'express';
import { authenticate } from '../middleware/auth.middleware';
import {
  createVVE,
  getVVEById,
  getVVEByVVNId,
  getAllVVEs,
  updateVVE,
  deleteVVE,
} from '../controllers/vesselVisitExecution.controller';
import {
  validateCreateVVE,
  validateUpdateVVE,
  validateVVEIdQuery,
  validateVVNIdQuery,
  validateVesselIdQuery,
  validateStatusQuery,
  validateDateRangeQuery,
} from '../validators/vesselVisitExecution.validator';

const router = Router();

// All routes require authentication
router.use(authenticate);

// Create VVE
router.post('/', validateCreateVVE, createVVE);

// Get VVE by ID
router.get('/:vveId', getVVEById);

// Get VVE by VVN ID (query parameter)
router.get('/by-vvn', validateVVNIdQuery, getVVEByVVNId);

// Get all VVEs (with optional filters: vesselId, status, startDate, endDate)
router.get('/', validateVesselIdQuery, validateStatusQuery, validateDateRangeQuery, getAllVVEs);

// Update VVE
router.put('/:vveId', validateUpdateVVE, updateVVE);
router.patch('/:vveId', validateUpdateVVE, updateVVE);

// Delete VVE
router.delete('/:vveId', deleteVVE);

export default router;

