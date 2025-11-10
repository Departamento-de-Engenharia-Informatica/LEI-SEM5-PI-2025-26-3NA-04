export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  isAuthenticated: boolean;
}

export enum UserRole {
  SHIPPING_AGENT = 'Shipping Agent Representative',
  LOGISTICS_OPERATOR = 'Logistics Operator',
  PORT_AUTHORITY = 'Port Authority Officer',
  ADMIN = 'Administrator',
}