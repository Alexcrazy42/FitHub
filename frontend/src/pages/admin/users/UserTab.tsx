import { UserPageTabType } from "./userPageTabType";
import React, { useState, useEffect } from 'react';
import {
  roleMapping,
} from '../../../types/auth';
import { useApiService } from '../../../api/useApiService';
import { ListResponse } from '../../../types/common';
import { toast } from 'react-toastify';
import { Table, Pagination, Button, Modal, Form, Input, Space } from 'antd';
import { IVisitorResponse } from "../../../types/users";



interface UserTabProps {
  activeTab: UserPageTabType;
}

export const UserTab: React.FC<UserTabProps> = ({ activeTab }) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [items, setItems] = useState<IVisitorResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const apiService = useApiService();

  useEffect(() => {
    if (activeTab === UserPageTabType.Users) {
      fetchAll();
    }
  }, [page, pageSize, activeTab]);

  const fetchAll = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<IVisitorResponse>>(
        `/v1/visitors?PageNumber=${page}&PageSize=${pageSize}`
      );

      if (response.success && response.data) {
        const userItems = response.data.items;
        setItems(userItems);
        setTotal(response.data.totalItems ?? userItems.length);
      } else {
        toast.error(response.error?.detail ?? 'Не удалось загрузить список посетителей');
        setItems([]);
        setTotal(0);
      }
    } catch (err) {
      console.error('Fetch trainers error:', err);
      toast.error('Ошибка сети при загрузке посетителей');
      setItems([]);
      setTotal(0);
    } finally {
      setLoading(false);
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
      render: (_: unknown, record: IVisitorResponse) => record.user.surname,
    },
    {
      title: 'Имя',
      dataIndex: 'name',
      key: 'name',
      render: (_: unknown, record: IVisitorResponse) => record.user.name,
    },
    {
      title: 'Email',
      dataIndex: 'email',
      key: 'email',
      ellipsis: true,
      render: (_: unknown, record: IVisitorResponse) => record.user.email,
    },
    {
      title: 'Роли',
      key: 'roles',
      render: (_: unknown, record: IVisitorResponse) => (
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
      render: (_: unknown, record: IVisitorResponse) => (
        <span style={{ color: record.user.isActive ? 'green' : 'red' }}>
          {record.user.isActive ? 'Активен' : 'Неактивен'}
        </span>
      ),
    },
    {
      title: 'Дата активации',
      key: 'startActiveAt',
      render: (_: unknown, record: IVisitorResponse) => {
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
    </div>
  );
};