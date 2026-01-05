import React, { useState, useEffect } from 'react';
import { Modal, Form, Input, Select, Avatar, Typography, Tag, Spin } from 'antd';
import { TeamOutlined } from '@ant-design/icons';
import { ChatType, ICreateChatRequest } from '../../../../types/messaging';
import { useApiService } from '../../../../api/useApiService';
import { useUserService } from '../../../../api/services/userService';
import { UserResponse } from '../../../../types/auth';
import { useAuth } from '../../../../context/useAuth';

const { Option } = Select;
const { Text } = Typography;

interface Props {
  open: boolean;
  onCancel: () => void;
  onSubmit: (values: ICreateChatRequest) => Promise<void>;
  loading: boolean;
}

function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(timer);
    };
  }, [value, delay]);

  return debouncedValue;
}

const CreateChatModal: React.FC<Props> = ({ open, onCancel, onSubmit, loading }) => {
  const apiService = useApiService();
  const userService = useUserService(apiService);
  const { user } = useAuth();
  
  const [form] = Form.useForm();
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedUsers, setSelectedUsers] = useState<string[]>([]);
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  
  const debouncedSearchQuery = useDebounce(searchQuery, 300);

  useEffect(() => {
    if (open) {
      form.resetFields();
      setSelectedUsers([]);
      setSearchQuery('');
      setUsers([]);
      setPage(1);
      setHasMore(true);
    }
  }, [open, form]);

  // Загрузка пользователей при изменении debounced query
  useEffect(() => {
    if (open) {
      loadUsers(true); // true = reset список
    }
  }, [debouncedSearchQuery, open]);

  const loadUsers = async (reset: boolean = false) => {
    if (loadingUsers) return;
    
    setLoadingUsers(true);
    
    try {
      const currentPage = reset ? 1 : page;
      
      const response = await userService.getUsers(
        { partName: debouncedSearchQuery },
        { PageNumber: currentPage, PageSize: 20 }
      );

      if (response.success && response.data) {
        const filteredUsers = response.data.items.filter(u => u.id !== user?.id);
        
        if (reset) {
          setUsers(filteredUsers);
          setPage(2);
        } else {
          setUsers(prev => [...prev, ...filteredUsers]);
          setPage(prev => prev + 1);
        }
        
        setHasMore((response.data.totalPages ?? 0) > page);
      }
    } catch (err) {
      console.error('Failed to load users:', err);
    } finally {
      setLoadingUsers(false);
    }
  };

  const handleFinish = async (values: any) => {
    const participantIds = user?.id 
      ? [...values.participantIds, user.id] 
      : values.participantIds;
    
    const request: ICreateChatRequest = {
      type: ChatType.Group,
      participantUserIds: participantIds,
    };

    await onSubmit(request);
  };

  // Обработка скролла для подгрузки
  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    const target = e.target as HTMLDivElement;
    const bottom = target.scrollHeight - target.scrollTop === target.clientHeight;
    
    if (bottom && hasMore && !loadingUsers) {
      loadUsers(false);
    }
  };

  return (
    <Modal
      title={
        <div className="flex items-center gap-2">
          <TeamOutlined className="text-blue-500" />
          <span>Создать групповой чат</span>
        </div>
      }
      open={open}
      onCancel={onCancel}
      onOk={() => form.submit()}
      confirmLoading={loading}
      okText="Создать"
      cancelText="Отмена"
      width={600}
      destroyOnClose
    >
      <Form
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        className="mt-4"
      >

        {/* Выбор участников */}
        <Form.Item
          name="participantIds"
          label="Добавьте участников"
          rules={[
            { required: true, message: 'Выберите участников' },
            {
              validator: (_, value) => {
                if (value?.length < 1) {
                  return Promise.reject('Выберите минимум одного участника');
                }
                return Promise.resolve();
              }
            }
          ]}
        >
          <Select
            mode="multiple"
            placeholder="Начните вводить имя или email..."
            size="large"
            showSearch
            searchValue={searchQuery}
            onSearch={setSearchQuery}
            filterOption={false}
            maxTagCount="responsive"
            onChange={setSelectedUsers}
            optionLabelProp="label"
            notFoundContent={loadingUsers ? <Spin size="small" /> : 'Пользователи не найдены'}
            onPopupScroll={handleScroll}
            loading={loadingUsers}
          >
            {users.map((user) => (
              <Option
                key={user.id}
                value={user.id}
                label={`${user.name} ${user.surname}`}
              >
                <div className="flex items-center gap-3 py-1">
                  <Avatar size="small" className="bg-blue-500">
                    {user.name[0]}
                  </Avatar>
                  <div className="flex-1">
                    <div className="font-medium">
                      {user.name} {user.surname}
                    </div>
                    <div className="text-xs text-gray-500">{user.email}</div>
                  </div>
                </div>
              </Option>
            ))}
            
            {/* Индикатор загрузки при подгрузке */}
            {loadingUsers && users.length > 0 && (
              <Option disabled key="loading">
                <div className="text-center py-2">
                  <Spin size="small" />
                </div>
              </Option>
            )}
          </Select>
        </Form.Item>

        {/* Информация о выбранных участниках */}
        {selectedUsers.length > 0 && (
          <div className="bg-gray-50 p-3 rounded-lg">
            <Text type="secondary" className="text-xs">
              Выбрано участников: {selectedUsers.length}
              {user && <span className="ml-1">(+ вы)</span>}
            </Text>
            <div className="flex flex-wrap gap-2 mt-2">
              {/* Текущий пользователь */}
              {user && (
                <Tag color="green">
                  {user.name} {user.surname} (вы)
                </Tag>
              )}
              
              {/* Выбранные пользователи */}
              {selectedUsers.map((userId) => {
                const selectedUser = users.find((u) => u.id === userId);
                return selectedUser ? (
                  <Tag key={userId} color="blue">
                    {selectedUser.name} {selectedUser.surname}
                  </Tag>
                ) : null;
              })}
            </div>
          </div>
        )}
      </Form>
    </Modal>
  );
};

export default CreateChatModal;
