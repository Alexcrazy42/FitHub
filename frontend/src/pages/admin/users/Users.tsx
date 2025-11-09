import { Tabs, TabsProps } from "antd";
import { useCallback, useState } from "react";
import { isValidTabKey, UserPageTabType } from "./userPageTabType";
import { CmsAdminTab } from "./CmsAdminTab";
import { GymAdminTab } from "./GymAdminTab";
import { TrainerTab } from "./TrainerTab";
import { UserTab } from "./UserTab";


export const Users : React.FC = () => {
    const [activeTab, setActiveTab] = useState<UserPageTabType>(UserPageTabType.CmsAdmins);
    
     const handleTabChange = useCallback((key: string) => {
      if (isValidTabKey(key)) {
        setActiveTab(key);
      } else {
        console.warn("Invalid tab key:", key);
        setActiveTab(UserPageTabType.CmsAdmins);
      }
    }, []);

    const items: TabsProps["items"] = [
        {
          key: UserPageTabType.CmsAdmins,
          label: "CMS администраторы",
          children: <CmsAdminTab activeTab={activeTab} />
        },
        {
          key: UserPageTabType.GymAdmins,
          label: "Администраторы залов",
          children: <GymAdminTab activeTab={activeTab} />
        },
        {
          key: UserPageTabType.Trainers,
          label: "Тренера",
          children: <TrainerTab activeTab={activeTab} />
        },
        {
          key: UserPageTabType.Users,
          label: "Пользователи",
          children: <UserTab activeTab={activeTab} />
        },
      ];

    return (
        <div className="p-6">
            <Tabs
                activeKey={activeTab}
                onChange={handleTabChange}
                items={items}
                type="card"
            />
        </div>
  );
}