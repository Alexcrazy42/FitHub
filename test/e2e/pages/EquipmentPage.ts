import { Page } from "@playwright/test";
import { BasePage } from "./BasePage";

export class EquipmentPage extends BasePage {
    constructor(page: Page) {
        super(page);
    }

    async open() : Promise<void> {
        await this.page.goto(`${this.baseUrl}/admin/equipments`)
    }
}