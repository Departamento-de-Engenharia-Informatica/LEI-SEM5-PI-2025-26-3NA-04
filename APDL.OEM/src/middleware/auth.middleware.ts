import { Request, Response, NextFunction } from 'express';
import { checkJwt } from '../config/auth0';

// Extend Express Request to include user
export interface AuthRequest extends Request {
  user?: {
    sub: string;
    email?: string;
    [key: string]: any;
  };
}

// Authentication middleware wrapper
export const authenticate = (req: Request, res: Response, next: NextFunction) => {
  checkJwt(req, res, (err) => {
    if (err) {
      return res.status(401).json({
        error: 'Unauthorized',
        message: 'Invalid or missing authentication token',
      });
    }
    next();
  });
};

export const authorize = (...allowedRoles: string[]) => {
  return (req: AuthRequest, res: Response, next: NextFunction) => {
    next();
  };
};

