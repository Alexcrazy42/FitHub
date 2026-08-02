import { Page } from "@playwright/test";
import { BACKEND_URLS } from "../config/sites";
import { expect, test } from "../fixtures/fixtures";
import { EquipmentPage, IEquipmentCreateOrEditModel } from "../pages/EquipmentPage";
import { expectNoRequest } from "../utils/pageExtensions";

const  createEquipment = async (cmsAdminPage: Page, model: IEquipmentCreateOrEditModel): Promise<void> => {
    const page = new EquipmentPage(cmsAdminPage);
    await page.open();

    await page.create(model);    
}

test('equipment create', async ({ cmsAdminPage }) => {
    const responsePromise = cmsAdminPage.waitForResponse(
        resp => resp.url().includes('/api/v1/equipments')
            && resp.request().method() == 'POST'
    );

    const createModel : IEquipmentCreateOrEditModel = {
        brand: 'TestBrand',
        name: 'test equipment',
        description: 'test equipment description',
        addDescription: null,
        instructionAddBefore: new Date(2026, 8, 1),
        isActive: true
    }

    await createEquipment(cmsAdminPage, createModel);

    const response = await responsePromise;
    expect(response.status()).toBe(200);

    const json = await response.json();
    const newId = json['id'];

    const url = BACKEND_URLS['fithub'];
    const deleteResponse = await cmsAdminPage.request.delete(`${url}/api/v1/equipments/${newId}`)

    expect(deleteResponse.status()).toBe(200);
})

test('equipment edit', async ({ cmsAdminPage }) => {
    const responsePromise = cmsAdminPage.waitForResponse(
        resp => resp.url().includes('/api/v1/equipments')
            && resp.request().method() == 'POST'
    );

    const createModel : IEquipmentCreateOrEditModel = {
        brand: 'TestBrand',
        name: 'test equipment',
        description: 'test equipment description',
        addDescription: null,
        instructionAddBefore: new Date(2026, 8, 1),
        isActive: true
    }

    await createEquipment(cmsAdminPage, createModel);

    const response = await responsePromise;
    expect(response.status()).toBe(200);

    const json = await response.json();
    const newId = json['id'];

    const eqPage = new EquipmentPage(cmsAdminPage);
    await eqPage.openConcretePage(newId);

    const updatePromise = cmsAdminPage.waitForResponse(
        resp => resp.url().includes('/api/v1/equipments')
            && resp.request().method() == 'PUT'
    );

    const updateModel : IEquipmentCreateOrEditModel = {
        brand: 'TestBrand',
        name: 'test equipment updated',
        description: 'test equipment description updated',
        addDescription: 'updated',
        instructionAddBefore: new Date(2026, 8, 1),
        isActive: true
    }

    await eqPage.edit(updateModel);

    const updateResponse = await updatePromise;

    expect(updateResponse.status()).toBe(200);

    const updateJson = await updateResponse.json();
    expect(updateJson['id']).toBe(newId);
    expect(updateJson['name']).toBe(updateModel.name);
    expect(updateJson['description']).toBe(updateModel.description);
    expect(updateJson['additionalDescroption']).toBe(updateModel.addDescription);
    expect(updateJson['isActive']).toBe(updateModel.isActive);

    const url = BACKEND_URLS['fithub'];
    const deleteResponse = await cmsAdminPage.request.delete(`${url}/api/v1/equipments/${newId}`)

    expect(deleteResponse.status()).toBe(200);
})

test('equipment delete', async ({ cmsAdminPage }) => {
    const responsePromise = cmsAdminPage.waitForResponse(
        resp => resp.url().includes('/api/v1/equipments')
            && resp.request().method() == 'POST'
    );

    const createModel : IEquipmentCreateOrEditModel = {
        brand: 'TestBrand',
        name: 'test equipment',
        description: 'test equipment description',
        addDescription: null,
        instructionAddBefore: new Date(2026, 8, 1),
        isActive: true
    }

    await createEquipment(cmsAdminPage, createModel);

    const response = await responsePromise;
    expect(response.status()).toBe(200);

    const json = await response.json();
    const newId = json['id'];

    const eqPage = new EquipmentPage(cmsAdminPage);
    await eqPage.open();
    await eqPage.delete(newId);

    const url = BACKEND_URLS['fithub'];

    const getResponse = await cmsAdminPage.request.get(`${url}/api/v1/equipments/${newId}`);
    expect(getResponse.status()).toBe(404);
})

test('should search brands only after typing at least 3 characters', async ({ cmsAdminPage }) => {
    const page = new EquipmentPage(cmsAdminPage);
    await page.open();

    await page.createEquipmentBrandSearch('Te');

    await expectNoRequest(cmsAdminPage, '/api/v1/brands', 2000);
})
