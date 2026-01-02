import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IChatMessageResponse } from '../types/messaging';

interface ChatState {
  chats: IChatMessageResponse[];
  hasMore: boolean;
  nextCursor?: string;
  loading: boolean;
  error: string | null;
}

const initialState: ChatState = {
  chats: [],
  hasMore: true,
  nextCursor: undefined,
  loading: false,
  error: null,
};

const chatSlice = createSlice({
  name: 'chat',
  initialState,
  reducers: {
    // Set all chats (initial load)
    setChats: (state, action: PayloadAction<IChatMessageResponse[]>) => {
      state.chats = action.payload;
      state.loading = false;
    },

    // Add more chats (infinite scroll)
    addChats: (
      state,
      action: PayloadAction<{
        chats: IChatMessageResponse[];
        hasMore: boolean;
        nextCursor?: string;
      }>
    ) => {
      state.chats.push(...action.payload.chats);
      state.hasMore = action.payload.hasMore;
      state.nextCursor = action.payload.nextCursor;
      state.loading = false;
    },

    // Update chat (from SignalR - new message received)
    updateChat: (
      state,
      action: PayloadAction<{
        chatId: string;
        updates: Partial<IChatMessageResponse>;
      }>
    ) => {
      const { chatId, updates } = action.payload;
      const index = state.chats.findIndex((c) => c.id === chatId);

      if (index !== -1) {
        state.chats[index] = { ...state.chats[index], ...updates };

        // Переместить чат в начало списка
        const chat = state.chats.splice(index, 1)[0];
        state.chats.unshift(chat);
      }
    },

    // Update last message (from SignalR)
    updateLastMessage: (
      state,
      action: PayloadAction<{
        chatId: string;
        lastMessage: IChatMessageResponse['lastMessage'];
        lastMessageTime: string;
      }>
    ) => {
      const { chatId, lastMessage, lastMessageTime } = action.payload;
      const chat = state.chats.find((c) => c.id === chatId);

      if (chat) {
        chat.lastMessage = lastMessage;
        chat.lastMessageTime = lastMessageTime;

        // Переместить в начало
        const index = state.chats.indexOf(chat);
        state.chats.splice(index, 1);
        state.chats.unshift(chat);
      }
    },

    // Increment unread count (from SignalR)
    incrementUnreadCount: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      const chat = state.chats.find((c) => c.id === chatId);
      if (chat) {
        chat.unreadCount += 1;
      }
    },

    // Reset unread count (when user opens chat)
    resetUnreadCount: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      const chat = state.chats.find((c) => c.id === chatId);
      if (chat) {
        chat.unreadCount = 0;
      }
    },

    // Add new chat (when created)
    addChat: (state, action: PayloadAction<IChatMessageResponse>) => {
      // Проверка на дубликат
      const exists = state.chats.some((c) => c.id === action.payload.id);
      if (!exists) {
        state.chats.unshift(action.payload);
      }
    },

    // Remove chat (when deleted or user excluded)
    removeChat: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      state.chats = state.chats.filter((c) => c.id !== chatId);
    },

    // Loading states
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },

    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
      state.loading = false;
    },

    // Clear all
    clearChats: () => initialState,
  },
});

export const {
  setChats,
  addChats,
  updateChat,
  updateLastMessage,
  incrementUnreadCount,
  resetUnreadCount,
  addChat,
  removeChat,
  setLoading,
  setError,
  clearChats,
} = chatSlice.actions;

export default chatSlice.reducer;
