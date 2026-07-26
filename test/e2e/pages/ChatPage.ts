import { expect, Locator, Page } from "@playwright/test";
import { BasePage } from "./BasePage";
import { PREFIX_URL, UserRole } from "../config/sites";

export interface MessageElementWithFiles {
    message: Locator;
    fileNames : string[]
}


export class ChatPage extends BasePage {
    public readonly textInput : Locator;
    public readonly sendBtn : Locator;
    public readonly testStickerId : string = '019f90b1-5fe6-75af-ba8c-9ede2bc5b9e5';
    private messageContainer: Locator;
    private stickerAddBtn: Locator;
    private attachFileInput: Locator;
    private pendingSendFileContainer : Locator;

    constructor(page: Page) {
        super(page);

        this.textInput = this.page.locator('textarea[placeholder="Введите сообщение..."]');
        this.sendBtn = this.page.locator(
            'button.ant-btn-primary:has-text("Отправить")'
        );
        this.messageContainer = page.locator(
            '#root > div > div > main > div > div > main > div > div.flex-1.overflow-hidden.bg-gray-50.min-h-0 > div > div > div'
        );
        this.stickerAddBtn = page.locator('#root > div > div > main > div > div > main > div > div:nth-child(3) > div > div > button:nth-child(4)')
        this.attachFileInput = page.locator('input[type=file]');
        this.pendingSendFileContainer = page.locator('#root > div > div > main > div > div > main > div > div:nth-child(3) > div > div.px-4.py-2.flex.flex-wrap.gap-2.bg-gray-50.border-b.border-gray-200')
    }

    // ========== Навигация ==========
    async open(role: UserRole): Promise<void> {
        const prefix = PREFIX_URL[role];
        await this.page.goto(`${this.baseUrl}/${prefix}/chat`);
    }

    async waitForChatLoaded(chatName: string): Promise<void> {
        const chatItem = this.page
            .locator('#chat-list-scrollable > div > div > div')
            .filter({ has: this.page.locator('h3', { hasText: chatName }) })
            .first();

        await chatItem.click();
    }

    // ========== Отправка сообщений ==========
    async sendMessage(text: string): Promise<void> {
        await this.textInput.waitFor({ state: 'visible' });
        await this.textInput.focus();
        
        await this.textInput.fill(text);
        
        await expect(this.sendBtn).toBeEnabled({ timeout: 5000 });
        
        await this.sendBtn.click();
        await expect(this.sendBtn).toBeEnabled({ timeout: 10000 });
        
        await expect(this.textInput).toHaveValue('', { timeout: 5000 });
    }

    async clickSendMessage() : Promise<void> {
        await this.sendBtn.click();
        await expect(this.sendBtn).toBeDisabled({ timeout: 10000 });
    }

    async fillEmptyText(): Promise<void> {
        await this.textInput.waitFor({ state: 'visible' });
        await this.textInput.focus();
        
        await this.textInput.fill('');

        await expect(this.sendBtn).toBeDisabled({ timeout: 10000 });
    }

    async fillMessageWithOnlySpaces(): Promise<void> {
        await this.textInput.waitFor({ state: 'visible' });
        await this.textInput.focus();
        
        await this.textInput.fill('   ');

        await expect(this.sendBtn).toBeDisabled({ timeout: 10000 });
    }

    async sendTestSticker(): Promise<void> {
        const stickerPopup = this.page.locator('#«rp»');
        await this.stickerAddBtn.click();

        const testSticketLocator = this.page.locator('#rc-tabs-0-panel-019f90b1-4337-7b0d-90c4-5ca2c92f21b7 > div > button > img');
        await testSticketLocator.click();

        await stickerPopup.waitFor({ state: 'hidden', timeout: 15000 });
    }

    async attachFiles(filePaths: string[]): Promise<void> {
        await this.attachFileInput.setInputFiles(filePaths);

        await this.pendingSendFileContainer.waitFor({ state: 'visible', timeout: 10000 })
    }

    async getPendingSendFiles() : Promise<string[]> {
        const fileNames: string[] = [];
        
        const fileDivs = this.pendingSendFileContainer.locator('> div');
        const count = await fileDivs.count();
        
        for (let i = 0; i < count; i++) {
            const nameSpan = fileDivs.nth(i).locator('span.max-w-\\[120px\\].truncate.text-gray-700');
            const fileName = await nameSpan.textContent();
            if (fileName) {
                fileNames.push(fileName.trim());
            }
        }
        
        return fileNames;
    }

