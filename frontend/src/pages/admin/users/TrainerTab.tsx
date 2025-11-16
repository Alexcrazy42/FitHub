import React, { useState, useEffect } from 'react';
import { UserPageTabType } from './userPageTabType';
import {
  CreateTrainerAdminRequest,
  ITrainerResponse,
  roleMapping,
} from '../../../types/auth';
import { useApiService } from '../../../api/useApiService';
import { ListResponse } from '../../../types/common';
import { toast } from 'react-toastify';
import { Table, Pagination, Button, Modal, Form, Input, Space } from 'antd';
import { useForm, Controller } from 'react-hook-form';

interface TrainerTabProps {
  activeTab: UserPageTabType;
}

export const TrainerTab: React.FC<TrainerTabProps> = ({ activeTab }) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [items, setItems] = useState<ITrainerResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const apiService = useApiService();

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateTrainerAdminRequest>({
    defaultValues: {
      email: '',
      surname: '',
      name: '',
    },
  });

  // Загрузка тренеров при активации вкладки
  useEffect(() => {
    if (activeTab === UserPageTabType.Trainers) {
      fetchAll();
    }
  }, [page, pageSize, activeTab]);

  const fetchAll = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<ITrainerResponse>>(
        `/v1/trainers?PageNumber=${page}&PageSize=${pageSize}`
      );

      if (response.success && response.data) {
        const userItems = response.data.items;
        setItems(userItems);
        setTotal(response.data.totalItems ?? userItems.length);
      } else {
        toast.error(response.error?.detail ?? 'Не удалось загрузить список тренеров');
        setItems([]);
        setTotal(0);
      }
    } catch (err) {
      console.error('Fetch trainers error:', err);
      toast.error('Ошибка сети при загрузке тренеров');
      setItems([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateTrainerAdminRequest) => {
    try {
      const response = await apiService.post<ITrainerResponse>('/v1/trainers', data);

      if (response.success && response.data) {
        toast.success('Тренер успешно создан!');
        setIsModalOpen(false);
        reset();
        await fetchAll();
      } else {
        toast.error(response.error?.detail ?? 'Ошибка при создании тренера');
      }
    } catch (err) {
      console.error('Create trainer error:', err);
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
      render: (_: unknown, record: ITrainerResponse) => record.user.surname,
    },
    {
      title: 'Имя',
      dataIndex: 'name',
      key: 'name',
      render: (_: unknown, record: ITrainerResponse) => record.user.name,
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
      ellipsis: true,
      render: (_: unknown, record: ITrainerResponse) => record.user.email,
    },
    {
      title: 'Роли',
      key: 'roles',
      render: (_: unknown, record: ITrainerResponse) => (
        <Space size={[0, 4]} wrap>
          {record.user.roleNames.map((role) => (
            <span
              key={role}
              style={{ background: '#f0f2f5', padding: '2px 8px', borderRadius: 4 }}
            >
              {roleMapping[role] || role}
            </span>
          ))}
        </Space>
      ),
    },
    {
      title: 'Статус',
      key: 'isActive',
      render: (_: unknown, record: ITrainerResponse) => (
        <span style={{ color: record.user.isActive ? 'green' : 'red' }}>
          {record.user.isActive ? 'Активен' : 'Неактивен'}
        </span>
      ),
    },
    {
      title: 'Дата активации',
      key: 'startActiveAt',
      render: (_: unknown, record: ITrainerResponse) => {
        if (!record.user.startActiveAt) return <span style={{ color: 'gray' }}>—</span>;

        const localDate = new Date(record.user.startActiveAt).toLocaleString();
        return <span>{localDate}</span>;
      },
    }
  ];

  return (
    <div style={{ padding: '16px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <h2>Тренеры</h2>
        <Button type="primary" onClick={() => setIsModalOpen(true)}>
          + Добавить тренера
        </Button>
      </div>

      <Table
        dataSource={items.map((user, index) => ({
          ...user,
          key: user.user.email || index,
        }))}
        columns={columns}
        loading={loading}
        pagination={false}
        scroll={{ x: true }}
        rowKey={(record) => record.user.email || record.id}
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

      {/* Модальное окно создания */}
      <Modal
        title="Создать тренера"
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          reset();
        }}
        footer={null}
        destroyOnClose
        width={600}
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
              render={({ field }) => <Input {...field} placeholder="trainer@fit.ru" />}
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
              render={({ field }) => <Input {...field} placeholder="Сидоров" />}
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
              render={({ field }) => <Input {...field} placeholder="Дмитрий" />}
            />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" loading={loading}>
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