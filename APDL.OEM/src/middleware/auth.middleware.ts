import { Request, Response, NextFunction } from 'express';
import { checkJwt } from '../config/auth0';

// Extend Express Request to include user and auth
export interface AuthRequest extends Request {
  auth?: {
    sub: string;
    email?: string;
    [key: string]: any;
  };
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
    
    // Extract user info from req.auth (express-jwt stores token payload here)
    const authReq = req as AuthRequest;
    if (authReq.auth) {
      authReq.user = {
        ...authReq.auth,
        // Override email if available from custom claim
        email: authReq.auth.email || authReq.auth['https://apdl-operations.eu.auth0.com/email'] || undefined,
      };
    }
    
    next();
  });
};

export const authorize = (...allowedRoles: string[]) => {
  return (req: AuthRequest, res: Response, next: NextFunction) => {
    next();
  };
};