    // ========== Получение сообщений ==========
    async getLastTextCurrentUserMessage(): Promise<string> {
        const messages = await this.messageContainer.locator('> div').all();
        
        for (let i = messages.length - 1; i >= 0; i--) {
            const message = messages[i];

            const myMessageElement = message.locator('> div > div.flex-1.max-w-2xl.items-end');
            const isMyMessage = await myMessageElement.count() > 0;
            
            if (isMyMessage) {
                const textElement = myMessageElement.locator('> div > div > div.break-words.whitespace-pre-wrap');
                return await textElement.textContent() || '';
            }
        }
        
        throw new Error('No my messages found in chat');
    }

    async getLastTextIncomingMessage() : Promise<string> {
        const messages = await this.messageContainer.locator('> div').all();

        for (let i = messages.length - 1; i >= 0; i--) {
            const message = messages[i];
            const incomingMessageElement = message.locator('> div > div.flex-1.max-w-2xl.items-start');
            const isIncoingMessage = await incomingMessageElement.count() > 0;
            
            if (isIncoingMessage) {
                const textElement = incomingMessageElement.locator('> div > div > div.break-words.whitespace-pre-wrap');
                return await textElement.textContent() || '';
            }
        }
        
        throw new Error('No my messages found in chat');
    }

    async getLastIncomingMessageAuthor() : Promise<string> {
        const messages = await this.messageContainer.locator('> div').all();
        
        for (let i = messages.length - 1; i >= 0; i--) {
            const message = messages[i];

            const myMessageElementAuthor = message.locator('> div > div.flex-1.max-w-2xl.items-start > div');
            const isMyMessage = await myMessageElementAuthor.count() > 0;
            
            if (isMyMessage) {
                const textElement = myMessageElementAuthor.locator('> span.text-sm');
                return await textElement.textContent() || '';
            }
        }
        
        throw new Error('No my messages found in chat');
    }

    async getLastMySendedStickerUrl() : Promise<string> {
        const messages = await this.messageContainer.locator('> div').all();
        
        const lastIdx = messages.length - 2;

        const message = messages[lastIdx];

        const mySticker = message.locator('> div > div.flex.flex-col.items-end > img');
        const isSticker = await mySticker.count() > 0;

        if (!isSticker) {
            throw new Error('No last sended sticker in chat');
        }

        const src = await mySticker.getAttribute('src');
        if (!src) {
            throw new Error('Атрибут "src" не найден у элемента');
        }
        return src;
    }

    async getLastMessageFiles() : Promise<MessageElementWithFiles> {
        const fileNames : string[] = [];

        const messages = await this.messageContainer.locator('> div').all();
        
        const lastIdx = messages.length - 2;

        const message = messages[lastIdx];

        const possibleFileMessage = message.locator('> div > div.flex-1.max-w-2xl.items-end.flex.flex-col > div > div > div.flex.flex-col.gap-2.mt-1');
        const isFileMessage = await possibleFileMessage.count() > 0;

        if (!isFileMessage) {
            throw new Error('last message no contains files');
        }

        const files = await possibleFileMessage.locator('> div').all();

        for(let i = 0; i < files.length; i++) {
            const file = files[i];

            const fileNameEl = await file.locator('> div.flex-1.min-w-0 > p.text-sm.font-medium.truncate');
            const isFileName = await fileNameEl.count() > 0;

            if (isFileName) {
                const text = await fileNameEl.textContent() || '';
                if(text == '') {
                    continue;
                }

                fileNames.push(text);
            }
        }

        if(fileNames.length == 0)
        {
            throw new Error('no fileNames')
        }

        return {
            message: message,
            fileNames: fileNames
        }
    }

    async canPreviewFile(message: Locator, fileName: string) : Promise<boolean> {
        const possibleFileMessage = message.locator('> div > div.flex-1.max-w-2xl.items-end.flex.flex-col > div > div > div.flex.flex-col.gap-2.mt-1');
        const isFileMessage = await possibleFileMessage.count() > 0;

        if (!isFileMessage) {
            throw new Error('last message no contains files');
        }

        const files = await possibleFileMessage.locator('> div').all();

        for(let i = 0; i < files.length; i++) {
            const file = files[i];

            const fileNameEl = await file.locator('> div.flex-1.min-w-0 > p.text-sm.font-medium.truncate');
            const isFileName = await fileNameEl.count() > 0;

            if (isFileName) {
                const text = await fileNameEl.textContent() || '';
                if(text == '') {
                    continue;
                }

                if(text == fileName) {
                    const previewLocator = await file.locator('> div.flex.flex-col.gap-1 > button:nth-child(1) > span > span > svg[data-icon="eye"]');
                    return await previewLocator.isVisible();
                }                
            }
        }

        return false;
    }

    // ========== Редактирование и удаление ==========
    async editMessage(messageId: string, newText: string): Promise<void> {
        throw new Error('not implemented');
    }

