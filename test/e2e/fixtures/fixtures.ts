import { test as base, Browser, BrowserContext, Page } from '@playwright/test';
import { FitHubCreds } from '../config/testUsers';
import { App, APP_URLS, POST_LOGIN_URL, UserRole } from '../config/sites';
import { LoginPage } from '../pages/FitHubLoginPage';


const storageCache = new Map<string, any>();

async function createFitHubAuthenticatedContext(
  browser: Browser,
  userRole: UserRole,
  app: App
): Promise<BrowserContext> {
  const cacheKey = `${userRole}_${app}`;

  if (storageCache.has(cacheKey)) {
    return await browser.newContext({ 
      storageState: storageCache.get(cacheKey) 
    });
  }
  
  const context = await browser.newContext();
  const page = await context.newPage();

  const creds = FitHubCreds[userRole];

  const loginPage = new LoginPage(page);
  await loginPage.open();
  await loginPage.login(creds);

  const storageState = await context.storageState();
  storageCache.set(cacheKey, storageState);

  await page.close();

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

    const url = APP_URLS['fithub'];
    const cmsUrl = `${url}/${POST_LOGIN_URL['cmsAdmin']}`;

    await page.goto(cmsUrl);

    await use(page);
    console.log('📄 ЗАКРЫВАЮ cmsAdminPage (test-scoped)');
    // Страница закроется автоматически при закрытии контекста
    page.close()
  },

  // 📄 Test-scoped: страницы создаются из worker-scoped контекстов
  gymAdminPage: async ({ gymAdminContext }, use) => {
    console.log('📄 СОЗДАЮ gymAdminPage (test-scoped)');
    
    const page = await gymAdminContext.newPage();

    const url = APP_URLS['fithub'];
    const gymUrl = `${url}/${POST_LOGIN_URL['gymAdmin']}`;

    await page.goto(gymUrl);

    await use(page);
    console.log('📄 ЗАКРЫВАЮ gymAdminPage (test-scoped)');
    page.close()
  }
});

export { expect } from '@playwright/test';