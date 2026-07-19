import { BACKEND_URLS } from "../config/sites";
import { expect, test } from "../fixtures/fixtures";
import { EquipmentPage, IEquipmentCreateModel } from "../pages/EquipmentPage";
import { expectNoRequest } from "../utils/pageExtensions";

test('equipment create', async ({ cmsAdminPage }) => {
    const responsePromise = cmsAdminPage.waitForResponse(
        resp => resp.url().includes('/api/v1/equipments')
            && resp.request().method() == 'POST'
    );

    const page = new EquipmentPage(cmsAdminPage);
    await page.open();

    const createModel : IEquipmentCreateModel = {
        brand: 'TestBrand',
        name: 'test equipment',
        description: 'test equipment description',
        addDescription: null,
        instructionAddBefore: new Date(2026, 8, 1),
        isActive: true
    }

    await page.create(createModel);

    const response = await responsePromise;
    expect(response.status()).toBe(200);

    const json = await response.json();
    const newId = json['id'];

    const url = BACKEND_URLS['fithub'];
    const deleteResponse = await cmsAdminPage.request.delete(`${url}/api/v1/equipments/${newId}`)

    expect(deleteResponse.status()).toBe(200);
})

test('equipment edit', async ({ cmsAdminPage }) => {
    throw new Error('not implemented')
})

test('equipment delete', async ({ cmsAdminPage }) => {
    throw new Error('not implemented')
})

test('should search brands only after typing at least 3 characters', async ({ cmsAdminPage }) => {
    const page = new EquipmentPage(cmsAdminPage);
    await page.open();

    await page.createEquipmentBrandSearch('Te');

    await expectNoRequest(cmsAdminPage, '/api/v1/brands', 2000);
})
