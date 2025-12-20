import { Router } from 'express';
import incidentTypeRoutes from './incidentType.routes';
import incidentRoutes from './incident.routes';
import vesselVisitExecutionRoutes from './vesselVisitExecution.routes';

const router = Router();

// Register all routes
router.use('/incident-types', incidentTypeRoutes);
router.use('/incidents', incidentRoutes);
router.use('/vessel-visit-executions', vesselVisitExecutionRoutes);

export default router;

