import { UserPageTabType } from "./userPageTabType";


interface GymAdminTabProps {
  activeTab: UserPageTabType;
}

export const GymAdminTab: React.FC<GymAdminTabProps> = ({
  activeTab,
}) => {
    return (
        <>{activeTab}</>
    )
}