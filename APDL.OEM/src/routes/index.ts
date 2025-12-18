import { Router } from 'express';
import incidentTypeRoutes from './incidentType.routes';

const router = Router();

// Register all routes
router.use('/incident-types', incidentTypeRoutes);

export default router;

