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

/**
 * @swagger
 * /api/vessel-visit-executions:
 *   get:
 *     summary: Get all VVEs with optional filters
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: vesselId
 *         schema:
 *           type: string
 *       - in: query
 *         name: status
 *         schema:
 *           type: string
 *           enum: [In Progress, Completed]
 *       - in: query
 *         name: startDate
 *         schema:
 *           type: string
 *           format: date-time
 *       - in: query
 *         name: endDate
 *         schema:
 *           type: string
 *           format: date-time
 *     responses:
 *       200:
 *         description: List of VVEs
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/VesselVisitExecution'
 *   post:
 *     summary: Create a new VVE
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - vvnId
 *               - vesselId
 *               - actualArrivalTime
 *             properties:
 *               vvnId:
 *                 type: string
 *                 description: VVN ID from ASP.NET backend
 *               vesselId:
 *                 type: string
 *                 description: Vessel ID from ASP.NET backend
 *               actualArrivalTime:
 *                 type: string
 *                 format: date-time
 *     responses:
 *       201:
 *         description: VVE created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/VesselVisitExecution'
 */
router.get('/', validateVesselIdQuery, validateStatusQuery, validateDateRangeQuery, getAllVVEs);
router.post('/', validateCreateVVE, createVVE);

/**
 * @swagger
 * /api/vessel-visit-executions/by-vvn:
 *   get:
 *     summary: Get VVE by VVN ID
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: vvnId
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: VVE details
 *       404:
 *         description: VVE not found
 */
router.get('/by-vvn', validateVVNIdQuery, getVVEByVVNId);

/**
 * @swagger
 * /api/vessel-visit-executions/{vveId}:
 *   get:
 *     summary: Get VVE by ID
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: VVE details
 *       404:
 *         description: VVE not found
 *   put:
 *     summary: Update VVE
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               actualBerthTime:
 *                 type: string
 *                 format: date-time
 *               actualDockId:
 *                 type: string
 *               actualUnberthTime:
 *                 type: string
 *                 format: date-time
 *               actualDepartureTime:
 *                 type: string
 *                 format: date-time
 *               status:
 *                 type: string
 *                 enum: [In Progress, Completed]
 *     responses:
 *       200:
 *         description: VVE updated
 *       404:
 *         description: VVE not found
 *   patch:
 *     summary: Partially update VVE
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               actualBerthTime:
 *                 type: string
 *                 format: date-time
 *               actualDockId:
 *                 type: string
 *               actualUnberthTime:
 *                 type: string
 *                 format: date-time
 *               actualDepartureTime:
 *                 type: string
 *                 format: date-time
 *               status:
 *                 type: string
 *                 enum: [In Progress, Completed]
 *     responses:
 *       200:
 *         description: VVE updated
 *   delete:
 *     summary: Delete VVE
 *     tags: [Vessel Visit Executions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       204:
 *         description: VVE deleted
 *       404:
 *         description: VVE not found
 */
router.get('/:vveId', getVVEById);
router.put('/:vveId', validateUpdateVVE, updateVVE);
router.patch('/:vveId', validateUpdateVVE, updateVVE);
router.delete('/:vveId', deleteVVE);

export default router;

