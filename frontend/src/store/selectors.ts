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


// ========================================
// Messages selectors
// ========================================

// Базовые селекторы (возвращают части state)
export const selectAllMessages = (state: RootState) => state.messages.messages;
export const selectAllSendingMessages = (state: RootState) => state.messages.sendingMessages;
export const selectMessagesLoadingState = (state: RootState) => state.messages.loading;
export const selectHasMoreMessagesState = (state: RootState) => state.messages.hasMore;

// ✅ Мемоизированные селекторы с параметром chatId
export const selectChatMessages = createSelector(
  [
    selectAllMessages,
    (_state: RootState, chatId: string) => chatId
  ],
  (allMessages, chatId) => allMessages[chatId] || []
);

export const selectSendingMessages = createSelector(
  [
    selectAllSendingMessages,
    (_state: RootState, chatId: string) => chatId
  ],
  (allSendingMessages, chatId) => allSendingMessages[chatId] || []
);

export const selectAllChatMessages = createSelector(
  [
    (state: RootState, chatId: string) => selectChatMessages(state, chatId),
    (state: RootState, chatId: string) => selectSendingMessages(state, chatId)
  ],
  (messages, sendingMessages) => {
    // ✅ Оптимизация: не создаем новый массив если sendingMessages пустой
    if (sendingMessages.length === 0) {
      return messages;
    }
    return [...messages, ...sendingMessages];
  }
);

export const selectMessagesLoading = createSelector(
  [
    selectMessagesLoadingState,
    (_state: RootState, chatId: string) => chatId
  ],
  (loadingState, chatId) => loadingState[chatId] || false
);

export const selectHasMoreMessages = createSelector(
  [
    selectHasMoreMessagesState,
    (_state: RootState, chatId: string) => chatId
  ],
  (hasMoreState, chatId) => hasMoreState[chatId] || false
);


// ========================================
// UI selectors
// ========================================

// Typing selectors
export const selectTypingUsers = createSelector(
  [
    (state: RootState) => state.ui.typingUsers,
    (_state: RootState, chatId: string) => chatId
  ],
  (typingUsers, chatId) => typingUsers[chatId] || []
);

// Reply/Edit selectors
export const selectReplyingToMessage = createSelector(
  [
    (state: RootState) => state.ui.replyingToMessage,
    (_state: RootState, chatId: string) => chatId
  ],
  (replyingToMessage, chatId) => replyingToMessage[chatId] || null
);

export const selectEditingMessage = createSelector(
  [
    (state: RootState) => state.ui.editingMessage,
    (_state: RootState, chatId: string) => chatId
  ],
  (editingMessage, chatId) => editingMessage[chatId] || null
);


// ========================================
// Participants selectors
// ========================================

export const selectChatParticipants = createSelector(
  [
    (state: RootState) => state.participants.participants,
    (_state: RootState, chatId: string) => chatId
  ],
  (participants, chatId) => participants[chatId] || []
);


// ========================================
// Connection & UI state
// ========================================

// Connection state
export const selectConnectionState = (state: RootState) =>
  state.ui.connectionState;

// Sidebar
export const selectSidebarCollapsed = (state: RootState) =>
  state.ui.sidebarCollapsed;


// ========================================
// Computed selectors
// ========================================

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
