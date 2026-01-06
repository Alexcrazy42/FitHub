import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IChatMessageResponse } from '../types/messaging';

interface ChatState {
  chats: IChatMessageResponse[];
  hasMore: boolean;
  loading: boolean;
  error: string | null;
}

const initialState: ChatState = {
  chats: [],
  hasMore: true,
  loading: false,
  error: null,
};

// LEARN:
// благодаря RTK есть функция createSlice, которая упрощает
// 1. не нужно вручную объявлять action type строки
// 2. Не нужно отдельно писать action creators (просто описал reducer setChats и RTK автоматом сгенерил action creator)
  // + он уже типизирован благодаря PayloadAction
// 3. Не нужен один большой switch-case reducer (вместо одного монолитного switch набор маленьких функций в reducers)
// 4. Можно "мутировать" state в теле reducer’а
  // В классическом Redux так было нельзя, приходилось делать копии: return { ...state, chats: action.payload, loading: false };
  // RTK под капотом использует Immer, и твои «мутации» превращаются в иммутабельные обновления.
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
      }>
    ) => {
      // Получаем существующие ID чатов
      const existingChatIds = new Set(state.chats.map(chat => chat.id));
      
      // Фильтруем новые чаты, оставляя только те, которых ещё нет
      const newChats = action.payload.chats.filter(
        chat => !existingChatIds.has(chat.id)
      );
      
      // Добавляем только новые чаты
      state.chats.push(...newChats);
      state.hasMore = action.payload.hasMore;
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
    updateLastMessage: (state, action) => {
      const { chatId, lastMessage, lastMessageTime } = action.payload;
      const index = state.chats.findIndex((c) => c.chat.id === chatId);

      if (index !== -1) {
        const chat = state.chats[index];

        // ✅ Создаём обновлённый чат
        const updatedChat = {
          ...chat,
          lastMessage,
          lastMessageTime,
          unreadCount: chat.unreadCount + 1,
        };

        // ✅ Создаём новый массив
        state.chats = [
          updatedChat,
          ...state.chats.slice(0, index),
          ...state.chats.slice(index + 1),
        ];
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