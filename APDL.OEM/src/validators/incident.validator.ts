import { body, param, query } from 'express-validator';

export const validateCreateIncident = [
  body('incidentTypeId')
    .notEmpty()
    .withMessage('Incident type ID is required')
    .isMongoId()
    .withMessage('Incident type ID must be a valid MongoDB ObjectId'),
  
  body('startTime')
    .notEmpty()
    .withMessage('Start time is required')
    .isISO8601()
    .withMessage('Start time must be a valid ISO 8601 date'),
  
  body('endTime')
    .optional()
    .isISO8601()
    .withMessage('End time must be a valid ISO 8601 date')
    .custom((value, { req }) => {
      if (value && req.body.startTime && new Date(value) <= new Date(req.body.startTime)) {
        throw new Error('End time must be after start time');
      }
      return true;
    }),
  
  body('severity')
    .notEmpty()
    .withMessage('Severity is required')
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
  
  body('description')
    .notEmpty()
    .withMessage('Description is required')
    .isString()
    .withMessage('Description must be a string')
    .trim()
    .isLength({ min: 10, max: 2000 })
    .withMessage('Description must be between 10 and 2000 characters'),
  
  body('responsibleUser')
    .optional()
    .isString()
    .withMessage('Responsible user must be a string')
    .trim(),
  
  body('affectedVVEIds')
    .optional()
    .isArray()
    .withMessage('Affected VVE IDs must be an array')
    .custom((value) => {
      if (value && !Array.isArray(value)) {
        throw new Error('Affected VVE IDs must be an array');
      }
      if (value && value.some((id: any) => typeof id !== 'string')) {
        throw new Error('All VVE IDs must be strings');
      }
      return true;
    }),
  
  body('affectsAllOngoingVVEs')
    .optional()
    .isBoolean()
    .withMessage('Affects all ongoing VVEs must be a boolean'),
  
  body('affectsAllUpcomingVVEs')
    .optional()
    .isBoolean()
    .withMessage('Affects all upcoming VVEs must be a boolean'),
  
  body()
    .custom((value) => {
      const hasSpecificVVEs = value.affectedVVEIds && value.affectedVVEIds.length > 0;
      const affectsAll = value.affectsAllOngoingVVEs || value.affectsAllUpcomingVVEs;
      
      if (!hasSpecificVVEs && !affectsAll) {
        throw new Error('Incident must affect at least one VVE or all ongoing/upcoming VVEs');
      }
      return true;
    }),
];

export const validateUpdateIncident = [
  param('id')
    .isMongoId()
    .withMessage('Invalid incident ID'),
  
  body('incidentTypeId')
    .optional()
    .isMongoId()
    .withMessage('Incident type ID must be a valid MongoDB ObjectId'),
  
  body('startTime')
    .optional()
    .isISO8601()
    .withMessage('Start time must be a valid ISO 8601 date'),
  
  body('endTime')
    .optional()
    .custom((value, { req }) => {
      if (value === null) {
        return true; // Allow null to reactivate
      }
      if (typeof value === 'string') {
        const endDate = new Date(value);
        const startDate = req.body.startTime ? new Date(req.body.startTime) : null;
        if (startDate && endDate <= startDate) {
          throw new Error('End time must be after start time');
        }
      }
      return true;
    }),
  
  body('severity')
    .optional()
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
  
  body('description')
    .optional()
    .isString()
    .withMessage('Description must be a string')
    .trim()
    .isLength({ min: 10, max: 2000 })
    .withMessage('Description must be between 10 and 2000 characters'),
  
  body('affectedVVEIds')
    .optional()
    .isArray()
    .withMessage('Affected VVE IDs must be an array')
    .custom((value) => {
      if (value && value.some((id: any) => typeof id !== 'string')) {
        throw new Error('All VVE IDs must be strings');
      }
      return true;
    }),
  
  body('affectsAllOngoingVVEs')
    .optional()
    .isBoolean()
    .withMessage('Affects all ongoing VVEs must be a boolean'),
  
  body('affectsAllUpcomingVVEs')
    .optional()
    .isBoolean()
    .withMessage('Affects all upcoming VVEs must be a boolean'),
];

export const validateIdParam = [
  param('id')
    .isMongoId()
    .withMessage('Invalid incident ID'),
];

export const validateSeverityQuery = [
  query('severity')
    .optional()
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
];

export const validateDateRangeQuery = [
  query('startDate')
    .optional()
    .isISO8601()
    .withMessage('Start date must be a valid ISO 8601 date'),
  
  query('endDate')
    .optional()
    .isISO8601()
    .withMessage('End date must be a valid ISO 8601 date')
    .custom((value, { req }) => {
      if (value && req.query?.startDate && new Date(value) < new Date(req.query.startDate as string)) {
        throw new Error('End date must be after start date');
      }
      return true;
    }),
];

export const validateVVEIdQuery = [
  query('vveId')
    .optional()
    .isString()
    .trim()
    .notEmpty()
    .withMessage('VVE ID must be a non-empty string'),
];

