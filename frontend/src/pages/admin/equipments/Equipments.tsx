import React, { useState } from "react";
import { Card, Tabs, TabsProps } from "antd";
import { BrandTab } from "./BrandTab";
import { MuscleGroupsTab } from "./MuscleGroupsTab";
import { EquipmentTab } from "./EquipmentsTab";

export const Equipments: React.FC = () => {
  const [activeTab, setActiveTab] = useState("equipments");

  const items: TabsProps["items"] = [
    {
      key: "equipments",
      label: "Тренажеры",
      children: <EquipmentTab activeTab={activeTab} />,
    },
    {
      key: "brands",
      label: "Брэнды",
      children: <BrandTab activeTab={activeTab} />, // <— передаем сюда активную вкладку
    },
    {
      key: "muscleGroups",
      label: "Мышцы",
      children: <MuscleGroupsTab />,
    },
  ];

  return (
    <div className="p-6">
      <Card title="Тренажеры" className="shadow-sm">
        <Tabs
          activeKey={activeTab}
          onChange={setActiveTab}
          items={items}
          type="card"
        />
      </Card>
    </div>
  );
};