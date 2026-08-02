import { Page, Locator } from '@playwright/test';
import { BasePage } from './BasePage';
import { POST_LOGIN_URL } from '../config/sites';
import { Credentials } from '../config/testUsers';


export class LoginPage extends BasePage {
  // Все локаторы — приватные, с четким типом Locator
  private readonly usernameInput: Locator;
  private readonly passwordInput: Locator;
  private readonly loginButton: Locator;

  constructor(page: Page) {
    super(page);
    
    // Инициализируем локаторы в конструкторе
    this.usernameInput = page.getByPlaceholder('Введите ваш email');
    this.passwordInput = page.getByPlaceholder('Введите ваш пароль');
    this.loginButton = page.getByRole('button', { name: 'Войти' });
  }

  async open(): Promise<void> {
    await this.page.goto(`${this.baseUrl}/login`);
    await this.page.waitForLoadState('networkidle');
  }

  async fillUsername(username: string): Promise<void> {
    await this.usernameInput.type(username);
  }

  async fillPassword(password: string): Promise<void> {
    await this.passwordInput.type(password);
  }

  async clickLoginButton(): Promise<void> {
    await this.loginButton.click();
  }
  
  async login(credentials: Credentials): Promise<void> {
    await this.fillUsername(credentials.login);
    await this.fillPassword(credentials.password);
    await this.clickLoginButton();

    const postLogin = POST_LOGIN_URL[credentials.userRole];
    
    await this.page.waitForURL(`**/${postLogin}`, { timeout: 10000 });
  }
}