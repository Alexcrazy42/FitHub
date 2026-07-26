import { APP_URLS } from "../config/sites";
import { test, expect } from "../fixtures/fixtures";
import { ChatPage } from "../pages/ChatPage";
import path from 'path';


test.describe('chat', () => {
    test.describe('messages', () => {
        test('send message', async ({ cmsAdminPage }) => {
            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            const text = crypto.randomUUID();
            await chatPage.sendMessage(text)

            const lastSendedMessage = await chatPage.getLastTextCurrentUserMessage();
            expect(lastSendedMessage).toBe(text);
        });

        test('fill empty message - send button is disabled', async ({ cmsAdminPage }) => {
            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            await chatPage.fillEmptyText();
            expect(chatPage.sendBtn).toBeDisabled({ timeout: 10000 })
        });


        test('fill message with only spaces - send button is disabled', async ({ cmsAdminPage }) => {
            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            await chatPage.fillMessageWithOnlySpaces();
        });

        test('send sticker', async ({ cmsAdminPage }) => {
            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            await chatPage.sendTestSticker();

            const stickerUrl = await chatPage.getLastMySendedStickerUrl();
            expect(stickerUrl).toContain(chatPage.testStickerId);
        });

        test('send supported preview file', async ({ cmsAdminPage }) => {
            const testFile = path.join(__dirname, 'test_files', 'test.pdf');

            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            await chatPage.attachFiles([testFile])

            const pendingFiles = await chatPage.getPendingSendFiles();
            expect(pendingFiles.length).toBe(1);
            expect(testFile).toContain(pendingFiles[0]);
            
            await chatPage.clickSendMessage();

            const messageWithFiles = await chatPage.getLastMessageFiles();
            expect(messageWithFiles.fileNames.length).toBe(1);
            expect(testFile).toContain(messageWithFiles.fileNames[0]);

            const canPreview = await chatPage.canPreviewFile(messageWithFiles.message, 'test.pdf');
            expect(canPreview).toBe(true);
        });

        test('send unsupported preview file', async ({ cmsAdminPage }) => {
            const testFile = path.join(__dirname, 'test_files', 'test.md');

            const chatPage = new ChatPage(cmsAdminPage);
            await chatPage.open('cmsAdmin');
            await chatPage.waitForChatLoaded('Александр, ТестАдминЗала')

            await chatPage.attachFiles([testFile])

            const pendingFiles = await chatPage.getPendingSendFiles();
            expect(pendingFiles.length).toBe(1);
            expect(testFile).toContain(pendingFiles[0]);
            
            await chatPage.clickSendMessage();

            const messageWithFiles = await chatPage.getLastMessageFiles();
            expect(messageWithFiles.fileNames.length).toBe(1);
            expect(testFile).toContain(messageWithFiles.fileNames[0]);

            const canPreview = await chatPage.canPreviewFile(messageWithFiles.message, 'test.md');
            expect(canPreview).toBe(false);
        });

        test('send audio message', async ({ cmsAdminPage, gymAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            expect(gymAdminPage.url()).toBe('');
            throw new Error();
        });

        test('edit message', async ({ cmsAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            throw new Error();
        });

        test('delete message', async ({ cmsAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            throw new Error();
        });

        test('scroll messages - messages loading', async ({ 
            cmsAdminPage, 
            gymAdminPage 
        }) => {
            expect(cmsAdminPage.url()).toBe('');
            expect(gymAdminPage.url()).toBe('');
            throw new Error();
        });

        test('user cannot edit another users message', async ({ 
            cmsAdminPage, 
            gymAdminPage 
        }) => {
            // gymAdminPage отправляет сообщение
            // cmsAdminPage пытается его отредактировать → ошибка
        });

        test('user cannot delete another users message', async ({ 
            cmsAdminPage, 
            gymAdminPage 
        }) => {
            // Аналогично
        });

        test('send message with 5000+ characters - should truncate or show error', async ({ cmsAdminPage }) => {
            // Проверка лимитов
        });

        test('send file exceeding max size limit', async ({ cmsAdminPage }) => {
            // Проверка ограничений на файлы
        });

        test('send unsupported file format', async ({ cmsAdminPage }) => {
            // Например, .exe или .sh
        });
    });

    test.describe('reply to messages', () => {
        test('reply to specific message', async ({ cmsAdminPage }) => {
            // Ответ с цитированием
        });

        test('view message thread', async ({ cmsAdminPage }) => {
            // Просмотр ветки сообщений
        });
    });

    test.describe('chat managment', () => {
        test('create chat', async ({ cmsAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            throw new Error();
        });

        test('add new recipient to chat', async ({ cmsAdminPage, gymAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            expect(gymAdminPage.url()).toBe('');
            throw new Error();
        });

        test('exclude from chat, user sees old messages, but not sees new', async ({ cmsAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            throw new Error();
        });


        test('leave from chat', async ({ cmsAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            throw new Error();
        });
    })

    test.describe('resilience', () => {
        test('connection failed, - show status and retry', async ({ cmsAdminPage, gymAdminPage }) => {
            expect(cmsAdminPage.url()).toBe('');
            expect(gymAdminPage.url()).toBe('');
            throw new Error();
        });
    })

    test.describe('access control', () => {
        test('unauthorized user cannot access chat', async ({ page }) => {
            const chatPage = new ChatPage(page);
            await chatPage.open('cmsAdmin');

            await page.waitForURL(`${APP_URLS['fithub']}/login`, { timeout: 10000 });
            expect(page.url()).toBe(`${APP_URLS['fithub']}/login`);
        });

        test('different user roles have different permissions', async ({ 
            cmsAdminPage, 
            gymAdminPage
        }) => {
            // Например: обычный пользователь не может удалять чужие сообщения
        });
    });
});