import { createContext, FC, ReactNode, useContext, useEffect, useState } from "react";
import { useAppDispatch } from "./store/hooks"
import { 
  HubConnection, 
  HubConnectionBuilder
} from '@microsoft/signalr';
import { API_URL_CLEAN } from "./api/useApiService";
import { setConnectionState, setUserTyping } from "./store/uiSlice";
import { ConnectionState, IMessageResponse } from "./types/messaging";
import { addMessage, deleteMessage, updateMessage } from "./store/messagesSlice";


interface SignalRContextType {
  connection: HubConnection | null;
  notifyTyping: (chatId: string) => Promise<void>;
}

const WebSocketContext = createContext<SignalRContextType | null>(null);


export const WebSocketProvider : FC<{ children: ReactNode }> = ({ children }) => {
    const dispatch = useAppDispatch();
    const [connection, setConnection] = useState<HubConnection | null>(null);

    useEffect(() => {
        const conn = new HubConnectionBuilder()
            .withUrl(`${API_URL_CLEAN}/chatHub`)
            .withAutomaticReconnect()
            .build();

        conn.on('UserTyping', (user: string, chatId: string) => {
          console.log(`from signalR: UserTyping user: ${user} chat: ${chatId}`)
          dispatch(setUserTyping({
            chatId: chatId,
            userId: user,
            userName: user,
            isTyping: true
          }));
        })

        conn.on('CreateMessage', (message: IMessageResponse) => {
          console.log(`CreateMessage, id=${message.id}`);
          dispatch(addMessage({
            chatId: message.chatId,
            message: message
          }));
        })

        conn.on('UpdateMessage', (message: IMessageResponse) => {
          console.log(`updatedMessage, id=${message.id}`);
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

        conn.start().then(() => {
          setConnection(conn);
        });
    }, [dispatch]);

    
    const notifyTyping = async (chatId: string) => {
      if (!connection) throw new Error('Not connected');
      await connection.invoke('NotifyTyping', chatId);
    };


    return (
      <WebSocketContext.Provider value={{ connection, notifyTyping }}>
        {children}
      </WebSocketContext.Provider>
    );
}


export const useSignalR = () => {
  const context = useContext(WebSocketContext);
  if (!context) throw new Error('useSignalR must be used within SignalRProvider');
  return context;
};