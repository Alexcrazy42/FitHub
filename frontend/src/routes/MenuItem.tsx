
export interface MenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  path?: string;
  element?: React.ReactNode;
  children?: MenuItem[];
}