import { RouterProvider } from 'react-router-dom';
import { router } from './routes/router';
import { useTheme } from './context/useTheme';
import { ConfigProvider, theme as antdTheme } from 'antd';
import { AuthProvider } from './context/AuthProvider';
import { store } from './store/store'
import { Provider } from 'react-redux';
import { WebSocketProvider } from './WebSocketProvider';

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
      <Provider store={store}>
        <WebSocketProvider>
          <AuthProvider>
            <RouterProvider router={router} />
          </AuthProvider>
        </WebSocketProvider>
      </Provider>
    </ConfigProvider>
  );
};

function App() {
  return <ThemedApp />;
}

export default App;