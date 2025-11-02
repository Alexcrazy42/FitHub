export type UserRole = 'GymVisitor' | 'GymAdmin' | 'Trainer' | 'CmsAdmin';

export const roleMapping: Record<UserRole, string> = {
  'GymVisitor': "Посетитель",
  'GymAdmin': "Администратор",
  "Trainer": "Тренер",
  "CmsAdmin": "Cms администратор"
};


export const roleRoutes: Record<UserRole, string> = {
  'GymVisitor': '/user',
  'GymAdmin': '/gym-admin',
  'Trainer': '/trainer',
  'CmsAdmin': '/admin'
};

export interface User {
  id: string;
  email?: string;
  loginExpirationAt?: Date;
  roles: UserRole[];
  currentRole: UserRole;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  email: string;
  userId: string;
  loginExpirationAt: Date;
  roleNames: UserRole[]
}