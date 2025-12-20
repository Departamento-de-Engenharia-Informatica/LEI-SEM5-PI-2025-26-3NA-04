import { body, query, ValidationChain } from 'express-validator';
import { Request } from 'express';

// Validation for creating a VVE
export const validateCreateVVE: ValidationChain[] = [
  body('vvnId')
    .notEmpty()
    .withMessage('VVN ID is required')
    .isString()
    .withMessage('VVN ID must be a string')
    .trim(),
  body('vesselId')
    .notEmpty()
    .withMessage('Vessel ID is required')
    .isString()
    .withMessage('Vessel ID must be a string')
    .trim(),
  body('actualArrivalTime')
    .notEmpty()
    .withMessage('Actual arrival time is required')
    .isISO8601()
    .withMessage('Actual arrival time must be a valid ISO 8601 date')
    .toDate(),
];

// Validation for updating a VVE
export const validateUpdateVVE: ValidationChain[] = [
  body('actualBerthTime')
    .optional()
    .isISO8601()
    .withMessage('Actual berth time must be a valid ISO 8601 date')
    .toDate(),
  body('actualDockId')
    .optional()
    .isString()
    .withMessage('Actual dock ID must be a string')
    .trim(),
  body('actualUnberthTime')
    .optional()
    .isISO8601()
    .withMessage('Actual unberth time must be a valid ISO 8601 date')
    .toDate(),
  body('actualDepartureTime')
    .optional()
    .isISO8601()
    .withMessage('Actual departure time must be a valid ISO 8601 date')
    .toDate(),
  body('status')
    .optional()
    .isIn(['In Progress', 'Completed'])
    .withMessage('Status must be either "In Progress" or "Completed"'),
];

// Validation for query parameters
export const validateVVEIdQuery = [
  query('vveId')
    .optional()
    .isString()
    .withMessage('VVE ID must be a string')
    .trim()
    .notEmpty()
    .withMessage('VVE ID must be a non-empty string'),
];

export const validateVVNIdQuery = [
  query('vvnId')
    .optional()
    .isString()
    .withMessage('VVN ID must be a string')
    .trim()
    .notEmpty()
    .withMessage('VVN ID must be a non-empty string'),
];

export const validateVesselIdQuery = [
  query('vesselId')
    .optional()
    .isString()
    .withMessage('Vessel ID must be a string')
    .trim()
    .notEmpty()
    .withMessage('Vessel ID must be a non-empty string'),
];

export const validateStatusQuery = [
  query('status')
    .optional()
    .isIn(['In Progress', 'Completed'])
    .withMessage('Status must be either "In Progress" or "Completed"'),
];

export const validateDateRangeQuery = [
  query('startDate')
    .optional()
    .isISO8601()
    .withMessage('Start date must be a valid ISO 8601 date')
    .toDate(),
  query('endDate')
    .optional()
    .isISO8601()
    .withMessage('End date must be a valid ISO 8601 date')
    .toDate()
    .custom((value, { req }) => {
      if (value && req.query?.startDate && new Date(value) < new Date(req.query.startDate as string)) {
        throw new Error('End date must be after start date');
      }
      return true;
    }),
];

