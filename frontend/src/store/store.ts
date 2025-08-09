import { configureStore } from '@reduxjs/toolkit';
import { persistStore, persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage'; // использует localStorage
import { combineReducers } from 'redux';

// Пример редьюсера (позже заменишь на свои)
const rootReducer = combineReducers({
  // auth: authReducer,
  // user: userReducer,
});

const persistConfig = {
  key: 'root',
  storage,
  version: 123,
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

export const store = configureStore({
  reducer: persistedReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false, // нужно для работы с redux-persist
    }),
});

export const persistor = persistStore(store);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;