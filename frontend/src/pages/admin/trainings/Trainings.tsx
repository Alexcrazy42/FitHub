import { Tabs, TabsProps } from "antd";
import { useState } from "react";
import { BaseGroupTrainingTab } from "./BaseGroupTrainingTab";
import { TrainingTypeTab } from "./TrainingTypeTab";

export const Trainings : React.FC = () => {
    const [activeTab, setActiveTab] = useState("trainings");
    
    const items: TabsProps["items"] = [
        {
          key: "trainings",
          label: "Тренировки",
          children: <BaseGroupTrainingTab activeTab={activeTab} />,
        },
        {
          key: "trainingTypes",
          label: "Типы тренировок",
          children: <TrainingTypeTab activeTab={activeTab} />,
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