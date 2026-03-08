import React, { useEffect, useState } from 'react';
import { Empty, Spin, Tabs, Tooltip } from 'antd';
import { useApiService } from '../../../../api/useApiService';
import { useStickerService } from '../../../../api/services/stickerService';
import { getFileRoute } from '../../../../api/files';
import { IStickerGroupResponse, IStickerResponse } from '../../../../types/stickers';

interface StickerPickerProps {
  onSelect: (sticker: IStickerResponse) => void;
}

interface GroupTabProps {
  group: IStickerGroupResponse;
  onSelect: (sticker: IStickerResponse) => void;
}

const GroupTab: React.FC<GroupTabProps> = ({ group, onSelect }) => {
  const apiService = useApiService();
  const stickerService = useStickerService(apiService);

  const [stickers, setStickers] = useState<IStickerResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    if (loaded) return;
    setLoading(true);
    stickerService
      .getStickersByGroup(group.id)
      .then((res) => {
        if (res.success && res.data) setStickers(res.data.items);
      })
      .finally(() => {
        setLoading(false);
        setLoaded(true);
      });
  }, [group.id]);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-24">
        <Spin size="small" />
      </div>
    );
  }

  if (stickers.length === 0) {
    return <Empty description="Нет стикеров" image={Empty.PRESENTED_IMAGE_SIMPLE} />;
  }

  return (
    <div className="grid grid-cols-5 gap-1 p-1">
      {stickers.map((sticker) => (
        <Tooltip key={sticker.id} title={sticker.name} mouseEnterDelay={0.6}>
          <button
            className="w-14 h-14 flex items-center justify-center rounded-lg hover:bg-gray-100 transition cursor-pointer p-1"
            onClick={() => onSelect(sticker)}
          >
            <img
              src={getFileRoute(sticker.fileId)}
              alt={sticker.name}
              className="w-12 h-12 object-contain"
              draggable={false}
            />
          </button>
        </Tooltip>
      ))}
    </div>
  );
};

const StickerPicker: React.FC<StickerPickerProps> = ({ onSelect }) => {
  const apiService = useApiService();
  const stickerService = useStickerService(apiService);

  const [groups, setGroups] = useState<IStickerGroupResponse[]>([]);
  const [loadingGroups, setLoadingGroups] = useState(false);

  useEffect(() => {
    setLoadingGroups(true);
    stickerService
      .getGroups()
      .then((res) => {
        if (res.success && res.data) {
          setGroups(res.data.items.filter((g) => g.isActive));
        }
      })
      .finally(() => setLoadingGroups(false));
  }, []);

  if (loadingGroups) {
    return (
      <div className="flex justify-center items-center w-72 h-40">
        <Spin />
      </div>
    );
  }

  if (groups.length === 0) {
    return (
      <div className="w-72 flex items-center justify-center h-24">
        <Empty description="Нет пакетов стикеров" image={Empty.PRESENTED_IMAGE_SIMPLE} />
      </div>
    );
  }

  const tabItems = groups.map((group) => ({
    key: group.id,
    label: group.name,
    children: <GroupTab group={group} onSelect={onSelect} />,
  }));

  return (
    <div className="w-72">
      <Tabs
        items={tabItems}
        size="small"
        tabBarStyle={{ marginBottom: 4, paddingLeft: 4, paddingRight: 4 }}
      />
    </div>
  );
};

export default StickerPicker;
