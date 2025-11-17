import { Layout, Button } from 'antd';
import { MenuUnfoldOutlined, MenuFoldOutlined, SunOutlined, MoonOutlined, LogoutOutlined } from '@ant-design/icons';
import { useState } from 'react';
import Sidebar from '../components/Sidebar';
import { Outlet, useNavigate } from 'react-router-dom';
import { useTheme } from '../context/useTheme';
import { useApiService } from '../api/useApiService';

const { Sider, Content, Header } = Layout;

const DashboardLayout: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false);
  const { theme, toggleTheme } = useTheme();
  const apiService = useApiService();
  const navigate = useNavigate();

  const logout = async () => {
    const response = await apiService.post<never>("/v1/auth/logout");
    if (response.success) {
      navigate('/login')
    } else {
      //toast.error(response.error?.detail);
    }
  }

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={(value) => setCollapsed(value)}
        width={250}
        theme={theme === 'dark' ? 'dark' : 'light'}
      >
        <div
          className={`flex items-center justify-center h-16 font-bold transition-all duration-200 ${
            theme === 'dark' ? 'text-white' : 'text-gray-800'
          }`}
        >
          {!collapsed && 'FitHub'}
        </div>
        <Sidebar collapsed={collapsed} />
      </Sider>

      <Layout>
        <Header
          style={{
            height: 64,
            backgroundColor: theme === "dark" ? "#1f2937" : "#ffffff",
            color: theme === "dark" ? "#ffffff" : "#1f2937",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            padding: "0 1rem",
            boxShadow: "0 1px 2px 0 rgba(0, 0, 0, 0.05)",
          }}
        >
          {/* Левая кнопка */}
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={() => setCollapsed(!collapsed)}
            style={{
              color: theme === "dark" ? "#ffffff" : "#374151",
              fontSize: 18,
            }}
            className={theme === "dark" ? "hover:!bg-gray-800" : "hover:!bg-gray-100"}
          />

          {/* Правая зона */}
          <div style={{ display: "flex", alignItems: "center", gap: "0.5rem" }}>
            {/* Смена темы */}
            <Button
              type="text"
              icon={theme === "dark" ? <SunOutlined /> : <MoonOutlined />}
              onClick={toggleTheme}
              style={{
                color: theme === "dark" ? "#fbbf24" : "#374151",
                fontSize: 18,
              }}
              className={theme === "dark" ? "hover:!bg-gray-800" : "hover:!bg-gray-100"}
            />

            <Button
              type="text"
              style={{
                display: "flex",
                alignItems: "center",
                gap: "0.4rem",
                padding: "0 0.75rem",
                color: theme === "dark" ? "#ef4444" : "#b91c1c", // красный оттенок
                fontWeight: 500,
              }}
              className={theme === "dark" ? "hover:!bg-gray-800" : "hover:!bg-gray-100"}
              icon={<LogoutOutlined />}
              onClick={logout}
            >
              Выйти
            </Button>
          </div>
        </Header>

        <Content className={`p-6 ${theme === 'dark' ? 'bg-gray-900' : 'bg-gray-50'}`}>
          <div className="max-w-7xl mx-auto">
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default DashboardLayout;