import { expressjwt, GetVerificationKey } from 'express-jwt';
import jwksRsa from 'jwks-rsa';

export const auth0Config = {
  domain: process.env.AUTH0_DOMAIN || 'apdl-operations.eu.auth0.com',
  audience: process.env.AUTH0_AUDIENCE || 'https://localhost:5001/api',
  issuer: process.env.AUTH0_ISSUER || 'https://apdl-operations.eu.auth0.com/',
};

// JWT validation middleware
export const checkJwt = expressjwt({
  secret: jwksRsa.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
    jwksUri: `https://${auth0Config.domain}/.well-known/jwks.json`,
  }) as GetVerificationKey,
  audience: auth0Config.audience,
  issuer: auth0Config.issuer,
  algorithms: ['RS256'],
});

