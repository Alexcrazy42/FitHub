import { Page } from "@playwright/test";
import { BasePage } from "./BasePage";

export interface IEquipmentCreateModel {
    brand: string
    name: string
    description: string
    addDescription: string | null
    instructionAddBefore?: Date,
    isActive: boolean
}

export class EquipmentPage extends BasePage {
    constructor(page: Page) {
        super(page);
    }

    async open() : Promise<void> {
        await this.page.goto(`${this.baseUrl}/admin/equipments`)
    }

    async create(model: IEquipmentCreateModel) : Promise<void> {
        await this.page.getByText('Добавить тренажер').click();

        // BRAND
        const brandLocator = this.page.locator('#rc_select_2')
        await brandLocator.click();
        await brandLocator.type(model.brand, { delay: 500 });
        await this.page.keyboard.press('Enter');

        // NAME
        await this.page.getByPlaceholder('Название оборудования').fill(model.name);

        // DESCRIPTION
        await this.page.getByRole('textbox', { name: 'Описание' }).nth(0).fill(model.description);

        // ADD_DESCRIPTION
        if (model.addDescription != null) {
            await this.page.getByRole('textbox', { name: 'Доп. описание' }).nth(0).fill(model.addDescription);
        }

        // INSTRUCTION_ADD_BEFORE
        if (model.instructionAddBefore != null) {
            await this.page.getByPlaceholder('Select date').click();
            await this.page
                .locator('.ant-picker-dropdown')
                .locator('[title="2026-07-25"]')
                .click();
        }

        // ACTIVE
        const switchEl = await this.page.getByRole('switch').nth(0);
        const isChecked = await switchEl.isChecked()
        if (model.isActive && !isChecked) {
            await switchEl.click();
        }

        // CREATE BUTTON
        const createBtn = await this.page.getByRole('button', {name: 'Создать'}).nth(0);
        await createBtn.click();
    }

    async createEquipmentBrandSearch(brand: string) {
        await this.page.getByText('Добавить тренажер').click();

        // BRAND
        const brandLocator = this.page.locator('#rc_select_2')
        await brandLocator.click();
        await brandLocator.type(brand, { delay: 500 });
        await this.page.keyboard.press('Enter');
    }
}