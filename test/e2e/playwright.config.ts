import { defineConfig, devices } from '@playwright/test';
import dotenv from 'dotenv';
import path from 'path';

// dotenv.config({ path: '.env.test' });

export default defineConfig({
  // Тесты ищет в папке specs
  testDir: './tests',
  
  // Таймауты (жесткие)
  timeout: 60000,
  expect: { timeout: 10000 },
  
  // Параллелизация
  fullyParallel: true,
  workers: 1, //process.env.CI ? 4 : undefined,
  //retries: process.env.CI ? 2 : 0,
  
  // Репортеры
  reporter: [
    ['html', { outputFolder: 'reports/html' }],
    ['list'],
    ['json', { outputFile: 'reports/results.json' }]
  ],
  
  // Глобальные хуки
  // globalSetup: path.join(__dirname, 'hooks/globalSetup.ts'),
  // globalTeardown: path.join(__dirname, 'hooks/globalTeardown.ts'),
  
  use: {
    // Базовый URL (берем из окружения)
    baseURL: process.env.BASE_URL || 'http://localhost:3000',
    
    // Артефакты
    headless: false,
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    
    // Логи
    actionTimeout: 15000,
    navigationTimeout: 30000,
    
    // Дополнительные опции
    viewport: { width: 1280, height: 720 },
    ignoreHTTPSErrors: true,
    
    // Переменные для тестов
    testIdAttribute: 'data-testid'
  },

  // Конфигурация для разных браузеров
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    }
    // {
    //   name: 'firefox',
    //   use: { ...devices['Desktop Firefox'] },
    // },
    
    // Для дымовых тестов можно оставить только chromium (ускорение CI)
  ],
});