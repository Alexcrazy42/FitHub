import { Page } from '@playwright/test';
import { APP_URLS } from '../config/sites';

export abstract class BasePage {
  protected readonly page: Page;
  protected readonly baseUrl: string;

  constructor(page: Page) {
    this.page = page;
    this.baseUrl = APP_URLS['fithub'];
  }

  async goto(url: string): Promise<void> {
    await this.page.goto(url);
  }

  async getCurrentUrl(): Promise<string> {
    return this.page.url();
  }
}