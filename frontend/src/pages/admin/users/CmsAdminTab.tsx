import React, { useState, useEffect, useRef } from 'react';
import { UserPageTabType } from "./userPageTabType";
import { CreateCmsAdminRequest, roleMapping, UserResponse } from "../../../types/auth";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { toast } from "react-toastify";
import { Table, Pagination, Button, Modal, Form, Input, Space, Menu } from 'antd';
import { useForm, Controller } from 'react-hook-form';
import './styles/cmsAdminTab.css'

interface CmsAdminTabProps {
  activeTab: UserPageTabType;
}

export const CmsAdminTab: React.FC<CmsAdminTabProps> = ({ activeTab }) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [items, setItems] = useState<UserResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const apiService = useApiService();
  const [contextMenu, setContextMenu] = useState<{ visible: boolean; x: number; y: number; record?: UserResponse }>({ visible: false, x: 0, y: 0 });
  const menuRef = useRef<HTMLDivElement>(null);

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateCmsAdminRequest>({
    defaultValues: {
      email: '',
      surname: '',
      name: '',
    },
  });

  useEffect(() => {
    if(activeTab == UserPageTabType.CmsAdmins) {
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
      const response = await apiService.get<ListResponse<UserResponse>>(`/v1/cms-admins?PageNumber=${page}&PageSize=${pageSize}`);

      if (response.success && response.data) {
        setItems(response.data?.items ?? []);
        setTotal(response.data?.totalItems ?? 0);
      } else {
        toast.error(response.error?.detail ?? 'Не удалось загрузить список администраторов');
        setItems([]);
        setTotal(0);
      }
    } catch (err) {
      console.error('Fetch error:', err);
      toast.error('Ошибка сети');
      setItems([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateCmsAdminRequest) => {
    try {
      const response = await apiService.post<UserResponse>('/v1/cms-admins', data);

      if (response.success && response.data) {
        toast.success('Администратор успешно создан!');
        setIsModalOpen(false);
        reset(); 
        await fetchAll(); 
      } else {
        toast.error(response.error?.detail ?? 'Ошибка при создании администратора');
      }
    } catch (err) {
      console.error('Create error:', err);
      toast.error('Неизвестная ошибка при создании');
    }
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setPage(page);
    if (pageSize) setPageSize(pageSize);
  };

  const columns = [
    {
      title: 'Фамилия',
      dataIndex: 'surname',
      key: 'surname',
    },
    {
      title: 'Имя',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
      ellipsis: true,
    },
    {
      title: 'Роли',
      key: 'roles',
      render: (_: unknown, record: UserResponse) => (
        <Space size={[0, 4]} wrap>
          {record.roleNames.map((role) => (
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
      render: (_: unknown, record: UserResponse) => (
        <span style={{ color: record.isActive ? 'green' : 'red' }}>
          {record.isActive ? 'Активен' : 'Неактивен'}
        </span>
      ),
    },
    {
      title: 'Дата активации',
      key: 'startActiveAt',
      render: (_: unknown, record: UserResponse) => {
        if (!record.startActiveAt) return <span style={{ color: 'gray' }}>—</span>;

        const localDate = new Date(record.startActiveAt).toLocaleString();
        return <span>{localDate}</span>;
      },
    }
  ];

  const setStatus = async (record: UserResponse, status : boolean) => {
    const response = await apiService.put<never>(`v1/cms-admins/${record.id}/set-status?status=${status}`);
    if(response.success) {
      const actionName = status ? "активировали" : "деактивировали";
      toast.info(`Успешно ${actionName} пользователя ${record.email}`);
      fetchAll();
    } else {
      toast.error(response.error?.detail ?? "Ошибка");
    }
  }


  const menu = (record?: UserResponse) => {
    // Если пользователь никогда не был активирован, меню не показываем
    if (!record?.startActiveAt) return null;

    return (
      <Menu>
        {record.isActive ? (
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

  const handleRightClick = (event: React.MouseEvent, record: UserResponse) => {
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

  return (
    <div style={{ padding: '16px' }} onClick={handleClickOutside}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2>Администраторы CMS</h2>
        <Button type="primary" onClick={() => setIsModalOpen(true)}>
          + Добавить администратора
        </Button>
      </div>

      <Table
        dataSource={items}
        columns={columns}
        rowKey="email"
        loading={loading}
        pagination={false}
        scroll={{ x: true }}
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
        title="Создать администратора CMS"
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          reset();
        }}
        footer={null}
        destroyOnClose
      >
        <Form layout="vertical" onFinish={handleSubmit(handleCreate)}>
          <Form.Item
            label="Email"
            required
            validateStatus={errors.email ? 'error' : ''}
            help={errors.email?.message}
          >
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
              render={({ field }) => <Input {...field} placeholder="user@example.com" />}
            />
          </Form.Item>

          <Form.Item
            label="Фамилия"
            required
            validateStatus={errors.surname ? 'error' : ''}
            help={errors.surname?.message}
          >
            <Controller
              name="surname"
              control={control}
              rules={{ required: 'Фамилия обязательна' }}
              render={({ field }) => <Input {...field} placeholder="Иванов" />}
            />
          </Form.Item>

          <Form.Item
            label="Имя"
            required
            validateStatus={errors.name ? 'error' : ''}
            help={errors.name?.message}
          >
            <Controller
              name="name"
              control={control}
              rules={{ required: 'Имя обязательно' }}
              render={({ field }) => <Input {...field} placeholder="Иван" />}
            />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit">
                Создать
              </Button>
              <Button
                onClick={() => {
                  setIsModalOpen(false);
                  reset();
                }}
              >
                Отмена
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
