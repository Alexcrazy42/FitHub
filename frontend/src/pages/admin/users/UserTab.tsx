import { UserPageTabType } from "./userPageTabType";
import React, { useState, useEffect, useRef } from 'react';
import {
  roleMapping,
} from '../../../types/auth';
import { useApiService } from '../../../api/useApiService';
import { ListResponse } from '../../../types/common';
import { toast } from 'react-toastify';
import { Table, Pagination, Space, Menu } from 'antd';
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
  const [contextMenu, setContextMenu] = useState<{ visible: boolean; x: number; y: number; record?: IVisitorResponse }>({ visible: false, x: 0, y: 0 });
      const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (activeTab === UserPageTabType.Users) {
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

  const setStatus = async (record: IVisitorResponse, status : boolean) => {
      const response = await apiService.put<never>(`v1/visitors/${record.id}/set-status?status=${status}`);
      if(response.success) {
        const actionName = status ? "активировали" : "деактивировали";
        toast.info(`Успешно ${actionName} пользователя ${record.user.email}`);
        fetchAll();
      } else {
        toast.error(response.error?.detail ?? "Ошибка");
      }
    }

  const handlePageChange = (page: number, pageSize?: number) => {
    setPage(page);
    if (pageSize) setPageSize(pageSize);
  };

  const menu = (record?: IVisitorResponse) => {
    // Если пользователь никогда не был активирован, меню не показываем
    if (!record?.user.startActiveAt) return null;

    return (
      <Menu>
        {record.user.isActive ? (
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

  const handleRightClick = (event: React.MouseEvent, record: IVisitorResponse) => {
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
    <div style={{ padding: '16px' }} onClick={handleClickOutside}>
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
    </div>
  );
};