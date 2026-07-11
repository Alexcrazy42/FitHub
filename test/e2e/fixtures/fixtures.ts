import { test as base, Browser, BrowserContext, Page } from '@playwright/test';
import { FitHubCreds } from '../config/testUsers';
import { App, APP_URLS, POST_LOGIN_URL, UserRole } from './sites';



const storageCache = new Map<string, any>();



async function createFitHubAuthenticatedContext(
  browser: Browser,
  userRole: UserRole,
  app: App
): Promise<BrowserContext> {
  const cacheKey = `${userRole}_${app}`;

  if (storageCache.has(cacheKey)) {
    console.log(`♻️ [Cache hit] Использую кэш для ${cacheKey}`);
    return await browser.newContext({ 
      storageState: storageCache.get(cacheKey) 
    });
  }

  console.log(`🐢 [Cache miss] Выполняю логин для ${cacheKey}`);
  
  const context = await browser.newContext();
  const page = await context.newPage();

  const baseUrl = APP_URLS[app];

  await page.goto(`${baseUrl}/login`);

  const creds = FitHubCreds[userRole];

  await page.getByPlaceholder('Введите ваш email').fill(creds.login);
  await page.getByPlaceholder('Введите ваш пароль').fill(creds.password);
  await page.getByRole('button', { name: 'Войти' }).click();

  const postLogin = POST_LOGIN_URL[userRole];

  await page.waitForURL(`${baseUrl}/${postLogin}`, { timeout: 10000 });

  const storageState = await context.storageState();
  storageCache.set(cacheKey, storageState);

  return context;
}

type AuthFixtures = {
  // Test-scoped фикстуры (страницы)
  cmsAdminPage: Page;
  gymAdminPage: Page;
};

type WorkerFixtures = {
  // Worker-scoped фикстуры (контексты)
  cmsAdminContext: BrowserContext;
  gymAdminContext: BrowserContext;
};

export const test = base.extend<AuthFixtures, WorkerFixtures>({
  cmsAdminContext: [
    async ({ browser }, use) => {
      console.log('🟢 СОЗДАЮ cmsAdminContext (worker-scoped) - ОДИН РАЗ');
      const context = await createFitHubAuthenticatedContext(
        browser,
        'cmsAdmin',
        'fithub'
      );
      await use(context);
      console.log('🔴 УНИЧТОЖАЮ cmsAdminContext (worker-scoped)');
      await context.close();
    },
    { scope: 'worker' }
  ],

  gymAdminContext: [
    async ({ browser }, use) => {
      console.log('🟢 СОЗДАЮ gymAdminContext (worker-scoped) - ОДИН РАЗ');
      const context = await createFitHubAuthenticatedContext(
        browser,
        'gymAdmin',
        'fithub'
      );
      await use(context);
      console.log('🔴 УНИЧТОЖАЮ gymAdminContext (worker-scoped)');
      await context.close();
    },
    { scope: 'worker' }
  ],

  // 📄 Test-scoped: страницы создаются из worker-scoped контекстов
  cmsAdminPage: async ({ cmsAdminContext }, use) => {
    console.log('📄 СОЗДАЮ cmsAdminPage (test-scoped)');
    const page = await cmsAdminContext.newPage();
    await use(page);
    console.log('📄 ЗАКРЫВАЮ cmsAdminPage (test-scoped)');
    // Страница закроется автоматически при закрытии контекста
    page.close()
  },

  // 📄 Test-scoped: страницы создаются из worker-scoped контекстов
  gymAdminPage: async ({ gymAdminContext }, use) => {
    console.log('📄 СОЗДАЮ gymAdminPage (test-scoped)');
    const page = await gymAdminContext.newPage();
    await use(page);
    console.log('📄 ЗАКРЫВАЮ gymAdminPage (test-scoped)');
    page.close()
  }
});

export { expect } from '@playwright/test';