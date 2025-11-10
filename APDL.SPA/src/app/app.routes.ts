import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Cube } from './3d/cube/cube';
import { Dashboard } from './pages/dashboard/dashboard';
import { Shipments } from './pages/shipments/shipments';
import { Operations } from './pages/operations/operations';
import { Reports } from './pages/reports/reports';
import { Admin } from './pages/admin/admin';
import { Layout } from './components/layout/layout';
import { VisitNotifications } from './pages/visit-notifications/visit-notifications';
import { Staff } from './pages/staff/staff';
import { Resources } from './pages/resources/resources';
import { Qualifications } from './pages/qualifications/qualifications';
import { Vessels } from './pages/vessels/vessels';
import { VesselTypes } from './pages/vessel-types/vessel-types';
import { Docks } from './pages/docks/docks';
import { StorageAreas } from './pages/storage-areas/storage-areas';
import { ShippingAgents } from './pages/shipping-agents/shipping-agents';
import { VisitApprovals } from './pages/visit-approvals/visit-approvals';

export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: Home },
      { path: 'vessel-types', component: VesselTypes },
      { path: 'vessels', component: Vessels },
      { path: 'docks', component: Docks },
      { path: 'storage-areas', component: StorageAreas },
      { path: 'shipping-agents', component: ShippingAgents },
      { path: 'visit-approvals', component: VisitApprovals },
      { path: 'visit-notifications', component: VisitNotifications },
      { path: 'staff', component: Staff },
      { path: 'resources', component: Resources },
      { path: 'qualifications', component: Qualifications },
      { path: 'cube', component: Cube }
    ]
  }
];