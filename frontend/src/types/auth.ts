import { IGymResponse } from "./gyms";

export type UserRole = 'GymVisitor' | 'GymAdmin' | 'Trainer' | 'CmsAdmin';

export const roleMapping: Record<UserRole, string> = {
  'GymVisitor': "Посетитель",
  'GymAdmin': "Администратор спортзала",
  "Trainer": "Тренер",
  "CmsAdmin": "Администратор"
};

export const roleRoutes: Record<UserRole, string> = {
  'GymVisitor': '/user',
  'GymAdmin': '/gym-admin',
  'Trainer': '/trainer',
  'CmsAdmin': '/admin'
};

export const rolePriority: Record<UserRole, number> = {
  'CmsAdmin': 1,
  'GymAdmin': 2,
  'Trainer': 3,
  'GymVisitor': 4
};

export const getMostImportantRole = (roles: UserRole[]): UserRole | null => {
  if (!roles || roles.length === 0) return null;
  
  return roles.reduce((mostImportant, currentRole) => {
    const currentPriority = rolePriority[currentRole] ?? 999;
    const mostImportantPriority = rolePriority[mostImportant] ?? 999;
    
    return currentPriority < mostImportantPriority ? currentRole : mostImportant;
  }, roles[0]);
};

export const getMostImportantRoleName = (roles: UserRole[]): string => {
  const role = getMostImportantRole(roles);
  return role ? roleMapping[role] : '';
};

export interface User {
  id: string;
  email?: string;
  roles: UserRole[];
  currentRole: UserRole;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface UserResponse {
  id: string;
  surname: string;
  name: string;
  email: string;
  isActive: boolean;
  startActiveAt: Date;
  roleNames: UserRole[];
}

export interface LoginResponse {
  email: string;
  userId: string;
  isTemporaryPassword: boolean;
  isActive: boolean;
  loginFlowDone: boolean;
  roleNames: UserRole[]
}


export interface StartRegisterRequest {
  surname: string;
  name: string;
  email: string;
  gymId: string;
}

export interface ResetPasswordRequest {
  userId: string;
  token: string;
  newPassword: string | null;
}

export interface ConfirmEmailRequest {
  userId: string;
  token: string;
}

export interface SetPasswordRequest {
  token: string;
  userId: string;
  password: string;
}


export interface CreateCmsAdminRequest {
  email: string;
  surname: string;
  name: string;
}

export interface CreateGymAdminRequest {
  email: string;
  surname: string;
  name: string;
  gymId: string;
}

export interface CreateTrainerAdminRequest {
  email: string;
  surname: string;
  name: string;
  gymId: string;
}

export interface IGymAdminResponse {
  id: string;
  user: UserResponse;
  gyms: IGymResponse[];
}

export interface ITrainerResponse {
  id: string;
  user: UserResponse;
  gyms: IGymResponse[];
}