    async deleteMessage(messageId: string): Promise<void> {
        throw new Error('not implemented');
    }

    async waitForMessageDeleted(messageId: string): Promise<void> {
        throw new Error('not implemented');
    }

    // ========== Права доступа ==========
    async canEditMessage(messageId: string): Promise<boolean> {
        throw new Error('not implemented');
    }

    async canDeleteMessage(messageId: string): Promise<boolean> {
        throw new Error('not implemented');
    }

    async getMessageActions(messageId: string): Promise<{
        canEdit: boolean;
        canDelete: boolean;
        canReply: boolean;
    }> {
        throw new Error('not implemented');
    }

    async getCurrentUserRole(): Promise<UserRole> {
        throw new Error('not implemented');
    }

    // ========== Управление чатами ==========
    async openChat(chatName: string): Promise<void> {
        throw new Error('not implemented');
    }

    async createChat(chatName: string, memberNames: string[]): Promise<void> {
        throw new Error('not implemented');
    }

    async getChatList(): Promise<string[]> {
        throw new Error('not implemented');
    }

    async getCurrentChatName(): Promise<string> {
        throw new Error('not implemented');
    }

    async getCurrentChatMembers(): Promise<string[]> {
        throw new Error('not implemented');
    }

    async getChatParticipantCount(): Promise<number> {
        throw new Error('not implemented');
    }

    async getChatCreator(): Promise<string> {
        throw new Error('not implemented');
    }

    async getChatCreationDate(): Promise<string> {
        throw new Error('not implemented');
    }

    async addMemberToChat(memberName: string): Promise<void> {
        throw new Error('not implemented');
    }

    async removeMemberFromChat(memberName: string): Promise<void> {
        throw new Error('not implemented');
    }

    async leaveChat(): Promise<void> {
        throw new Error('not implemented');
    }

    async isMemberInChat(memberName: string): Promise<boolean> {
        throw new Error('not implemented');
    }

    // ========== Ответы и ветки ==========
    async replyToMessage(messageId: string, replyText: string): Promise<void> {
        throw new Error('not implemented');
    }

    async getRepliesToMessage(messageId: string): Promise<string[]> {
        throw new Error('not implemented');
    }

    async viewMessageThread(messageId: string): Promise<void> {
        throw new Error('not implemented');
    }

    // ========== Скролл и загрузка ==========
    async scrollToTop(): Promise<void> {
        throw new Error('not implemented');
    }

    async scrollToBottom(): Promise<void> {
        throw new Error('not implemented');
    }

    async waitForMessagesLoading(): Promise<void> {
        throw new Error('not implemented');
    }

    async isLoadingMessages(): Promise<boolean> {
        throw new Error('not implemented');
    }

    async waitForNewMessages(count: number): Promise<void> {
        throw new Error('not implemented');
    }

    // ========== Ошибки и валидация ==========
    async getErrorMessage(): Promise<string> {
        throw new Error('not implemented');
    }

    async isErrorVisible(): Promise<boolean> {
        throw new Error('not implemented');
    }

    async getErrorType(): Promise<'validation' | 'network' | 'permission'> {
        throw new Error('not implemented');
    }

    // ========== Состояние UI ==========
    async isMessageInputEnabled(): Promise<boolean> {
        throw new Error('not implemented');
    }

    async isSendButtonEnabled(): Promise<boolean> {
        throw new Error('not implemented');
    }

    async getMessageInputValue(): Promise<string> {
        throw new Error('not implemented');
    }

    async clearMessageInput(): Promise<void> {
        throw new Error('not implemented');
    }

    async isTypingIndicatorVisible(): Promise<boolean> {
        throw new Error('not implemented');
    }

    // ========== Файловые ограничения ==========
    async getFileSizeLimit(): Promise<number> {
        throw new Error('not implemented');
    }

    async getAllowedFileTypes(): Promise<string[]> {
        throw new Error('not implemented');
    }

    async getMaxFileSize(): Promise<number> {
        throw new Error('not implemented');
    }

    // ========== Состояние соединения ==========
    async getConnectionStatus(): Promise<'connected' | 'disconnected' | 'reconnecting'> {
        throw new Error('not implemented');
    }

    async simulateConnectionLoss(): Promise<void> {
        throw new Error('not implemented');
    }

    async waitForConnectionRestored(): Promise<void> {
        throw new Error('not implemented');
    }

    // ========== Участники ==========
    async getUnreadMessageCount(chatName: string): Promise<number> {
        throw new Error('not implemented');
    }

    // ========== Хелперы ==========
    async waitForMessageDelivered(messageId: string): Promise<void> {
        throw new Error('not implemented');
    }
}