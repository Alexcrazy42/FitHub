import { UserRole } from "../fixtures/fixtures"

export const FitHubCreds : Record<UserRole, {login: string, password: string}> = {
    'cmsAdmin' : {login: 'alexcrazy42@mail.ru', password: 'alexcrazy42'},
    'gymAdmin' : {login: 'alexcrazy421@mail.ru', password: 'alexcrazy42'}
}