import { RouterProvider } from 'react-router-dom';
import { router } from './routes/router';
import { useTheme } from './context/useTheme';
import { ConfigProvider, theme as antdTheme } from 'antd';
import { AuthProvider } from './context/AuthProvider';

const ThemedApp = () => {
  const { theme } = useTheme();

  return (
    <ConfigProvider
      theme={{
        algorithm:
          theme === 'dark'
            ? antdTheme.darkAlgorithm
            : antdTheme.defaultAlgorithm,
      }}
    >
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </ConfigProvider>
  );
};

function App() {
  return <ThemedApp />;
}

export default App;