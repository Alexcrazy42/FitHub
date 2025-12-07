import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { ThemeProvider } from './context/ThemeProvider';
import 'antd/dist/reset.css';
import './index.css';
import { ToastContainer } from 'react-toastify';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
      <ThemeProvider>
          <App />
          <ToastContainer position="top-right" autoClose={2000} />
      </ThemeProvider>
  </React.StrictMode>
);