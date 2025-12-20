import { Response } from 'express';
import { validationResult } from 'express-validator';
import { vesselVisitExecutionService, CreateVVEDto, UpdateVVEDto } from '../services/vesselVisitExecution.service';
import { AuthRequest } from '../middleware/auth.middleware';

// Create a new VVE
export const createVVE = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    // Extract creator user ID from Auth0 token
    const creatorUserId = req.user?.sub || req.auth?.sub || req.user?.email || req.auth?.email || 'unknown';

    const vveData: CreateVVEDto = {
      vvnId: req.body.vvnId,
      vesselId: req.body.vesselId,
      actualArrivalTime: req.body.actualArrivalTime,
      creatorUserId,
    };

    const vve = await vesselVisitExecutionService.create(vveData);
    res.status(201).json(vve);
  } catch (error: any) {
    res.status(400).json({
      error: 'Failed to create VVE',
      message: error.message,
    });
  }
};

// Get VVE by ID
export const getVVEById = async (req: AuthRequest, res: Response) => {
  try {
    const { vveId } = req.params;
    const vve = await vesselVisitExecutionService.getById(vveId);

    if (!vve) {
      return res.status(404).json({
        error: 'VVE not found',
        message: `No VVE found with ID: ${vveId}`,
      });
    }

    res.json(vve);
  } catch (error: any) {
    res.status(500).json({
      error: 'Failed to retrieve VVE',
      message: error.message,
    });
  }
};

// Get VVE by VVN ID
export const getVVEByVVNId = async (req: AuthRequest, res: Response) => {
  try {
    const { vvnId } = req.query;
    if (!vvnId) {
      return res.status(400).json({
        error: 'VVN ID is required',
        message: 'Please provide a VVN ID in the query parameters',
      });
    }

    const vve = await vesselVisitExecutionService.getByVVNId(vvnId as string);
    if (!vve) {
      return res.status(404).json({
        error: 'VVE not found',
        message: `No VVE found for VVN ID: ${vvnId}`,
      });
    }

    res.json(vve);
  } catch (error: any) {
    res.status(500).json({
      error: 'Failed to retrieve VVE',
      message: error.message,
    });
  }
};

// Get all VVEs
export const getAllVVEs = async (req: AuthRequest, res: Response) => {
  try {
    const { vesselId, status, startDate, endDate } = req.query;

    let vves;

    if (vesselId) {
      vves = await vesselVisitExecutionService.getByVesselId(vesselId as string);
    } else if (status) {
      vves = await vesselVisitExecutionService.getByStatus(status as 'In Progress' | 'Completed');
    } else if (startDate && endDate) {
      vves = await vesselVisitExecutionService.getByDateRange(
        new Date(startDate as string),
        new Date(endDate as string)
      );
    } else {
      vves = await vesselVisitExecutionService.getAll();
    }

    res.json(vves);
  } catch (error: any) {
    res.status(500).json({
      error: 'Failed to retrieve VVEs',
      message: error.message,
    });
  }
};

// Update VVE
export const updateVVE = async (req: AuthRequest, res: Response) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ errors: errors.array() });
    }

    const { vveId } = req.params;
    const updateData: UpdateVVEDto = {
      actualBerthTime: req.body.actualBerthTime,
      actualDockId: req.body.actualDockId,
      actualUnberthTime: req.body.actualUnberthTime,
      actualDepartureTime: req.body.actualDepartureTime,
      status: req.body.status,
    };

    // Remove undefined fields
    Object.keys(updateData).forEach((key) => {
      if (updateData[key as keyof UpdateVVEDto] === undefined) {
        delete updateData[key as keyof UpdateVVEDto];
      }
    });

    const vve = await vesselVisitExecutionService.update(vveId, updateData);

    if (!vve) {
      return res.status(404).json({
        error: 'VVE not found',
        message: `No VVE found with ID: ${vveId}`,
      });
    }

    res.json(vve);
  } catch (error: any) {
    res.status(400).json({
      error: 'Failed to update VVE',
      message: error.message,
    });
  }
};

// Delete VVE
export const deleteVVE = async (req: AuthRequest, res: Response) => {
  try {
    const { vveId } = req.params;
    const deleted = await vesselVisitExecutionService.delete(vveId);

    if (!deleted) {
      return res.status(404).json({
        error: 'VVE not found',
        message: `No VVE found with ID: ${vveId}`,
      });
    }

    res.status(204).send();
  } catch (error: any) {
    res.status(500).json({
      error: 'Failed to delete VVE',
      message: error.message,
    });
  }
};

