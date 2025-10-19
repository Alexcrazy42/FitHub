import React, { useState } from "react";
import { Card, Tabs, TabsProps } from "antd";
import { EquipmentsTab } from "./EquipmentsTab";
import { BrandTab } from "./BrandTab";
import { EquipmentInstructionTab } from "./EquipmentInstructionTab";
import { MuscleGroupsTab } from "./MuscleGroupsTab";

export const Equipments: React.FC = () => {
  const [activeTab, setActiveTab] = useState("equipments");

  const items: TabsProps["items"] = [
    {
      key: "equipments",
      label: "Тренажеры",
      children: <EquipmentsTab />,
    },
    {
        key: "brands",
        label: "Брэнды",
        children: <BrandTab />
    },
    {
        key: "instructions",
        label: "Инструкции",
        children: <EquipmentInstructionTab />
    },
    {
        key: "muscleGroups",
        label: "Мышцы",
        children: <MuscleGroupsTab />
    }
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
