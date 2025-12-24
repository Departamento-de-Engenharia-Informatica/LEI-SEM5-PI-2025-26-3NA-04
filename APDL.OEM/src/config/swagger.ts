import swaggerJsdoc from 'swagger-jsdoc';

const options: swaggerJsdoc.Options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'APDL Operations & Execution Management API',
      version: '1.0.0',
      description: 'REST API for the Operations & Execution Management (OEM) module of the APDL system. This module manages execution data of port activities, including Incident Types, Incidents, and Vessel Visit Executions (VVEs).',
      contact: {
        name: 'APDL Team',
      },
    },
    servers: [
      {
        url: process.env.API_URL || 'http://localhost:3000',
        description: 'Development server',
      },
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: 'http',
          scheme: 'bearer',
          bearerFormat: 'JWT',
          description: 'Auth0 JWT token. Format: Bearer <token>',
        },
      },
      schemas: {
        Error: {
          type: 'object',
          properties: {
            error: {
              type: 'string',
              description: 'Error type',
            },
            message: {
              type: 'string',
              description: 'Error message',
            },
          },
        },
        ValidationError: {
          type: 'object',
          properties: {
            errors: {
              type: 'array',
              items: {
                type: 'object',
                properties: {
                  type: { type: 'string' },
                  msg: { type: 'string' },
                  path: { type: 'string' },
                  location: { type: 'string' },
                },
              },
            },
          },
        },
        IncidentType: {
          type: 'object',
          properties: {
            _id: { type: 'string' },
            code: { type: 'string', example: 'T-INC001' },
            name: { type: 'string', example: 'Equipment Failure' },
            description: { type: 'string' },
            severity: { type: 'string', enum: ['Minor', 'Major', 'Critical'] },
            parentId: { type: 'string', nullable: true },
            createdAt: { type: 'string', format: 'date-time' },
            updatedAt: { type: 'string', format: 'date-time' },
          },
        },
        Incident: {
          type: 'object',
          properties: {
            _id: { type: 'string' },
            incidentTypeId: { type: 'string' },
            startTime: { type: 'string', format: 'date-time' },
            endTime: { type: 'string', format: 'date-time', nullable: true },
            duration: { type: 'number', nullable: true, description: 'Duration in minutes' },
            severity: { type: 'string', enum: ['Minor', 'Major', 'Critical'] },
            description: { type: 'string' },
            responsibleUser: { type: 'string' },
            affectedVVEIds: {
              type: 'array',
              items: { type: 'string' },
            },
            affectsAllOngoingVVEs: { type: 'boolean' },
            affectsAllUpcomingVVEs: { type: 'boolean' },
            status: { type: 'string', enum: ['active', 'resolved'] },
            createdAt: { type: 'string', format: 'date-time' },
            updatedAt: { type: 'string', format: 'date-time' },
          },
        },
        VesselVisitExecution: {
          type: 'object',
          properties: {
            _id: { type: 'string' },
            vveId: { type: 'string', description: 'Auto-generated GUID' },
            vvnId: { type: 'string', description: 'Reference to VVN from ASP.NET backend' },
            vesselId: { type: 'string', description: 'Vessel identifier from ASP.NET backend' },
            actualArrivalTime: { type: 'string', format: 'date-time' },
            creatorUserId: { type: 'string', description: 'User ID from Auth0 token' },
            status: { type: 'string', enum: ['In Progress', 'Completed'] },
            actualBerthTime: { type: 'string', format: 'date-time', nullable: true },
            actualDockId: { type: 'string', nullable: true },
            actualUnberthTime: { type: 'string', format: 'date-time', nullable: true },
            actualDepartureTime: { type: 'string', format: 'date-time', nullable: true },
            createdAt: { type: 'string', format: 'date-time' },
            updatedAt: { type: 'string', format: 'date-time' },
          },
        },
        OperationPlan: {
          type: 'object',
          properties: {
            _id: { type: 'string' },
            planId: { type: 'string', description: 'Auto-generated plan identifier (GUID)' },
            vvnIds: {
              type: 'array',
              items: { type: 'string' },
              description: 'Array of VVN IDs used to generate this plan',
            },
            schedule: {
              type: 'array',
              items: {
                type: 'object',
                properties: {
                  vesselId: { type: 'string' },
                  vvnId: { type: 'string' },
                  vesselLabel: { type: 'string' },
                  arrivalTime: { type: 'number', description: 'Time units (1 unit = 10 minutes)' },
                  unloadStartTime: { type: 'number' },
                  loadEndTime: { type: 'number' },
                  dockId: { type: 'string' },
                  craneId: { type: 'string' },
                  staffId: { type: 'string' },
                  storageId: { type: 'string' },
                  timeFactor: { type: 'number' },
                  delay: { type: 'number' },
                  unloadTimeUnits: { type: 'number' },
                  loadTimeUnits: { type: 'number' },
                },
              },
            },
            status: { type: 'string', enum: ['Generated', 'Manual', 'Approved'] },
            totalDelay: { type: 'number', description: 'Total delay in time units' },
            algorithmUsed: { type: 'string', description: 'Prolog algorithm used' },
            creatorUserId: { type: 'string' },
            notes: { type: 'string', nullable: true },
            createdAt: { type: 'string', format: 'date-time' },
            updatedAt: { type: 'string', format: 'date-time' },
          },
        },
      },
      responses: {
        UnauthorizedError: {
          description: 'Authentication required',
          content: {
            'application/json': {
              schema: {
                $ref: '#/components/schemas/Error',
              },
            },
          },
        },
        ValidationError: {
          description: 'Validation error',
          content: {
            'application/json': {
              schema: {
                $ref: '#/components/schemas/ValidationError',
              },
            },
          },
        },
      },
    },
    security: [
      {
        bearerAuth: [],
      },
    ],
  },
  apis: ['./src/routes/*.ts', './src/controllers/*.ts'],
};

export const swaggerSpec = swaggerJsdoc(options);

