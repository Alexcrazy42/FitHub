export enum UserPageTabType {
  CmsAdmins = "cms-admins",
  GymAdmins = "gym-admins",
  Trainers = "trainers",
  Users = "users"
}


export const isValidTabKey = (key: string): key is UserPageTabType => {
  return Object.values(UserPageTabType).includes(key as UserPageTabType);
};