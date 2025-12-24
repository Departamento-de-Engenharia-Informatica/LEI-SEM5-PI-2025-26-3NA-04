import { UserRole } from "./user";

export interface MenuItem {
  label: string;
  route: string;
  icon?: string;
  allowedRoles: UserRole[];
  description?: string;
}

export const MENU_ITEMS: MenuItem[] = [
  // Common
  {
    label: 'MENU.HOME',
    route: '/home',
    icon: '🏠',
    allowedRoles: [
      UserRole.SHIPPING_AGENT,
      UserRole.LOGISTICS_OPERATOR,
      UserRole.PORT_AUTHORITY,
      UserRole.ADMIN
    ],
    description: 'Dashboard and overview'
  },


  //Admin
  {
    label: 'MENU.CREATE_USER',
    route: '/create-user',
    icon: '➕',
    allowedRoles: [UserRole.ADMIN],
    description: 'Create new users and send activation link'
  },


  // Port Authority Officer
  {
    label: 'MENU.VESSEL_TYPES',
    route: '/vessel-types',
    icon: '🚢',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Manage vessel type classifications (UC 2.2.1)'
  },
  {
    label: 'MENU.VESSELS',
    route: '/vessels',
    icon: '⚓',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Register and manage vessel records (UC 2.2.2)'
  },
  {
    label: 'MENU.DOCKS',
    route: '/docks',
    icon: '🏗️',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Manage port docks and berths (UC 2.2.3)'
  },
  {
    label: 'MENU.STORAGE_AREAS',
    route: '/storage-areas',
    icon: '📦',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Manage yards and warehouses (UC 2.2.4)'
  },
  {
    label: 'MENU.SHIPPING_AGENTS',
    route: '/shipping-agents',
    icon: '🏢',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Manage shipping agent organizations (UC 2.2.5, 2.2.6)'
  },
  {
    label: 'MENU.VISIT_APPROVALS',
    route: '/visit-approvals',
    icon: '✅',
    allowedRoles: [UserRole.PORT_AUTHORITY, UserRole.ADMIN],
    description: 'Review and approve vessel visit notifications (UC 2.2.7)'
  },

  // Shipping Agent Representative
  {
    label: 'MENU.VISIT_NOTIFICATIONS',
    route: '/visit-notifications',
    icon: '📋',
    allowedRoles: [UserRole.SHIPPING_AGENT, UserRole.ADMIN],
    description: 'Create and manage vessel visit notifications (UC 2.2.8, 2.2.9, 2.2.10)'
  },

  // Logistics Operator
  {
    label: 'MENU.STAFF',
    route: '/staff',
    icon: '👷',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Manage operating staff members (UC 2.2.11)'
  },
  {
    label: 'MENU.RESOURCES',
    route: '/resources',
    icon: '🏗️',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Manage physical resources (cranes, trucks, etc.) (UC 2.2.12)'
  },
  {
    label: 'MENU.QUALIFICATIONS',
    route: '/qualifications',
    icon: '🎓',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Manage staff and resource qualifications (UC 2.2.13)'
  },
  {
    label: 'MENU.OPERATION_PLANS',
    route: '/operation-plans',
    icon: '📋',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Generate and manage operation plans (UC 4.1.2, 4.1.4)'
  },
  {
    label: 'MENU.VESSEL_VISIT_EXECUTIONS',
    route: '/vessel-visit-executions',
    icon: '⚓',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Create and manage vessel visit executions (UC 4.1.7)'
  },
  {
    label: 'MENU.INCIDENT_TYPES',
    route: '/incident-types',
    icon: '⚠️',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Manage incident type classifications (UC 4.1.12)'
  },
  {
    label: 'MENU.INCIDENTS',
    route: '/incidents',
    icon: '🚨',
    allowedRoles: [UserRole.LOGISTICS_OPERATOR, UserRole.ADMIN],
    description: 'Log and manage incidents (UC 4.1.13)'
  },

  // 3D
  {
    label: 'MENU.3D_VISUALIZATION',
    route: '/port-layout',
    icon: '🎲',
    allowedRoles: [
      UserRole.SHIPPING_AGENT,
      UserRole.LOGISTICS_OPERATOR,
      UserRole.PORT_AUTHORITY,
      UserRole.ADMIN
    ],
    description: 'Port visualization and simulation'
  }
];