import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IChatParticipantResponse } from '../types/messaging';

interface ParticipantsState {
  participants: Record<string, IChatParticipantResponse[]>; // chatId -> participants[]
  loading: Record<string, boolean>; // chatId -> loading
}

const initialState: ParticipantsState = {
  participants: {},
  loading: {},
};

const participantsSlice = createSlice({
  name: 'participants',
  initialState,
  reducers: {
    // Set participants for chat
    setParticipants: (
      state,
      action: PayloadAction<{
        chatId: string;
        participants: IChatParticipantResponse[];
      }>
    ) => {
      const { chatId, participants } = action.payload;
      state.participants[chatId] = participants;
      state.loading[chatId] = false;
    },

    // Add participant (from SignalR - InviteUser)
    addParticipant: (
      state,
      action: PayloadAction<{
        chatId: string;
        participant: IChatParticipantResponse;
      }>
    ) => {
      const { chatId, participant } = action.payload;

      if (!state.participants[chatId]) {
        state.participants[chatId] = [];
      }

      // Проверка на дубликат
      const exists = state.participants[chatId].some(
        (p) => p.id === participant.id
      );
      if (!exists) {
        state.participants[chatId].push(participant);
      }
    },

    // Remove participant (from SignalR - ExcludeUser)
    removeParticipant: (
      state,
      action: PayloadAction<{ chatId: string; participantId: string }>
    ) => {
      const { chatId, participantId } = action.payload;

      if (state.participants[chatId]) {
        state.participants[chatId] = state.participants[chatId].filter(
          (p) => p.id !== participantId
        );
      }
    },

    // Loading
    setParticipantsLoading: (
      state,
      action: PayloadAction<{ chatId: string; loading: boolean }>
    ) => {
      const { chatId, loading } = action.payload;
      state.loading[chatId] = loading;
    },

    // Clear participants for chat
    clearParticipants: (state, action: PayloadAction<string>) => {
      const chatId = action.payload;
      delete state.participants[chatId];
      delete state.loading[chatId];
    },

    // Clear all
    clearAllParticipants: () => initialState,
  },
});

export const {
  setParticipants,
  addParticipant,
  removeParticipant,
  setParticipantsLoading,
  clearParticipants,
  clearAllParticipants,
} = participantsSlice.actions;

export default participantsSlice.reducer;
