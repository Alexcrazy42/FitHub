import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
// 1. **Fast Refresh** (преемник Hot Reloading)
// 2. **JSX трансформация** через Babel
// 3. **HMR для React-компонентов**
// - Vite обнаруживает изменение файла
// - Отправляет HMR-обновление в браузер
// - React перемонтирует только измененный компонент
// - **Состояние компонента сохраняется** (если возможно)

import tailwindcss from '@tailwindcss/vite'
// Благодаря: @tailwindcss/vite + Нативный CSS HMR
import path from 'path';

export default defineConfig({
  plugins: [
    react(),
    tailwindcss()
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  }
})
