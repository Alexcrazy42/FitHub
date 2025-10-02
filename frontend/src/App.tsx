// src/App.tsx
import { RouterProvider } from 'react-router-dom';
import { router } from './routes';
import { useTheme } from './context/useTheme';
import { ConfigProvider, theme as antdTheme } from 'antd';

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
      <RouterProvider router={router} />
    </ConfigProvider>
  );
};

function App() {
  return <ThemedApp />;
}

export default App;