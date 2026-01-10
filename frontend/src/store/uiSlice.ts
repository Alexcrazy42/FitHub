import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { ConnectionState } from '../types/messaging';

interface TypingUser {
  userId: string;
  userName: string;
}

interface UIState {
  currentChatId: string | null;
  connectionState: ConnectionState;
  typingUsers: Record<string, TypingUser[]>; // chatId -> users[]
  sidebarCollapsed: boolean;
  replyingToMessage: Record<string, string | null>; // chatId -> messageId
  editingMessage: Record<string, string | null>; // chatId -> messageId
}

const initialState: UIState = {
  currentChatId: null,
  connectionState: ConnectionState.DISCONNECTED,
  typingUsers: {},
  sidebarCollapsed: false,
  replyingToMessage: {},
  editingMessage: {},
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    // Current chat
    setCurrentChatId: (state, action: PayloadAction<string | null>) => {
      state.currentChatId = action.payload;
    },

    // Connection state
    setConnectionState: (state, action: PayloadAction<ConnectionState>) => {
      state.connectionState = action.payload;
    },

    // Typing users
    setUserTyping: (
      state,
      action: PayloadAction<{
        chatId: string;
        userId: string;
        userName: string;
        isTyping: boolean;
      }>
    ) => {
      const { chatId, userId, userName, isTyping } = action.payload;

      if (!state.typingUsers[chatId]) {
        state.typingUsers[chatId] = [];
      }

      if (isTyping) {
        // Добавляем если еще нет
        const exists = state.typingUsers[chatId].some((u) => u.userId === userId);
        if (!exists) {
          state.typingUsers[chatId].push({ userId, userName });
        }
      } else {
        // Убираем
        state.typingUsers[chatId] = state.typingUsers[chatId].filter(
          (u) => u.userId !== userId
        );
      }
    },

    clearTyping: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      delete state.typingUsers[chatId];
    },

    // Sidebar
    toggleSidebar: (state) => {
      state.sidebarCollapsed = !state.sidebarCollapsed;
    },

    setSidebarCollapsed: (state, action: PayloadAction<boolean>) => {
      state.sidebarCollapsed = action.payload;
    },

    // Reply to message
    setReplyingToMessage: (
      state,
      action: PayloadAction<{ chatId: string; messageId: string | null }>
    ) => {
      const { chatId, messageId } = action.payload;
      state.replyingToMessage[chatId] = messageId;
    },

    cancelReply: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      state.replyingToMessage[chatId] = null;
    },

    // Edit message
    setEditingMessage: (
      state,
      action: PayloadAction<{ chatId: string; messageId: string | null }>
    ) => {
      const { chatId, messageId } = action.payload;
      state.editingMessage[chatId] = messageId;
    },

    cancelEdit: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      state.editingMessage[chatId] = null;
    },

    // Clear all UI state
    clearUIState: () => initialState,
  },
});

export const {
  setCurrentChatId,
  setConnectionState,
  setUserTyping,
  clearTyping,
  toggleSidebar,
  setSidebarCollapsed,
  setReplyingToMessage,
  cancelReply,
  setEditingMessage,
  cancelEdit,
  clearUIState,
} = uiSlice.actions;

export default uiSlice.reducer;
