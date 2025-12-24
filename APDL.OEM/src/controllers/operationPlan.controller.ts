import { Request, Response } from 'express';
import { validationResult } from 'express-validator';
import operationPlanService from '../services/operationPlan.service';
import { AuthRequest } from '../middleware/auth.middleware';

export class OperationPlanController {
  async generateOperationPlan(req: AuthRequest, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({ errors: errors.array() });
        return;
      }

      const { vvnIds, algorithm, notes } = req.body;

      const authToken = req.headers.authorization?.replace('Bearer ', '');
      if (!authToken) {
        res.status(401).json({
          error: 'Unauthorized',
          message: 'Authentication token is required',
        });
        return;
      }

      const creatorUserId = req.user?.sub || req.auth?.sub || 'unknown';

      const plan = await operationPlanService.generateOperationPlan(
        { vvnIds, algorithm, notes },
        authToken,
        creatorUserId
      );

      res.status(201).json(plan);
    } catch (error: any) {
      res.status(500).json({
        error: 'Internal server error',
        message: error.message,
      });
    }
  }

  async getAllOperationPlans(req: Request, res: Response): Promise<void> {
    try {
      const plans = await operationPlanService.getAllOperationPlans();
      res.json(plans);
    } catch (error: any) {
      res.status(500).json({
        error: 'Internal server error',
        message: error.message,
      });
    }
  }

  async getOperationPlanById(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({ errors: errors.array() });
        return;
      }

      const { id } = req.params;
      const plan = await operationPlanService.getOperationPlanById(id);

      if (!plan) {
        res.status(404).json({
          error: 'Not found',
          message: 'Operation plan not found',
        });
        return;
      }

      res.json(plan);
    } catch (error: any) {
      res.status(500).json({
        error: 'Internal server error',
        message: error.message,
      });
    }
  }

  async updateOperationPlan(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({ errors: errors.array() });
        return;
      }

      const { id } = req.params;
      const { schedule, status, notes } = req.body;

      const plan = await operationPlanService.updateOperationPlan(id, {
        schedule,
        status,
        notes,
      });

      if (!plan) {
        res.status(404).json({
          error: 'Not found',
          message: 'Operation plan not found',
        });
        return;
      }

      res.json(plan);
    } catch (error: any) {
      if (error.message.includes('conflict')) {
        res.status(400).json({
          error: 'Validation error',
          message: error.message,
        });
        return;
      }

      res.status(500).json({
        error: 'Internal server error',
        message: error.message,
      });
    }
  }

  async deleteOperationPlan(req: Request, res: Response): Promise<void> {
    try {
      const errors = validationResult(req);
      if (!errors.isEmpty()) {
        res.status(400).json({ errors: errors.array() });
        return;
      }

      const { id } = req.params;
      const deleted = await operationPlanService.deleteOperationPlan(id);

      if (!deleted) {
        res.status(404).json({
          error: 'Not found',
          message: 'Operation plan not found',
        });
        return;
      }

      res.status(204).send();
    } catch (error: any) {
      res.status(500).json({
        error: 'Internal server error',
        message: error.message,
      });
    }
  }
}

export default new OperationPlanController();

