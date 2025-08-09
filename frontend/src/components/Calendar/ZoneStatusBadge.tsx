import { Badge } from "antd";

interface ZoneStatusBadgeProps {
  isAvailable: boolean;
}

const ZoneStatusBadge: React.FC<ZoneStatusBadgeProps> = ({ isAvailable }) => (
  <Badge status={isAvailable ? "success" : "error"} text="" />
);

export default ZoneStatusBadge;