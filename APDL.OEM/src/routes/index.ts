import { Router } from 'express';
import incidentTypeRoutes from './incidentType.routes';
import incidentRoutes from './incident.routes';

const router = Router();

// Register all routes
router.use('/incident-types', incidentTypeRoutes);
router.use('/incidents', incidentRoutes);

export default router;

