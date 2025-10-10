export type UserRole = 'GymVisitor' | 'GymAdmin';

export const roleMapping: Record<UserRole, string> = {
  'GymVisitor': "Посетитель",
  'GymAdmin': "Администратор"
};


export const roleRoutes: Record<UserRole, string> = {
  'GymVisitor': '/user',
  'GymAdmin': '/admin'
};

export interface User {
  id: string;
  name: string;
  email?: string;
  loginExpirationAt?: string;
  roles: UserRole[];
  currentRole: UserRole;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  login: string;
  userId: string;
  name: string;
  loginExpirationAt: Date;
  roleNames: UserRole[]
}