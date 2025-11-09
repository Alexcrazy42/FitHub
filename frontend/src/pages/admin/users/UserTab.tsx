import { UserPageTabType } from "./userPageTabType";


interface UserTabProps {
  activeTab: UserPageTabType;
}

export const UserTab: React.FC<UserTabProps> = ({
  activeTab,
}) => {
    return (
        <>{activeTab}</>
    )
}