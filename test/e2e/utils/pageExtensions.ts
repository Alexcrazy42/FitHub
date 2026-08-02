import { Page } from "@playwright/test";

export async function expectNoRequest(
    page: Page,
    urlPattern: string | RegExp,
    timeout: number
): Promise<void> {
    const responsePromise = page.waitForResponse(
        resp => {
            const urlMatch = typeof urlPattern === 'string'
                ? resp.url().includes(urlPattern)
                : urlPattern.test(resp.url());
            return urlMatch;
        },
        { timeout }
    );
    try {
        await responsePromise;
        throw new Error(`Запрос ${urlPattern} был отправлен, а не должен был`);
    } catch (error) {
        if (error instanceof Error && error.message.includes('Timeout')) {
            return;
        }
        throw error;
    }
}