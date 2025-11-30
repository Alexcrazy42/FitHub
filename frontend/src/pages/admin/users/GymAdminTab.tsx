import React, { useState, useEffect, useRef } from 'react';
import { UserPageTabType } from './userPageTabType';
import {
  CreateGymAdminRequest,
  IGymAdminResponse,
  roleMapping,
} from '../../../types/auth';
import { useApiService } from '../../../api/useApiService';
import { ListResponse } from '../../../types/common';
import { toast } from 'react-toastify';
import { Table, Pagination, Button, Modal, Input, Space, Select, Menu } from 'antd';
import { useForm, Controller } from 'react-hook-form';
import { IGymResponse } from '../../../types/gyms';
import './styles/gymAdminTab.css'

interface GymAdminTabProps {
  activeTab: UserPageTabType;
}

export const GymAdminTab: React.FC<GymAdminTabProps> = ({ activeTab }) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [items, setItems] = useState<IGymAdminResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [gymOptions, setGymOptions] = useState<{ value: string; label: string }[]>([]);
  const [formLoading, setFormLoading] = useState(false);
  const apiService = useApiService();
  const [contextMenu, setContextMenu] = useState<{ visible: boolean; x: number; y: number; record?: IGymAdminResponse }>({ visible: false, x: 0, y: 0 });
  const menuRef = useRef<HTMLDivElement>(null);

  const {
    control,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
    trigger
  } = useForm<CreateGymAdminRequest>({
    defaultValues: {
      email: '',
      surname: '',
      name: '',
      gymId: '',
    },
  });

  useEffect(() => {
    const loadGyms = async () => {
      try {
        const response = await apiService.get<ListResponse<IGymResponse>>('/v1/gyms?PageSize=1000');
        if (response.success && response.data?.items) {
          setGymOptions(
            response.data.items.map((gym) => ({
              value: gym.id,
              label: `${gym.name}`,
            }))
          );
        }
      } catch (err) {
        console.error('Failed to load gyms', err);
        toast.warn('Не удалось загрузить список спортзалов');
      }
    };

    if (isModalOpen && gymOptions.length === 0) {
      loadGyms();
    }
  }, [isModalOpen, apiService]);

  // Загрузка администраторов при активации вкладки
  useEffect(() => {
    if (activeTab === UserPageTabType.GymAdmins) {
      fetchAll();
    }
  }, [page, pageSize, activeTab]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (contextMenu.visible && menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setContextMenu({ ...contextMenu, visible: false });
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [contextMenu]);

  const fetchAll = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<IGymAdminResponse>>(
        `/v1/gym-admins?PageNumber=${page}&PageSize=${pageSize}`
      );

      if (response.success && response.data) {
        const userItems = response.data.items;
        setItems(userItems);
        setTotal(response.data.totalItems ?? userItems.length);
      } else {
        toast.error(response.error?.detail ?? 'Не удалось загрузить список администраторов спортзалов');
        setItems([]);
        setTotal(0);
      }
    } catch (err) {
      console.error('Fetch gym admins error:', err);
      toast.error('Ошибка сети при загрузке администраторов');
      setItems([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateGymAdminRequest) => {
    setFormLoading(true);
    try {
      const response = await apiService.post<IGymAdminResponse>('/v1/gym-admins', data);

      if (response.success && response.data) {
        toast.success('Администратор спортзала успешно создан!');
        setIsModalOpen(false);
        reset();
        await fetchAll();
      } else {
        toast.error(response.error?.detail ?? 'Ошибка при создании администратора спортзала');
      }
    } catch (err) {
      console.error('Create gym admin error:', err);
      toast.error('Неизвестная ошибка при создании');
    } finally {
      setFormLoading(false);
    }
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setPage(page);
    if (pageSize) setPageSize(pageSize);
  };

  const onSubmit = async (data: CreateGymAdminRequest) => {
    const isValid = await trigger();
    if (isValid) {
      await handleCreate(data);
    }
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
    reset();
  };

  const columns = [
    {
      title: 'Фамилия',
      dataIndex: ['user', 'surname'],
      key: 'surname',
      render: (_: unknown, record: IGymAdminResponse) => record.user.surname,
    },
    {
      title: 'Имя',
      dataIndex: ['user', 'name'],
      key: 'name',
      render: (_: unknown, record: IGymAdminResponse) => record.user.name,
    },
    {
      title: 'Email',
      dataIndex: ['user', 'email'],
      key: 'email',
      ellipsis: true,
      render: (_: unknown, record: IGymAdminResponse) => record.user.email,
    },
    {
      title: 'Спортзалы',
      key: 'gyms',
      render: (_: unknown, record: IGymAdminResponse) =>
        record.gyms.length > 0 ? (
          <Space size={[0, 4]} wrap>
            {record.gyms.map((gym) => (
              <span
                key={gym.id}
                style={{ background: '#e6f7ff', padding: '2px 8px', borderRadius: 4, border: '1px solid #91d5ff' }}
              >
                {gym.name}
              </span>
            ))}
          </Space>
        ) : (
          <span style={{ color: '#ff4d4f' }}>—</span>
        ),
    },
    {
      title: 'Роли',
      key: 'roles',
      render: (_: unknown, record: IGymAdminResponse) => (
        <Space size={[0, 4]} wrap>
          {record.user.roleNames.map((role) => (
            <span key={role} style={{ background: '#f0f2f5', padding: '2px 8px', borderRadius: 4 }}>
              {roleMapping[role] || role}
            </span>
          ))}
        </Space>
      ),
    },
    {
      title: 'Статус',
      key: 'isActive',
      render: (_: unknown, record: IGymAdminResponse) => (
        <span style={{ color: record.user.isActive ? 'green' : 'red' }}>
          {record.user.isActive ? 'Активен' : 'Неактивен'}
        </span>
      ),
    },
    {
      title: 'Дата активации',
      key: 'startActiveAt',
      render: (_: unknown, record: IGymAdminResponse) => {
        if (!record.user.startActiveAt) return <span style={{ color: 'gray' }}>—</span>;

        const localDate = new Date(record.user.startActiveAt).toLocaleString();
        return <span>{localDate}</span>;
      },
    }
  ];

  const handleRightClick = (event: React.MouseEvent, record: IGymAdminResponse) => {
    event.preventDefault();
    setContextMenu({
      visible: true,
      x: event.clientX,
      y: event.clientY,
      record,
    });
  };

  const handleClickOutside = () => {
    if (contextMenu.visible) {
      setContextMenu({ ...contextMenu, visible: false });
    }
  };

  const setStatus = async (record: IGymAdminResponse, status : boolean) => {
      const response = await apiService.put<never>(`v1/gym-admins/${record.id}/set-status?status=${status}`);
      if(response.success) {
        const actionName = status ? "активировали" : "деактивировали";
        toast.info(`Успешно ${actionName} пользователя ${record.user.email}`);
        fetchAll();
      } else {
        toast.error(response.error?.detail ?? "Ошибка");
      }
  }

  const menu = (record?: IGymAdminResponse) => {
    // Если пользователь никогда не был активирован, меню не показываем
    if (!record?.user.startActiveAt) return null;

    return (
      <Menu>
        {record?.user.isActive ? (
          <Menu.Item
            key="deactivate"
            onClick={() => setStatus(record, false)}
          >
            Деактивировать
          </Menu.Item>
        ) : (
          <Menu.Item
            key="activate"
            onClick={() => setStatus(record, true)}
          >
            Активировать
          </Menu.Item>
        )}
      </Menu>
    );
  };


  return (
    <div style={{ padding: '16px' }} onClick={handleClickOutside}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2>Администраторы спортзалов</h2>
        <Button type="primary" onClick={() => setIsModalOpen(true)}>
          + Добавить администратора
        </Button>
      </div>

      <Table
        dataSource={items}
        columns={columns}
        loading={loading}
        pagination={false}
        scroll={{ x: true }}
        rowKey={(record) => record.user.email || record.id}
        onRow={(record) => ({
          onContextMenu: (event) => handleRightClick(event, record),
        })}
      />

      <div style={{ marginTop: 16, textAlign: 'right' }}>
        <Pagination
          current={page}
          pageSize={pageSize}
          total={total}
          onChange={handlePageChange}
          showSizeChanger
          showQuickJumper
          showTotal={(total) => `Всего: ${total}`}
        />
      </div>

      {contextMenu.visible && (
        <div
          ref={menuRef}
          style={{
            position: 'fixed',
            top: contextMenu.y,
            left: contextMenu.x,
            zIndex: 1000,
            boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            borderRadius: 6,
            background: '#fff',
            transform: 'scale(0.8)',
            opacity: 0,
            transition: 'transform 0.25s ease-out, opacity 0.15s ease-out',
            animation: 'menuFadeIn 0.25s forwards',
          }}
        >
          {menu(contextMenu.record)}
        </div>
      )}


      {/* Модальное окно создания */}
      <Modal
        title="Создать администратора спортзала"
        open={isModalOpen}
        onCancel={handleModalClose}
        footer={null}
        destroyOnClose
        width={600}
      >
        <form onSubmit={handleSubmit(onSubmit)}>
          <div style={{ marginBottom: 16 }}>
            <label style={{ display: 'block', marginBottom: 4, fontWeight: 500 }}>
              Email <span style={{ color: 'red' }}>*</span>
            </label>
            <Controller
              name="email"
              control={control}
              rules={{
                required: 'Email обязателен',
                pattern: {
                  value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                  message: 'Некорректный email',
                },
              }}
              render={({ field }) => (
                <Input
                  {...field}
                  status={errors.email ? 'error' : ''}
                  placeholder="Введите email"
                />
              )}
            />
            {errors.email && (
              <div style={{ color: '#ff4d4f', fontSize: 12, marginTop: 4 }}>
                {errors.email.message}
              </div>
            )}
          </div>

          <div style={{ marginBottom: 16 }}>
            <label style={{ display: 'block', marginBottom: 4, fontWeight: 500 }}>
              Фамилия <span style={{ color: 'red' }}>*</span>
            </label>
            <Controller
              name="surname"
              control={control}
              rules={{ required: 'Фамилия обязательна' }}
              render={({ field }) => (
                <Input
                  {...field}
                  status={errors.surname ? 'error' : ''}
                  placeholder="Введите фамилию"
                />
              )}
            />
            {errors.surname && (
              <div style={{ color: '#ff4d4f', fontSize: 12, marginTop: 4 }}>
                {errors.surname.message}
              </div>
            )}
          </div>

          <div style={{ marginBottom: 16 }}>
            <label style={{ display: 'block', marginBottom: 4, fontWeight: 500 }}>
              Имя <span style={{ color: 'red' }}>*</span>
            </label>
            <Controller
              name="name"
              control={control}
              rules={{ required: 'Имя обязательно' }}
              render={({ field }) => (
                <Input
                  {...field}
                  status={errors.name ? 'error' : ''}
                  placeholder="Введите имя"
                />
              )}
            />
            {errors.name && (
              <div style={{ color: '#ff4d4f', fontSize: 12, marginTop: 4 }}>
                {errors.name.message}
              </div>
            )}
          </div>

          <div style={{ marginBottom: 24 }}>
            <label style={{ display: 'block', marginBottom: 4, fontWeight: 500 }}>
              Спортзал <span style={{ color: 'red' }}>*</span>
            </label>
            <Controller
              name="gymId"
              control={control}
              rules={{ required: 'Выберите спортзал' }}
              render={({ field }) => (
                <Select
                  {...field}
                  status={errors.gymId ? 'error' : ''}
                  placeholder="Выберите спортзал"
                  options={gymOptions}
                  loading={gymOptions.length === 0 && isModalOpen}
                  allowClear
                  style={{ width: '100%' }}
                  onChange={(value) => setValue('gymId', value || '')}
                />
              )}
            />
            {errors.gymId && (
              <div style={{ color: '#ff4d4f', fontSize: 12, marginTop: 4 }}>
                {errors.gymId.message}
              </div>
            )}
          </div>

          <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end' }}>
            <Button 
              type="primary" 
              htmlType="submit" 
              loading={formLoading}
            >
              Создать
            </Button>
            <Button 
              onClick={handleModalClose}
              disabled={formLoading}
            >
              Отмена
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};