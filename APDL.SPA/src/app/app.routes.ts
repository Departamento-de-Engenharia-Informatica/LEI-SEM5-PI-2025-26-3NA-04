import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { Cube } from './3d/cube/cube';
import { Layout } from './components/layout/layout';
import { AccessDenied } from './pages/access-denied/access-denied';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { UserRole } from './models/user';
import { VesselTypes } from './pages/vessel-types/vessel-types';
import { Vessels } from './pages/vessels/vessels';
import { Docks } from './pages/docks/docks';
import { StorageAreas } from './pages/storage-areas/storage-areas';
import { ShippingAgents } from './pages/shipping-agents/shipping-agents';
import { VisitApprovals } from './pages/visit-approvals/visit-approvals';
import { VisitNotifications } from './pages/visit-notifications/visit-notifications';
import { Staff } from './pages/staff/staff';
import { Resources } from './pages/resources/resources';
import { Qualifications } from './pages/qualifications/qualifications';
import { PortSceneComponent } from './3d/port-scene/port-scene';
import { OperationPlans } from './pages/operation-plans/operation-plans';
import { VesselVisitExecutions } from './pages/vessel-visit-executions/vessel-visit-executions';
import { IncidentTypes } from './pages/incident-types/incident-types';
import { Incidents } from './pages/incidents/incidents';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: Login
  },

  {
    path: 'access-denied',
    component: AccessDenied,
    canActivate: [authGuard]
  },

  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      {
        path: 'home',
        component: Home
      },

      {
        path: 'port-layout', 
        component: PortSceneComponent
      },

      {
        path: 'vessel-types',
        component: VesselTypes,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },
      {
        path: 'vessels',
        component: Vessels,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },
      {
        path: 'docks',
        component: Docks,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },
      {
        path: 'storage-areas',
        component: StorageAreas,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },
      {
        path: 'shipping-agents',
        component: ShippingAgents,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },
      {
        path: 'visit-approvals',
        component: VisitApprovals,
        canActivate: [roleGuard([UserRole.PORT_AUTHORITY, UserRole.ADMIN])]
      },

      {
        path: 'visit-notifications',
        component: VisitNotifications,
        canActivate: [roleGuard([UserRole.SHIPPING_AGENT, UserRole.ADMIN])]
      },

      {
        path: 'staff',
        component: Staff,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'resources',
        component: Resources,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'qualifications',
        component: Qualifications,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'operation-plans',
        component: OperationPlans,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'vessel-visit-executions',
        component: VesselVisitExecutions,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'incident-types',
        component: IncidentTypes,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },
      {
        path: 'incidents',
        component: Incidents,
        canActivate: [roleGuard([UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN])]
      },

      {
        path: 'cube',
        component: Cube
      },
      {
        path: 'create-user',
        loadComponent: () => import('./pages/create-user/create-user').then(m => m.CreateUser),
        canActivate: [roleGuard([UserRole.ADMIN])]
      }

    ]
  },

  {
    path: '**',
    redirectTo: '/home'
  }
];