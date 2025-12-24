import { Router } from 'express';
import operationPlanController from '../controllers/operationPlan.controller';
import { authenticate } from '../middleware/auth.middleware';
import { body, param } from 'express-validator';

const router = Router();

/**
 * @swagger
 * /operation-plans/generate:
 *   post:
 *     summary: Generate Operation Plan from VVNs using Prolog scheduling
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             required:
 *               - vvnIds
 *             properties:
 *               vvnIds:
 *                 type: array
 *                 items:
 *                   type: string
 *                 description: Array of Vessel Visit Notification IDs
 *               algorithm:
 *                 type: string
 *                 description: "Prolog algorithm to use (default: original)"
 *               notes:
 *                 type: string
 *                 description: Optional notes about the plan
 *     responses:
 *       201:
 *         description: Operation Plan generated successfully
 *       400:
 *         description: Validation error
 *       401:
 *         description: Unauthorized
 *       500:
 *         description: Internal server error
 */
router.post(
  '/generate',
  authenticate,
  [
    body('vvnIds')
      .isArray({ min: 1 })
      .withMessage('vvnIds must be a non-empty array'),
    body('vvnIds.*')
      .isString()
      .withMessage('Each vvnId must be a string'),
    body('algorithm').optional().isString(),
    body('notes').optional().isString(),
  ],
  operationPlanController.generateOperationPlan.bind(operationPlanController)
);

/**
 * @swagger
 * /operation-plans:
 *   get:
 *     summary: Get all Operation Plans
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of Operation Plans
 *       500:
 *         description: Internal server error
 */
router.get(
  '/',
  authenticate,
  operationPlanController.getAllOperationPlans.bind(operationPlanController)
);

/**
 * @swagger
 * /operation-plans/{id}:
 *   get:
 *     summary: Get Operation Plan by ID
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *         description: Operation Plan ID
 *     responses:
 *       200:
 *         description: Operation Plan details
 *       404:
 *         description: Operation Plan not found
 *       500:
 *         description: Internal server error
 */
router.get(
  '/:id',
  authenticate,
  [param('id').isString().withMessage('Plan ID must be a string')],
  operationPlanController.getOperationPlanById.bind(operationPlanController)
);

/**
 * @swagger
 * /operation-plans/{id}:
 *   put:
 *     summary: Update Operation Plan manually
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *         description: Operation Plan ID
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               schedule:
 *                 type: array
 *                 description: Updated schedule entries
 *               status:
 *                 type: string
 *                 enum: [Generated, Manual, Approved]
 *                 description: Updated status
 *               notes:
 *                 type: string
 *                 description: Updated notes
 *     responses:
 *       200:
 *         description: Operation Plan updated successfully
 *       400:
 *         description: "Validation error (e.g., dock conflicts)"
 *       404:
 *         description: Operation Plan not found
 *       500:
 *         description: Internal server error
 */
router.put(
  '/:id',
  authenticate,
  [
    param('id').isString().withMessage('Plan ID must be a string'),
    body('schedule').optional().isArray(),
    body('status')
      .optional()
      .isIn(['Generated', 'Manual', 'Approved'])
      .withMessage('Status must be one of: Generated, Manual, Approved'),
    body('notes').optional().isString(),
  ],
  operationPlanController.updateOperationPlan.bind(operationPlanController)
);

/**
 * @swagger
 * /operation-plans/{id}:
 *   delete:
 *     summary: Delete Operation Plan
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *         description: Operation Plan ID
 *     responses:
 *       204:
 *         description: Operation Plan deleted successfully
 *       404:
 *         description: Operation Plan not found
 *       500:
 *         description: Internal server error
 */
router.delete(
  '/:id',
  authenticate,
  [param('id').isString().withMessage('Plan ID must be a string')],
  operationPlanController.deleteOperationPlan.bind(operationPlanController)
);

export default router;

