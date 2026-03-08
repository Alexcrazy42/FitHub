import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IMessageResponse } from '../types/messaging';

interface MessagesState {
  messages: Record<string, IMessageResponse[]>; // chatId -> messages[]
  hasMore: Record<string, boolean>; // chatId -> hasMore
  nextCursor: Record<string, string | undefined>; // chatId -> cursor
  loading: Record<string, boolean>; // chatId -> loading
  sendingMessages: Record<string, IMessageResponse[]>; // chatId -> messages[] (optimistic)
}

const initialState: MessagesState = {
  messages: {},
  hasMore: {},
  nextCursor: {},
  loading: {},
  sendingMessages: {},
};

const messagesSlice = createSlice({
  name: 'messages',
  initialState,
  reducers: {
    // Set messages for chat (initial load - latest messages)
    setMessages: (
      state,
      action: PayloadAction<{
        chatId: string;
        messages: IMessageResponse[];
        hasMore: boolean;
        nextCursor?: string;
      }>
    ) => {
      const { chatId, messages, hasMore, nextCursor } = action.payload;
      state.messages[chatId] = messages;
      state.hasMore[chatId] = hasMore;
      state.nextCursor[chatId] = nextCursor;
      state.loading[chatId] = false;
    },

    // Add new message (from SignalR or after sending)
    addMessage: (
      state,
      action: PayloadAction<{ chatId: string; message: IMessageResponse }>
    ) => {
      const { chatId, message } = action.payload;

      if (!state.messages[chatId]) {
        state.messages[chatId] = [];
      }

      // Проверка на дубликаты
      const existingIndex = state.messages[chatId].findIndex((m) => m.id === message.id);
      if (existingIndex === -1) {
        state.messages[chatId].push(message);

        // Удаляем из sendingMessages если есть (optimistic update completed)
        if (state.sendingMessages[chatId]) {
          state.sendingMessages[chatId] = state.sendingMessages[chatId].filter(
            (m) => m.id !== message.id
          );
        }
      } else if (message.attachments.length > state.messages[chatId][existingIndex].attachments.length) {
        // Если дубликат, но новая версия содержит больше вложений (например, HTTP-ответ пришёл
        // после SignalR-события, которое было отправлено до сохранения вложений) — обновляем.
        state.messages[chatId][existingIndex] = message;
      }
    },

    // Add older messages (pagination - scroll up)
    addMessagesToTop: (
      state,
      action: PayloadAction<{
        chatId: string;
        messages: IMessageResponse[];
        hasMore: boolean;
        nextCursor?: string;
      }>
    ) => {
      const { chatId, messages, hasMore, nextCursor } = action.payload;

      if (!state.messages[chatId]) {
        state.messages[chatId] = [];
      }

      // Добавляем в начало (старые сообщения)
      state.messages[chatId] = [...messages, ...state.messages[chatId]];
      state.hasMore[chatId] = hasMore;
      state.nextCursor[chatId] = nextCursor;
      state.loading[chatId] = false;
    },

    // Update message (edit from SignalR)
    updateMessage: (
      state,
      action: PayloadAction<{
        chatId: string;
        messageId: string;
        updates: Partial<IMessageResponse>;
      }>
    ) => {
      const { chatId, messageId, updates } = action.payload;

      if (state.messages[chatId]) {
        const index = state.messages[chatId].findIndex((m) => m.id === messageId);
        if (index !== -1) {
          state.messages[chatId][index] = {
            ...state.messages[chatId][index],
            ...updates,
          };
        }
      }
    },

    // Delete message (from SignalR)
    deleteMessage: (
      state,
      action: PayloadAction<{ chatId: string; messageId: string }>
    ) => {
      const { chatId, messageId } = action.payload;

      if (state.messages[chatId]) {
        state.messages[chatId] = state.messages[chatId].filter(
          (m) => m.id !== messageId
        );
      }
    },

    // Optimistic update - add sending message
    addSendingMessage: (
      state,
      action: PayloadAction<{ chatId: string; message: IMessageResponse }>
    ) => {
      const { chatId, message } = action.payload;

      if (!state.sendingMessages[chatId]) {
        state.sendingMessages[chatId] = [];
      }

      state.sendingMessages[chatId].push(message);
    },

    // Mark sending message as failed
    markSendingMessageAsFailed: (
      state,
      action: PayloadAction<{ chatId: string; messageId: string }>
    ) => {
      const { chatId, messageId } = action.payload;

      if (state.sendingMessages[chatId]) {
        const message = state.sendingMessages[chatId].find(
          (m) => m.id === messageId
        );
        if (message) {
          // (message as IMessageResponse).failed = true; // TODO
        }
      }
    },

    // Remove sending message (on cancel or after failure)
    removeSendingMessage: (
      state,
      action: PayloadAction<{ chatId: string; messageId: string }>
    ) => {
      const { chatId, messageId } = action.payload;

      if (state.sendingMessages[chatId]) {
        state.sendingMessages[chatId] = state.sendingMessages[chatId].filter(
          (m) => m.id !== messageId
        );
      }
    },

    // Loading states
    setMessagesLoading: (
      state,
      action: PayloadAction<{ chatId: string; loading: boolean }>
    ) => {
      const { chatId, loading } = action.payload;
      state.loading[chatId] = loading;
    },

    // Clear messages for chat (when leaving chat)
    clearMessages: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      delete state.messages[chatId];
      delete state.hasMore[chatId];
      delete state.nextCursor[chatId];
      delete state.loading[chatId];
      delete state.sendingMessages[chatId];
    },

    // Clear all messages
    clearAllMessages: () => initialState,
  },
});

export const {
  setMessages,
  addMessage,
  addMessagesToTop,
  updateMessage,
  deleteMessage,
  addSendingMessage,
  markSendingMessageAsFailed,
  removeSendingMessage,
  setMessagesLoading,
  clearMessages,
  clearAllMessages,
} = messagesSlice.actions;

export default messagesSlice.reducer;
