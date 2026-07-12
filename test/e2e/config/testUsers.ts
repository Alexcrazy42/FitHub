import { UserRole } from "./sites";

export interface Credentials {
  login: string;
  password: string;
  userRole: UserRole;
}


export const FitHubCreds : Record<UserRole, Credentials> = {
    'cmsAdmin' : { 
        login: 'alexcrazy42@mail.ru', 
        password: 'alexcrazy42', 
        userRole: 'cmsAdmin' 
    },
    'gymAdmin' : { 
        login: 'alexcrazy421@mail.ru', 
        password: 'alexcrazy42',
        userRole: 'gymAdmin' 
    }
}