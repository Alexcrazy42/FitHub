import { useEffect, useState } from 'react';
import { Tabs, Avatar, Button, Input, Form, Spin, Tag } from 'antd';
import {
  UserOutlined,
  LockOutlined,
  InfoCircleOutlined,
  SaveOutlined,
  KeyOutlined,
} from '@ant-design/icons';
import { toast } from 'react-toastify';
import { UserResponse, roleMapping } from '../../types/auth';
import { useApiService } from '../../api/useApiService';
import { useUserService } from '../../api/services/userService';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';

const MyAccountPage: React.FC = () => {
  const apiService = useApiService();
  const userService = useUserService(apiService);

  const [profile, setProfile] = useState<UserResponse | null>(null);
  const [loading, setLoading] = useState(true);

  const [profileForm] = Form.useForm();
  const [passwordForm] = Form.useForm();
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingPassword, setSavingPassword] = useState(false);

  useEffect(() => {
    userService.getMe().then((resp) => {
      if (resp.success && resp.data) {
        setProfile(resp.data);
        profileForm.setFieldsValue({ name: resp.data.name, surname: resp.data.surname });
      }
    }).finally(() => setLoading(false));
  }, []);

  const handleSaveProfile = async (values: { name: string; surname: string }) => {
    setSavingProfile(true);
    try {
      const resp = await userService.updateProfile(values.name, values.surname);
      if (resp.success) {
        setProfile((p) => p ? { ...p, name: values.name, surname: values.surname } : p);
        toast.success('Профиль обновлён');
      } else {
        toast.error(resp.error?.detail ?? 'Ошибка при сохранении');
      }
    } finally {
      setSavingProfile(false);
    }
  };

  const handleChangePassword = async (values: {
    oldPassword: string;
    newPassword: string;
    confirmPassword: string;
  }) => {
    if (values.newPassword !== values.confirmPassword) {
      toast.error('Пароли не совпадают');
      return;
    }
    setSavingPassword(true);
    try {
      const resp = await userService.changePassword(values.oldPassword, values.newPassword);
      if (resp.success) {
        passwordForm.resetFields();
        toast.success('Пароль изменён');
      } else {
        toast.error(resp.error?.detail ?? 'Ошибка при смене пароля');
      }
    } finally {
      setSavingPassword(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (!profile) return null;

  const avatarSrc = `https://ui-avatars.com/api/?name=${profile.name[0]}${profile.surname[0]}&background=random&size=96`;

  const tabItems = [
    {
      key: 'profile',
      label: (
        <span className="flex items-center gap-1">
          <UserOutlined /> Профиль
        </span>
      ),
      children: (
        <div className="max-w-md">
          <div className="flex items-center gap-4 mb-6">
            <Avatar size={72} src={avatarSrc} icon={<UserOutlined />} />
            <div>
              <p className="text-lg font-semibold">{profile.surname} {profile.name}</p>
              <p className="text-sm text-gray-500">{profile.email}</p>
            </div>
          </div>

          <Form form={profileForm} layout="vertical" onFinish={handleSaveProfile}>
            <Form.Item
              label="Имя"
              name="name"
              rules={[{ required: true, message: 'Введите имя' }]}
            >
              <Input placeholder="Имя" />
            </Form.Item>
            <Form.Item
              label="Фамилия"
              name="surname"
              rules={[{ required: true, message: 'Введите фамилию' }]}
            >
              <Input placeholder="Фамилия" />
            </Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={savingProfile}
              icon={<SaveOutlined />}
            >
              Сохранить
            </Button>
          </Form>
        </div>
      ),
    },
    {
      key: 'security',
      label: (
        <span className="flex items-center gap-1">
          <LockOutlined /> Безопасность
        </span>
      ),
      children: (
        <div className="max-w-md">
          <h3 className="text-base font-semibold mb-4">Смена пароля</h3>
          <Form form={passwordForm} layout="vertical" onFinish={handleChangePassword}>
            <Form.Item
              label="Текущий пароль"
              name="oldPassword"
              rules={[{ required: true, message: 'Введите текущий пароль' }]}
            >
              <Input.Password placeholder="Текущий пароль" />
            </Form.Item>
            <Form.Item
              label="Новый пароль"
              name="newPassword"
              rules={[
                { required: true, message: 'Введите новый пароль' },
                { min: 6, message: 'Минимум 6 символов' },
              ]}
            >
              <Input.Password placeholder="Новый пароль" />
            </Form.Item>
            <Form.Item
              label="Подтвердите пароль"
              name="confirmPassword"
              rules={[{ required: true, message: 'Подтвердите пароль' }]}
            >
              <Input.Password placeholder="Повторите пароль" />
            </Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={savingPassword}
              icon={<KeyOutlined />}
            >
              Изменить пароль
            </Button>
          </Form>
        </div>
      ),
    },
    {
      key: 'info',
      label: (
        <span className="flex items-center gap-1">
          <InfoCircleOutlined /> Аккаунт
        </span>
      ),
      children: (
        <div className="max-w-md space-y-4">
          <Row label="Email" value={profile.email} />
          <Row
            label="Роли"
            value={
              <div className="flex flex-wrap gap-1">
                {profile.roleNames.map((r) => (
                  <Tag key={r} color="blue">
                    {roleMapping[r] ?? r}
                  </Tag>
                ))}
              </div>
            }
          />
          <Row
            label="Статус"
            value={
              <Tag color={profile.isActive ? 'green' : 'red'}>
                {profile.isActive ? 'Активен' : 'Неактивен'}
              </Tag>
            }
          />
          {profile.startActiveAt && (
            <Row
              label="Активен с"
              value={format(new Date(profile.startActiveAt), 'd MMMM yyyy', { locale: ru })}
            />
          )}
          <Row label="ID аккаунта" value={<span className="font-mono text-xs text-gray-500">{profile.id}</span>} />
        </div>
      ),
    },
  ];

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold mb-6">Настройки аккаунта</h1>
      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <Tabs items={tabItems} />
      </div>
    </div>
  );
};

const Row: React.FC<{ label: string; value: React.ReactNode }> = ({ label, value }) => (
  <div className="flex flex-col sm:flex-row sm:items-center gap-1">
    <span className="text-sm text-gray-500 w-32 flex-shrink-0">{label}</span>
    <span className="text-sm text-gray-800">{value}</span>
  </div>
);

export default MyAccountPage;
