export type UserRole = 'cmsAdmin' | 'gymAdmin';
export type App = 'fithub';

export const APP_URLS : Record<App, string> = {
  'fithub': 'http://localhost:5173' // TODO: config
}

export const POST_LOGIN_URL : Record<UserRole, string> = {
  'cmsAdmin': 'admin',
  'gymAdmin': 'gym-admin'
}