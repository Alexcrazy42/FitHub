import { Tabs, TabsProps } from "antd";
import { useState } from "react";

export const Users : React.FC = () => {
    const [activeTab, setActiveTab] = useState("gym-admins");
    
    const items: TabsProps["items"] = [
        {
          key: "gym-admins",
          label: "Администраторы залов",
        //   children: <BaseGroupTrainingTab activeTab={activeTab} />,
        },
        {
          key: "trainers",
          label: "Тренера",
        //   children: <TrainingTypeTab activeTab={activeTab} />,
        },
        {
          key: "users",
          label: "Пользователи",
        //   children: <TrainingTypeTab activeTab={activeTab} />,
        },
      ];

    return (
        <div className="p-6">
        {/* <Card title="Тренировки" className="shadow-sm"> */}
            <Tabs
                activeKey={activeTab}
                onChange={setActiveTab}
                items={items}
                type="card"
            />
        {/* </Card> */}
        </div>
  );
}