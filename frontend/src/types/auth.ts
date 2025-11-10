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
  surname: string;
  name: string;
  email: string;
  isActive: boolean;
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
}