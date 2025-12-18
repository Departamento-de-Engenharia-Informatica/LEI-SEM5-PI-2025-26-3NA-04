import { body, param, query } from 'express-validator';

export const validateCreateIncidentType = [
  body('code')
    .notEmpty()
    .withMessage('Code is required')
    .isString()
    .withMessage('Code must be a string')
    .trim()
    .toUpperCase()
    .matches(/^T-INC\d{3,}$/)
    .withMessage('Code must follow format: T-INC### (e.g., T-INC001)'),
  
  body('name')
    .notEmpty()
    .withMessage('Name is required')
    .isString()
    .withMessage('Name must be a string')
    .trim()
    .isLength({ min: 3, max: 100 })
    .withMessage('Name must be between 3 and 100 characters'),
  
  body('description')
    .notEmpty()
    .withMessage('Description is required')
    .isString()
    .withMessage('Description must be a string')
    .trim()
    .isLength({ min: 10, max: 500 })
    .withMessage('Description must be between 10 and 500 characters'),
  
  body('severity')
    .notEmpty()
    .withMessage('Severity is required')
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
  
  body('parentId')
    .optional()
    .isMongoId()
    .withMessage('Parent ID must be a valid MongoDB ObjectId'),
];

export const validateUpdateIncidentType = [
  param('id')
    .isMongoId()
    .withMessage('Invalid incident type ID'),
  
  body('code')
    .optional()
    .isString()
    .withMessage('Code must be a string')
    .trim()
    .toUpperCase()
    .matches(/^T-INC\d{3,}$/)
    .withMessage('Code must follow format: T-INC### (e.g., T-INC001)'),
  
  body('name')
    .optional()
    .isString()
    .withMessage('Name must be a string')
    .trim()
    .isLength({ min: 3, max: 100 })
    .withMessage('Name must be between 3 and 100 characters'),
  
  body('description')
    .optional()
    .isString()
    .withMessage('Description must be a string')
    .trim()
    .isLength({ min: 10, max: 500 })
    .withMessage('Description must be between 10 and 500 characters'),
  
  body('severity')
    .optional()
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
  
  body('parentId')
    .optional()
    .isMongoId()
    .withMessage('Parent ID must be a valid MongoDB ObjectId'),
];

export const validateIdParam = [
  param('id')
    .isMongoId()
    .withMessage('Invalid incident type ID'),
];

export const validateSearchQuery = [
  query('q')
    .optional()
    .isString()
    .trim()
    .isLength({ min: 2 })
    .withMessage('Search query must be at least 2 characters'),
];

export const validateSeverityQuery = [
  query('severity')
    .optional()
    .isIn(['Minor', 'Major', 'Critical'])
    .withMessage('Severity must be one of: Minor, Major, Critical'),
];

