import React, { useEffect, useRef, useState } from 'react';
import {
  Badge,
  Button,
  Card,
  Drawer,
  Form as AntForm,
  Input,
  Modal,
  Popconfirm,
  Space,
  Table,
  Tag,
  Tooltip,
} from 'antd';
import {
  CheckOutlined,
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
  ReloadOutlined,
  SwapOutlined,
} from '@ant-design/icons';
import { toast } from 'react-toastify';
import { Controller, useForm } from 'react-hook-form';
import { useApiService } from '../../../api/useApiService';
import { useStickerService } from '../../../api/services/stickerService';
import { getFileRoute } from '../../../api/files';
import { IStickerGroupResponse, IStickerResponse } from '../../../types/stickers';
import ImageUploader, { ImageUploaderHandle } from '../../../components/ImageUploader/ImageUploader';

// ─── Group form ──────────────────────────────────────────────────────────────

interface GroupFormValues {
  name: string;
}

// ─── Sticker form ────────────────────────────────────────────────────────────

interface AddStickerFormValues {
  name: string;
}

// ─── Main page ───────────────────────────────────────────────────────────────

export const StickersPage: React.FC = () => {
  const apiService = useApiService();
  const stickerService = useStickerService(apiService);

  // Groups list
  const [groups, setGroups] = useState<IStickerGroupResponse[]>([]);
  const [groupsLoading, setGroupsLoading] = useState(false);

  // Selected group + its stickers
  const [selectedGroup, setSelectedGroup] = useState<IStickerGroupResponse | null>(null);
  const [stickers, setStickers] = useState<IStickerResponse[]>([]);
  const [stickersLoading, setStickersLoading] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);

  // Create group modal
  const [createGroupOpen, setCreateGroupOpen] = useState(false);
  const [createGroupLoading, setCreateGroupLoading] = useState(false);
  const createGroupForm = useForm<GroupFormValues>({ defaultValues: { name: '' } });

  // Edit group name modal
  const [editGroupOpen, setEditGroupOpen] = useState(false);
  const [editGroupLoading, setEditGroupLoading] = useState(false);
  const [editingGroup, setEditingGroup] = useState<IStickerGroupResponse | null>(null);
  const editGroupForm = useForm<GroupFormValues>({ defaultValues: { name: '' } });

  // Add sticker modal
  const [addStickerOpen, setAddStickerOpen] = useState(false);
  const [addStickerLoading, setAddStickerLoading] = useState(false);
  const [pendingStickerFileId, setPendingStickerFileId] = useState<string | null>(null);
  const addStickerForm = useForm<AddStickerFormValues>({ defaultValues: { name: '' } });
  const addStickerUploaderRef = useRef<ImageUploaderHandle>(null);

  // Rename sticker modal
  const [renameStickerOpen, setRenameStickerOpen] = useState(false);
  const [renameStickerLoading, setRenameStickerLoading] = useState(false);
  const [renamingSticker, setRenamingSticker] = useState<IStickerResponse | null>(null);
  const renameStickerForm = useForm<AddStickerFormValues>({ defaultValues: { name: '' } });

  // Replace photo uploader (per-sticker, hidden input)
  const replacePhotoRef = useRef<ImageUploaderHandle>(null);
  const [replacingSticker, setReplacingSticker] = useState<IStickerResponse | null>(null);

  // ── Fetch groups ────────────────────────────────────────────────────────────

  const fetchGroups = async () => {
    setGroupsLoading(true);
    try {
      const res = await stickerService.getGroups();
      if (res.success && res.data) setGroups(res.data.items);
      else toast.error(res.error?.detail ?? 'Ошибка загрузки групп');
    } finally {
      setGroupsLoading(false);
    }
  };

  useEffect(() => { fetchGroups(); }, []);

  // ── Fetch stickers for a group ──────────────────────────────────────────────

  const fetchStickers = async (group: IStickerGroupResponse) => {
    setStickersLoading(true);
    try {
      const res = await stickerService.getStickersByGroup(group.id);
      if (res.success && res.data) setStickers(res.data.items);
      else toast.error(res.error?.detail ?? 'Ошибка загрузки стикеров');
    } finally {
      setStickersLoading(false);
    }
  };

  const openGroupDrawer = (group: IStickerGroupResponse) => {
    setSelectedGroup(group);
    setDrawerOpen(true);
    fetchStickers(group);
  };

  // ── Create group ────────────────────────────────────────────────────────────

  const handleCreateGroup = async (values: GroupFormValues) => {
    setCreateGroupLoading(true);
    try {
      const res = await stickerService.createGroup(values.name.trim());
      if (res.success && res.data) {
        toast.success('Группа создана');
        setCreateGroupOpen(false);
        createGroupForm.reset();
        await fetchGroups();
        openGroupDrawer(res.data);
      } else {
        toast.error(res.error?.detail ?? 'Ошибка создания группы');
      }
    } finally {
      setCreateGroupLoading(false);
    }
  };

  // ── Edit group name ─────────────────────────────────────────────────────────

  const openEditGroup = (group: IStickerGroupResponse, e: React.MouseEvent) => {
    e.stopPropagation();
    setEditingGroup(group);
    editGroupForm.reset({ name: group.name });
    setEditGroupOpen(true);
  };

  const handleEditGroup = async (values: GroupFormValues) => {
    if (!editingGroup) return;
    setEditGroupLoading(true);
    try {
      const res = await stickerService.updateGroup(editingGroup.id, values.name.trim());
      if (res.success && res.data) {
        toast.success('Название обновлено');
        setEditGroupOpen(false);
        if (selectedGroup?.id === editingGroup.id) setSelectedGroup(res.data);
        await fetchGroups();
      } else {
        toast.error(res.error?.detail ?? 'Ошибка обновления');
      }
    } finally {
      setEditGroupLoading(false);
    }
  };

  // ── Activate group ──────────────────────────────────────────────────────────

  const handleActivateGroup = async (group: IStickerGroupResponse, e: React.MouseEvent) => {
    e.stopPropagation();
    const res = await stickerService.activateGroup(group.id);
    if (res.success && res.data) {
      toast.success('Группа активирована');
      if (selectedGroup?.id === group.id) setSelectedGroup(res.data);
      await fetchGroups();
    } else {
      toast.error(res.error?.detail ?? 'Ошибка активации');
    }
  };

  // ── Delete group ────────────────────────────────────────────────────────────

  const handleDeleteGroup = async (group: IStickerGroupResponse) => {
    const res = await stickerService.deleteGroup(group.id);
    if (res.success) {
      toast.success('Группа удалена');
      if (selectedGroup?.id === group.id) {
        setDrawerOpen(false);
        setSelectedGroup(null);
      }
      await fetchGroups();
    } else {
      toast.error(res.error?.detail ?? 'Ошибка удаления');
    }
  };

  // ── Add sticker ─────────────────────────────────────────────────────────────

  const handleStickerFileUploaded = async (fileId: string) => {
    setPendingStickerFileId(fileId);
  };

  const handleAddSticker = async (values: AddStickerFormValues) => {
    if (!selectedGroup || !pendingStickerFileId) {
      toast.error('Сначала загрузите изображение стикера');
      return;
    }
    setAddStickerLoading(true);
    try {
      const res = await stickerService.addSticker(selectedGroup.id, pendingStickerFileId, values.name.trim());
      if (res.success) {
        toast.success('Стикер добавлен');
        setAddStickerOpen(false);
        addStickerForm.reset();
        setPendingStickerFileId(null);
        await fetchStickers(selectedGroup);
      } else {
        toast.error(res.error?.detail ?? 'Ошибка добавления стикера');
      }
    } finally {
      setAddStickerLoading(false);
    }
  };

  // ── Rename sticker ──────────────────────────────────────────────────────────

  const openRenameSticker = (sticker: IStickerResponse) => {
    setRenamingSticker(sticker);
    renameStickerForm.reset({ name: sticker.name });
    setRenameStickerOpen(true);
  };

  const handleRenameSticker = async (values: AddStickerFormValues) => {
    if (!renamingSticker || !selectedGroup) return;
    setRenameStickerLoading(true);
    try {
      const res = await stickerService.updateStickerName(renamingSticker.id, values.name.trim());
      if (res.success) {
        toast.success('Стикер переименован');
        setRenameStickerOpen(false);
        await fetchStickers(selectedGroup);
      } else {
        toast.error(res.error?.detail ?? 'Ошибка переименования');
      }
    } finally {
      setRenameStickerLoading(false);
    }
  };

  // ── Replace sticker photo ───────────────────────────────────────────────────

  const handleReplacePhoto = (sticker: IStickerResponse) => {
    setReplacingSticker(sticker);
    replacePhotoRef.current?.openFileDialog();
  };

  const handleReplaceFileUploaded = async (newFileId: string) => {
    if (!replacingSticker || !selectedGroup) return;
    const res = await stickerService.updateStickerPhoto(replacingSticker.id, newFileId);
    if (res.success) {
      toast.success('Фото заменено');
      setReplacingSticker(null);
      await fetchStickers(selectedGroup);
    } else {
      toast.error(res.error?.detail ?? 'Ошибка замены фото');
    }
  };

  // ── Delete sticker ──────────────────────────────────────────────────────────

  const handleDeleteSticker = async (sticker: IStickerResponse) => {
    if (!selectedGroup) return;
    const res = await stickerService.removeSticker(sticker.id);
    if (res.success) {
      toast.success('Стикер удалён');
      await fetchStickers(selectedGroup);
    } else {
      toast.error(res.error?.detail ?? 'Ошибка удаления стикера');
    }
  };

  // ── Table columns ───────────────────────────────────────────────────────────

  const groupColumns = [
    {
      title: 'Название',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Статус',
      key: 'isActive',
      render: (_: unknown, record: IStickerGroupResponse) =>
        record.isActive ? (
          <Badge status="success" text="Активна" />
        ) : (
          <Badge status="default" text="Неактивна" />
        ),
    },
    {
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, record: IStickerGroupResponse) => (
        <Space onClick={(e) => e.stopPropagation()}>
          {!record.isActive && (
            <Tooltip title="Активировать">
              <Button
                type="text"
                icon={<CheckOutlined />}
                onClick={(e) => handleActivateGroup(record, e)}
              />
            </Tooltip>
          )}
          <Tooltip title="Переименовать">
            <Button
              type="text"
              icon={<EditOutlined />}
              onClick={(e) => openEditGroup(record, e)}
            />
          </Tooltip>
          <Popconfirm
            title="Удалить группу и все её стикеры?"
            okText="Удалить"
            cancelText="Отмена"
            okButtonProps={{ danger: true }}
            onConfirm={() => handleDeleteGroup(record)}
          >
            <Button type="text" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  // ── Render ──────────────────────────────────────────────────────────────────

  return (
    <div className="p-6">
      <Card
        title="Стикеры"
        className="shadow-sm"
        extra={
          <Space>
            <Button icon={<ReloadOutlined />} onClick={fetchGroups}>
              Обновить
            </Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={() => setCreateGroupOpen(true)}>
              Добавить группу
            </Button>
          </Space>
        }
      >
        <Table
          columns={groupColumns}
          dataSource={groups}
          rowKey="id"
          loading={groupsLoading}
          pagination={false}
          onRow={(record) => ({
            onClick: () => openGroupDrawer(record),
            style: { cursor: 'pointer' },
          })}
          locale={{ emptyText: 'Нет групп стикеров' }}
        />
      </Card>

      {/* ── Drawer: stickers in group ─────────────────────────────────────── */}
      <Drawer
        title={
          <span>
            Стикеры группы:{' '}
            <span className="font-semibold">{selectedGroup?.name}</span>
            {selectedGroup?.isActive ? (
              <Tag color="success" className="ml-2">Активна</Tag>
            ) : (
              <Tag color="default" className="ml-2">Неактивна</Tag>
            )}
          </span>
        }
        placement="right"
        width={520}
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        destroyOnClose
        extra={
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => {
              setPendingStickerFileId(null);
              addStickerForm.reset();
              setAddStickerOpen(true);
            }}
          >
            Добавить стикер
          </Button>
        }
      >
        {stickersLoading ? (
          <div className="flex justify-center items-center h-32 text-gray-400">Загрузка...</div>
        ) : stickers.length === 0 ? (
          <div className="flex justify-center items-center h-32 text-gray-400">
            Стикеров нет. Добавьте первый!
          </div>
        ) : (
          <div className="grid grid-cols-3 gap-4">
            {stickers.map((sticker) => (
              <div
                key={sticker.id}
                className="flex flex-col items-center border rounded-xl p-2 gap-2 bg-gray-50 hover:bg-white hover:shadow transition"
              >
                <img
                  src={getFileRoute(sticker.fileId)}
                  alt={sticker.name}
                  className="w-20 h-20 object-contain"
                />
                <span className="text-xs text-gray-600 text-center truncate w-full">{sticker.name}</span>
                <Space size={4}>
                  <Tooltip title="Переименовать">
                    <Button
                      size="small"
                      type="text"
                      icon={<EditOutlined />}
                      onClick={() => openRenameSticker(sticker)}
                    />
                  </Tooltip>
                  <Tooltip title="Заменить изображение">
                    <Button
                      size="small"
                      type="text"
                      icon={<SwapOutlined />}
                      onClick={() => handleReplacePhoto(sticker)}
                    />
                  </Tooltip>
                  <Popconfirm
                    title="Удалить стикер?"
                    okText="Удалить"
                    cancelText="Отмена"
                    okButtonProps={{ danger: true }}
                    onConfirm={() => handleDeleteSticker(sticker)}
                  >
                    <Button size="small" type="text" danger icon={<DeleteOutlined />} />
                  </Popconfirm>
                </Space>
              </div>
            ))}
          </div>
        )}

        {/* Hidden uploader for replacing sticker photo */}
        <ImageUploader
          ref={replacePhotoRef}
          maxFileCount={1}
          fileUpload={handleReplaceFileUploaded}
          onSuccessCancel={() => {}}
        />
      </Drawer>

      {/* ── Modal: create group ───────────────────────────────────────────── */}
      <Modal
        title="Новая группа стикеров"
        open={createGroupOpen}
        onCancel={() => { setCreateGroupOpen(false); createGroupForm.reset(); }}
        onOk={createGroupForm.handleSubmit(handleCreateGroup)}
        confirmLoading={createGroupLoading}
        okText="Создать"
        cancelText="Отмена"
        destroyOnClose
      >
        <AntForm layout="vertical">
          <AntForm.Item
            label="Название"
            validateStatus={createGroupForm.formState.errors.name ? 'error' : ''}
            help={createGroupForm.formState.errors.name?.message}
          >
            <Controller
              name="name"
              control={createGroupForm.control}
              rules={{ required: 'Введите название' }}
              render={({ field }) => <Input {...field} placeholder="Например: Реакции" autoFocus />}
            />
          </AntForm.Item>
        </AntForm>
      </Modal>

      {/* ── Modal: edit group name ────────────────────────────────────────── */}
      <Modal
        title="Переименовать группу"
        open={editGroupOpen}
        onCancel={() => setEditGroupOpen(false)}
        onOk={editGroupForm.handleSubmit(handleEditGroup)}
        confirmLoading={editGroupLoading}
        okText="Сохранить"
        cancelText="Отмена"
        destroyOnClose
      >
        <AntForm layout="vertical">
          <AntForm.Item
            label="Название"
            validateStatus={editGroupForm.formState.errors.name ? 'error' : ''}
            help={editGroupForm.formState.errors.name?.message}
          >
            <Controller
              name="name"
              control={editGroupForm.control}
              rules={{ required: 'Введите название' }}
              render={({ field }) => <Input {...field} autoFocus />}
            />
          </AntForm.Item>
        </AntForm>
      </Modal>

      {/* ── Modal: add sticker ────────────────────────────────────────────── */}
      <Modal
        title="Добавить стикер"
        open={addStickerOpen}
        onCancel={() => { setAddStickerOpen(false); setPendingStickerFileId(null); addStickerForm.reset(); }}
        onOk={addStickerForm.handleSubmit(handleAddSticker)}
        confirmLoading={addStickerLoading}
        okText="Добавить"
        cancelText="Отмена"
        destroyOnClose
      >
        <AntForm layout="vertical">
          <AntForm.Item label="Изображение">
            <div className="flex items-center gap-3">
              {pendingStickerFileId ? (
                <img
                  src={getFileRoute(pendingStickerFileId)}
                  alt="preview"
                  className="w-20 h-20 object-contain border rounded-lg"
                />
              ) : (
                <div className="w-20 h-20 border-2 border-dashed border-gray-300 rounded-lg flex items-center justify-center text-gray-400 text-xs text-center">
                  Нет файла
                </div>
              )}
              <Button onClick={() => addStickerUploaderRef.current?.openFileDialog()}>
                {pendingStickerFileId ? 'Заменить' : 'Загрузить'}
              </Button>
            </div>
            <ImageUploader
              ref={addStickerUploaderRef}
              maxFileCount={1}
              fileUpload={handleStickerFileUploaded}
              onSuccessCancel={() => {}}
            />
          </AntForm.Item>
          <AntForm.Item
            label="Название"
            validateStatus={addStickerForm.formState.errors.name ? 'error' : ''}
            help={addStickerForm.formState.errors.name?.message}
          >
            <Controller
              name="name"
              control={addStickerForm.control}
              rules={{ required: 'Введите название' }}
              render={({ field }) => <Input {...field} placeholder="Например: thumbs up" />}
            />
          </AntForm.Item>
        </AntForm>
      </Modal>

      {/* ── Modal: rename sticker ─────────────────────────────────────────── */}
      <Modal
        title="Переименовать стикер"
        open={renameStickerOpen}
        onCancel={() => setRenameStickerOpen(false)}
        onOk={renameStickerForm.handleSubmit(handleRenameSticker)}
        confirmLoading={renameStickerLoading}
        okText="Сохранить"
        cancelText="Отмена"
        destroyOnClose
      >
        <AntForm layout="vertical">
          <AntForm.Item
            label="Название"
            validateStatus={renameStickerForm.formState.errors.name ? 'error' : ''}
            help={renameStickerForm.formState.errors.name?.message}
          >
            <Controller
              name="name"
              control={renameStickerForm.control}
              rules={{ required: 'Введите название' }}
              render={({ field }) => <Input {...field} autoFocus />}
            />
          </AntForm.Item>
        </AntForm>
      </Modal>
    </div>
  );
};
