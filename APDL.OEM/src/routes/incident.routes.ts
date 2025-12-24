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

/**
 * @swagger
 * /api/incidents:
 *   get:
 *     summary: Get all incidents
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of all incidents
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/Incident'
 *   post:
 *     summary: Create a new incident
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - incidentTypeId
 *               - startTime
 *               - severity
 *               - description
 *             properties:
 *               incidentTypeId:
 *                 type: string
 *               startTime:
 *                 type: string
 *                 format: date-time
 *               endTime:
 *                 type: string
 *                 format: date-time
 *                 nullable: true
 *               severity:
 *                 type: string
 *                 enum: [Minor, Major, Critical]
 *               description:
 *                 type: string
 *               affectedVVEIds:
 *                 type: array
 *                 items:
 *                   type: string
 *               affectsAllOngoingVVEs:
 *                 type: boolean
 *               affectsAllUpcomingVVEs:
 *                 type: boolean
 *     responses:
 *       201:
 *         description: Incident created successfully
 */
router.get('/', getAllIncidents);
router.post('/', validateCreateIncident, createIncident);

/**
 * @swagger
 * /api/incidents/active:
 *   get:
 *     summary: Get all active incidents
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of active incidents
 */
router.get('/active', getActiveIncidents);

/**
 * @swagger
 * /api/incidents/resolved:
 *   get:
 *     summary: Get all resolved incidents
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of resolved incidents
 */
router.get('/resolved', getResolvedIncidents);

/**
 * @swagger
 * /api/incidents/{id}:
 *   get:
 *     summary: Get incident by ID
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Incident details
 *       404:
 *         description: Incident not found
 *   put:
 *     summary: Update incident
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
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
 *               incidentTypeId:
 *                 type: string
 *               startTime:
 *                 type: string
 *                 format: date-time
 *               endTime:
 *                 type: string
 *                 format: date-time
 *               severity:
 *                 type: string
 *                 enum: [Minor, Major, Critical]
 *               description:
 *                 type: string
 *               affectedVVEIds:
 *                 type: array
 *                 items:
 *                   type: string
 *               affectsAllOngoingVVEs:
 *                 type: boolean
 *               affectsAllUpcomingVVEs:
 *                 type: boolean
 *     responses:
 *       200:
 *         description: Incident updated
 *       404:
 *         description: Incident not found
 *   delete:
 *     summary: Delete incident
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       204:
 *         description: Incident deleted
 *       404:
 *         description: Incident not found
 */
router.get('/:id', validateIdParam, getIncidentById);
router.put('/:id', validateUpdateIncident, updateIncident);
router.delete('/:id', validateIdParam, deleteIncident);

/**
 * @swagger
 * /api/incidents/{id}/resolve:
 *   put:
 *     summary: Resolve an incident
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - endTime
 *             properties:
 *               endTime:
 *                 type: string
 *                 format: date-time
 *     responses:
 *       200:
 *         description: Incident resolved
 */
router.put('/:id/resolve', validateIdParam, resolveIncident);

/**
 * @swagger
 * /api/incidents/search:
 *   get:
 *     summary: Search incidents
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: q
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Search results
 */
router.get('/search', searchIncidents);

/**
 * @swagger
 * /api/incidents/by-user:
 *   get:
 *     summary: Get incidents by responsible user
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of incidents
 */
router.get('/by-user', getIncidentsByUser);

/**
 * @swagger
 * /api/incidents/filter/severity:
 *   get:
 *     summary: Filter incidents by severity
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: severity
 *         required: true
 *         schema:
 *           type: string
 *           enum: [Minor, Major, Critical]
 *     responses:
 *       200:
 *         description: Filtered incidents
 */
router.get('/filter/severity', validateSeverityQuery, filterIncidentsBySeverity);

/**
 * @swagger
 * /api/incidents/filter/date-range:
 *   get:
 *     summary: Filter incidents by date range
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: startDate
 *         required: true
 *         schema:
 *           type: string
 *           format: date-time
 *       - in: query
 *         name: endDate
 *         required: true
 *         schema:
 *           type: string
 *           format: date-time
 *     responses:
 *       200:
 *         description: Filtered incidents
 */
router.get('/filter/date-range', validateDateRangeQuery, filterIncidentsByDateRange);

/**
 * @swagger
 * /api/incidents/filter/vve:
 *   get:
 *     summary: Filter incidents by VVE ID
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Filtered incidents
 */
router.get('/filter/vve', validateVVEIdQuery, filterIncidentsByVVE);

/**
 * @swagger
 * /api/incidents/affecting-vve:
 *   get:
 *     summary: Get incidents affecting a specific VVE
 *     tags: [Incidents]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: vveId
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: List of incidents affecting the VVE
 */
router.get('/affecting-vve', validateVVEIdQuery, getIncidentsAffectingVVE);

export default router;


