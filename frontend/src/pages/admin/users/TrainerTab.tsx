import { UserPageTabType } from "./userPageTabType";


interface TrainerTabProps {
  activeTab: UserPageTabType;
}

export const TrainerTab: React.FC<TrainerTabProps> = ({
  activeTab,
}) => {
    return (
        <>{activeTab}</>
    )
}