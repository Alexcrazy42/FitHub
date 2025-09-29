// App.tsx
import React, { FC, ReactNode } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from "react-router-dom";
import { Layout, Menu } from "antd";
import GymsPage from "./pages/gyms/GymPage";

const { Sider, Content } = Layout;

// --- Заглушки страниц ---

const ClassTemplatesPage: FC = () => <div>📋 Базовые групповые тренировки</div>;
const MuscleGroupsPage: FC = () => <div>💪 Группы мышц</div>;
const MachinesPage: FC = () => <div>🏋️‍♂️ Тренажёры</div>;
const VideoTrainingsPage: FC = () => <div>🎥 Видеотренировки</div>;

// --- Layout для CmsAdmin ---
interface CmsAdminLayoutProps {
  children: ReactNode;
}

const CmsAdminLayout: FC<CmsAdminLayoutProps> = ({ children }) => (
  <Layout style={{ minHeight: "100vh" }}>
    <Sider width={220} theme="dark">
      <div
        style={{
          color: "white",
          fontWeight: "bold",
          padding: "16px",
          textAlign: "center",
        }}
      >
        CmsAdmin
      </div>
      <Menu theme="dark" mode="inline" defaultSelectedKeys={["gyms"]}>
        <Menu.Item key="gyms">
          <Link to="/gyms">Спортзалы</Link>
        </Menu.Item>
        <Menu.Item key="class-templates">
          <Link to="/class-templates">Базовые групповые тренировки</Link>
        </Menu.Item>
        <Menu.Item key="muscle-groups">
          <Link to="/muscle-groups">Группы мышц</Link>
        </Menu.Item>
        <Menu.Item key="machines">
          <Link to="/machines">Тренажёры</Link>
        </Menu.Item>
        <Menu.Item key="video-trainings">
          <Link to="/video-trainings">Видеотренировки</Link>
        </Menu.Item>
      </Menu>
    </Sider>
    <Layout style={{ background: "#f0f2f5" }}>
      <Content
        style={{
          margin: "16px",
          padding: "16px",
          background: "#fff",
          width: "100%",
          minHeight: "calc(100vh - 32px)",
        }}
      >
        {children}
      </Content>
    </Layout>
  </Layout>
);

// --- Защита маршрута ---
interface ProtectedRouteProps {
  children: ReactNode;
  allowedType: "CmsAdmin" | "Trainer" | "Member";
}

const ProtectedRoute: FC<ProtectedRouteProps> = ({ children, allowedType }) => {
  const currentUserType: "CmsAdmin" | "Trainer" | "Member" = "CmsAdmin"; // заглушка

  if (currentUserType !== allowedType) {
    return <div style={{ padding: 20 }}>Доступ запрещён</div>;
  }

  return children
};

// --- App ---
const App: FC = () => (
  <Router>
    <Routes>
      <Route
        path="/"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <Navigate to="/gyms" replace />
          </ProtectedRoute>
        }
      />
      <Route
        path="/gyms"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <CmsAdminLayout>
              <GymsPage />
            </CmsAdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/class-templates"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <CmsAdminLayout>
              <ClassTemplatesPage />
            </CmsAdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/muscle-groups"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <CmsAdminLayout>
              <MuscleGroupsPage />
            </CmsAdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/machines"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <CmsAdminLayout>
              <MachinesPage />
            </CmsAdminLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/video-trainings"
        element={
          <ProtectedRoute allowedType="CmsAdmin">
            <CmsAdminLayout>
              <VideoTrainingsPage />
            </CmsAdminLayout>
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<div style={{ padding: 20 }}>Страница не найдена</div>} />
    </Routes>
  </Router>
);

export default App;
