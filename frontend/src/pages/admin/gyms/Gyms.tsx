import React, { useState } from "react";
import { Card, Tabs, TabsProps } from "antd";
import { GymsTab } from "./GymsTab";
import { GymZonesTab } from "./GymZonesTab";

export const Gyms: React.FC = () => {
  const [activeTab, setActiveTab] = useState("gyms");

  const items: TabsProps["items"] = [
    {
      key: "gyms",
      label: "Залы",
      children: <GymsTab />,
    },
    {
      key: "zones",
      label: "Зоны",
      children: <GymZonesTab />,
    },
  ];

  return (
    <div className="p-6">
      <Card title="Спортзалы" className="shadow-sm">
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
