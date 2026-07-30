export type UserRole = 'cmsAdmin' | 'gymAdmin';
export type App = 'fithub';

export const APP_URLS : Record<App, string> = {
  'fithub': 'http://localhost:5173'
}

export const BACKEND_URLS : Record<App, string> = {
  'fithub': 'http://localhost:5001'
}

export const POST_LOGIN_URL : Record<UserRole, string> = {
  'cmsAdmin': 'admin',
  'gymAdmin': 'gym-admin'
}

export const PREFIX_URL : Record<UserRole, string> = {
  'cmsAdmin': 'admin',
  'gymAdmin': 'gym-Admin'
}