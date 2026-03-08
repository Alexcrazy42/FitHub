import { createContext, FC, ReactNode, useContext, useEffect, useRef, useState } from "react";
import { useAppDispatch } from "./store/hooks"
import { 
  HubConnection, 
  HubConnectionBuilder,
  HubConnectionState
} from '@microsoft/signalr';
import { API_URL_CLEAN } from "./api/useApiService";
import { setConnectionState, setUserTyping } from "./store/uiSlice";
import { ConnectionState, IMessageResponse } from "./types/messaging";
import { addMessage, deleteMessage, updateMessage } from "./store/messagesSlice";
import { updateLastMessage } from "./store/chatSlice";
import { useAuth } from "./context/useAuth";


interface SignalRContextType {
  connection: HubConnection | null;
  notifyTyping: (chatId: string) => Promise<void>;
  notifyStopTyping: (chatId: string) => Promise<void>;
}

const WebSocketContext = createContext<SignalRContextType | null>(null);


export const WebSocketProvider : FC<{ children: ReactNode }> = ({ children }) => {
    const dispatch = useAppDispatch();
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const {user} = useAuth();
    const userRef = useRef(user);
    useEffect(() => { userRef.current = user; }, [user]);

    const retryTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    useEffect(() => {
        let destroyed = false;

        const conn = new HubConnectionBuilder()
            .withUrl(`${API_URL_CLEAN}/chatHub`)
            .withAutomaticReconnect()
            .build();

        conn.on('UserTyping', (userId: string, userName: string, chatId: string) => {
          console.log(`from signalR: UserTyping user: ${userName} chat: ${chatId}`)
          dispatch(setUserTyping({
            chatId: chatId,
            userId: userId,
            userName: userName,
            isTyping: true
          }));
        })

        conn.on('UserStopTyping', (userId: string, userName: string, chatId: string) => {
          console.log(`from signalR: UserStopTyping user: ${userName} chat: ${chatId}`)
          dispatch(setUserTyping({
            chatId: chatId,
            userId: userId,
            userName: userName,
            isTyping: false
          }));
        })

        conn.on('CreateMessage', (message: IMessageResponse) => {
          dispatch(addMessage({
            chatId: message.chatId,
            message: message
          }));
          dispatch(
            updateLastMessage({
              chatId: message.chatId,
              lastMessage: message,
              lastMessageTime: message.createdAt,
              needIncrement: message.createdBy.id !== userRef.current?.id
            })
          );
          
        })

        conn.on('UpdateMessage', (message: IMessageResponse) => {
          dispatch(updateMessage({
            chatId: message.chatId,
            messageId: message.id,
            updates: message
          }));
        })

        conn.on('MessageDeleted', (chatId: string, messageId: string) => {
          dispatch(deleteMessage({
            chatId,
            messageId
          }))
        })

        conn.onreconnecting(() => {
          dispatch(setConnectionState(ConnectionState.RECONNECTING));
        });

        conn.onreconnected(() => {
          dispatch(setConnectionState(ConnectionState.CONNECTED));
        });

        const retryDelays = [2000, 5000, 10000, 20000];
        let attempt = 0;

        const tryConnect = async () => {
          try {
            await conn.start();
            if (destroyed) return;
            setConnection(conn);
            dispatch(setConnectionState(ConnectionState.CONNECTED));
          } catch {
            if (destroyed) return;
            const delay = retryDelays[Math.min(attempt, retryDelays.length - 1)];
            attempt++;
            console.warn(`SignalR: initial connection failed, retrying in ${delay}ms (attempt ${attempt})`);
            retryTimerRef.current = setTimeout(tryConnect, delay);
          }
        };

        dispatch(setConnectionState(ConnectionState.RECONNECTING));
        tryConnect();

        return () => {
          destroyed = true;
          if (retryTimerRef.current) clearTimeout(retryTimerRef.current);
          conn.off('UserTyping');
          conn.off('UserStopTyping');
          conn.off('CreateMessage');
          conn.off('UpdateMessage');
          conn.off('MessageDeleted');
        };
    }, [dispatch]);

    
    const notifyTyping = async (chatId: string) => {
      if (!connection || connection.state !== HubConnectionState.Connected) {
        console.warn('SignalR not connected, skipping notifyTyping');
        return; // Просто выходим без ошибки
      }
      
      try {
        await connection.invoke('NotifyTyping', chatId);
      } catch (error) {
        console.error('Failed to notify typing:', error);
      }
    };

    const notifyStopTyping = async (chatId: string) => {
      if (!connection || connection.state !== HubConnectionState.Connected) {
        console.warn('SignalR not connected, skipping notifyStopTyping');
        return;
      }
      
      try {
        await connection.invoke('NotifyStopTyping', chatId);
      } catch (error) {
        console.error('Failed to notify stop typing:', error);
      }
    };


    return (
      <WebSocketContext.Provider value={{ connection, notifyTyping, notifyStopTyping }}>
        {children}
      </WebSocketContext.Provider>
    );
}


export const useSignalR = () => {
  const context = useContext(WebSocketContext);
  if (!context) throw new Error('useSignalR must be used within SignalRProvider');
  return context;
};