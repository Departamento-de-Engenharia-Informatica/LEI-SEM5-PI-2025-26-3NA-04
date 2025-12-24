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

/**
 * @swagger
 * /api/incident-types:
 *   get:
 *     summary: Get all incident types
 *     tags: [Incident Types]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of all incident types
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/IncidentType'
 *       401:
 *         $ref: '#/components/responses/UnauthorizedError'
 *   post:
 *     summary: Create a new incident type
 *     tags: [Incident Types]
 *     security:
 *       - bearerAuth: []
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - code
 *               - name
 *               - severity
 *             properties:
 *               code:
 *                 type: string
 *                 example: T-INC001
 *               name:
 *                 type: string
 *                 example: Equipment Failure
 *               description:
 *                 type: string
 *               severity:
 *                 type: string
 *                 enum: [Minor, Major, Critical]
 *               parentId:
 *                 type: string
 *                 nullable: true
 *     responses:
 *       201:
 *         description: Incident type created successfully
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/IncidentType'
 *       400:
 *         $ref: '#/components/responses/ValidationError'
 */
router.get('/', getAllIncidentTypes);
router.post('/', validateCreateIncidentType, createIncidentType);

/**
 * @swagger
 * /api/incident-types/root:
 *   get:
 *     summary: Get root incident types (no parent)
 *     tags: [Incident Types]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of root incident types
 */
router.get('/root', getRootIncidentTypes);

/**
 * @swagger
 * /api/incident-types/hierarchy:
 *   get:
 *     summary: Get complete incident type hierarchy
 *     tags: [Incident Types]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: Hierarchical structure of incident types
 */
router.get('/hierarchy', getIncidentTypeHierarchy);

/**
 * @swagger
 * /api/incident-types/search:
 *   get:
 *     summary: Search incident types by name or code
 *     tags: [Incident Types]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: q
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Search results
 */
router.get('/search', validateSearchQuery, searchIncidentTypes);

/**
 * @swagger
 * /api/incident-types/filter:
 *   get:
 *     summary: Filter incident types by severity
 *     tags: [Incident Types]
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
 *         description: Filtered incident types
 */
router.get('/filter', validateSeverityQuery, filterIncidentTypesBySeverity);

/**
 * @swagger
 * /api/incident-types/{id}:
 *   get:
 *     summary: Get incident type by ID
 *     tags: [Incident Types]
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
 *         description: Incident type details
 *       404:
 *         description: Incident type not found
 *   put:
 *     summary: Update incident type
 *     tags: [Incident Types]
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
 *               name:
 *                 type: string
 *               description:
 *                 type: string
 *               severity:
 *                 type: string
 *                 enum: [Minor, Major, Critical]
 *               parentId:
 *                 type: string
 *                 nullable: true
 *     responses:
 *       200:
 *         description: Incident type updated
 *       404:
 *         description: Incident type not found
 *   delete:
 *     summary: Delete incident type
 *     tags: [Incident Types]
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
 *         description: Incident type deleted
 *       404:
 *         description: Incident type not found
 */
router.get('/:id', validateIdParam, getIncidentTypeById);
router.put('/:id', validateUpdateIncidentType, updateIncidentType);
router.delete('/:id', validateIdParam, deleteIncidentType);

/**
 * @swagger
 * /api/incident-types/{id}/children:
 *   get:
 *     summary: Get children of an incident type
 *     tags: [Incident Types]
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
 *         description: List of child incident types
 */
router.get('/:id/children', validateIdParam, getIncidentTypeChildren);

export default router;

