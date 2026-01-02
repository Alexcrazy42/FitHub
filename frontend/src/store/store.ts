import { configureStore, combineReducers } from '@reduxjs/toolkit';
import { 
  persistStore, 
  persistReducer,
  FLUSH,
  REHYDRATE,
  PAUSE,
  PERSIST,
  PURGE,
  REGISTER,
} from 'redux-persist';
import storage from 'redux-persist/lib/storage'; // localStorage
// import storageSession from 'redux-persist/lib/storage/session'; // sessionStorage

import chatReducer from './chatSlice';
import messagesReducer from './messagesSlice';
import uiReducer from './uiSlice';
import participantsReducer from './participantsSlice';

// Конфигурация persist
const persistConfig = {
  key: 'root',
  version: 1,
  storage, // используем localStorage
  whitelist: ['chat', 'messages'], // Что сохранять (только чаты и сообщения)
  // blacklist: ['ui'], // Что НЕ сохранять (UI состояние не нужно)
};

// Объединяем все reducers
const rootReducer = combineReducers({
  chat: chatReducer,
  messages: messagesReducer,
  ui: uiReducer,
  participants: participantsReducer,
});

// Создаем persisted reducer
const persistedReducer = persistReducer(persistConfig, rootReducer);

// Создаем store с persisted reducer
export const store = configureStore({
  reducer: persistedReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        // Игнорируем actions от redux-persist
        ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
      },
    }),
});

// Создаем persistor
export const persistor = persistStore(store);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
