import { UserPageTabType } from "./userPageTabType";


interface CmsAdminTabProps {
  activeTab: UserPageTabType;
}

export const CmsAdminTab: React.FC<CmsAdminTabProps> = ({
  activeTab,
}) => {
    return (
        <>{activeTab}</>
    )
}