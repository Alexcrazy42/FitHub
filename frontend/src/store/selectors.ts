import { RootState } from './store';
import { createSelector } from '@reduxjs/toolkit';

// LEARN
// зачем нужны селекторы:
    // 1. Инкапсуляция структуры стора: компонент не знает, что данные в state.chat.chats или state.messages.messages[chatId], он просто вызывает селектор.
    // 2. Переиспользование: один и тот же селектор можно использовать в разных компонентах.
    // 3. Упрощение рефакторинга: изменил структуру стора → меняешь селектор, а не каждый компонент.
    // 4. Производные данные: можно считать суммы, фильтровать, находить текущий чат и т.п., не дублируя эту логику в компонентах.


// LEARN: простые селекторы, где достается просто что либо (мемоизация не нужна)
// Chats selectors
export const selectChats = (state: RootState) => state.chat.chats;
export const selectChatsLoading = (state: RootState) => state.chat.loading;
export const selectChatsError = (state: RootState) => state.chat.error;
export const selectHasMoreChats = (state: RootState) => state.chat.hasMore;

// Current chat selector
export const selectCurrentChatId = (state: RootState) => state.ui.currentChatId;


// LEARN
// Мемоизированные селекторы (createSelector)
// createSelector из RTK (обертка над Reselect) делает мемоизацию: если входные аргументы не поменялись, результат берется из кеша, а не пересчитывается заново. Это важно для производных данных и производительности.


export const selectCurrentChat = createSelector(
  [selectChats, selectCurrentChatId],
  (chats, currentChatId) => {
    if (!currentChatId) return null;
    return chats.find((c) => c.chat.id === currentChatId) || null;
  }
);

// Messages selectors
export const selectAllMessages = (state: RootState) => state.messages.messages;

export const selectChatMessages = (chatId: string) =>
  createSelector([selectAllMessages], (messages) => messages[chatId] || []);

export const selectSendingMessages = (chatId: string) => (state: RootState) => state.messages.sendingMessages[chatId] || [];

export const selectAllChatMessages = (chatId: string) =>
  createSelector(
    [selectChatMessages(chatId), selectSendingMessages(chatId)],
    (messages, sendingMessages) => [...messages, ...sendingMessages]
  );

export const selectMessagesLoading = (chatId: string) =>
  (state: RootState) => state.messages.loading[chatId] || false;

export const selectHasMoreMessages = (chatId: string) =>
  (state: RootState) => state.messages.hasMore[chatId] || false;

// Typing selectors
export const selectTypingUsers = (chatId: string) =>
  (state: RootState) => state.ui.typingUsers[chatId] || [];

// Reply/Edit selectors
export const selectReplyingToMessage = (chatId: string) =>
  (state: RootState) => state.ui.replyingToMessage[chatId] || null;

export const selectEditingMessage = (chatId: string) =>
  (state: RootState) => state.ui.editingMessage[chatId] || null;

// Participants selectors
export const selectChatParticipants = (chatId: string) =>
  (state: RootState) => state.participants.participants[chatId] || [];

// Connection state
export const selectConnectionState = (state: RootState) =>
  state.ui.connectionState;

// Sidebar
export const selectSidebarCollapsed = (state: RootState) =>
  state.ui.sidebarCollapsed;




// LEARN
//   Как это работает пошагово
    // Ты вызываешь selectTotalUnreadCount(state).
    // createSelector внутри сначала вызывает все input‑селекторы: selectChats(state) → chats.
    // Берёт результаты (chats) и передаёт их в result‑функцию: (chats) => chats.reduce(...).
    // Возвращает результат reduce.
// Total unread count (для badge)
export const selectTotalUnreadCount = createSelector(
  [selectChats],
  (chats) => chats.reduce((total, chat) => total + chat.unreadCount, 0)
);
