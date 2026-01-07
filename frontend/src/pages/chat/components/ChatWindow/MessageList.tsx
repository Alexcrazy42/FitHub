import React, { useEffect, useRef, useState } from 'react';
import { Spin } from 'antd';
import MessageItem from './MessageItem';
import { IMessageResponse } from '../../../../types/messaging';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { 
  selectMessagesLoading,
  selectCurrentChat,
  selectAllChatMessages, // использовать вместо prop messages
} from '../../../../store/selectors';
import { 
  setMessages, 
  addMessagesToTop, 
  setMessagesLoading 
} from '../../../../store/messagesSlice';
import { useApiService } from '../../../../api/useApiService';
import { useMessageService } from '../../../../api/services/messageService';

interface MessageListProps {
  chatId: string;
}

const PAGE_SIZE = 20;

export const MessageList: React.FC<MessageListProps> = ({ chatId }) => {
  const apiService = useApiService();
  const messageService = useMessageService(apiService);
  
  const dispatch = useAppDispatch();
  const currentChat = useAppSelector(selectCurrentChat);
  const messages = useAppSelector((state) => selectAllChatMessages(chatId)(state)); // из store
  const loading = useAppSelector(selectMessagesLoading(chatId));
  
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const topObserverRef = useRef<HTMLDivElement>(null);
  const bottomObserverRef = useRef<HTMLDivElement>(null);
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const unreadMessageRef = useRef<HTMLDivElement>(null);
  
  const [initialLoadDone, setInitialLoadDone] = useState(false);
  const [oldestMessageDate, setOldestMessageDate] = useState<Date | null>(null);
  const [newestMessageDate, setNewestMessageDate] = useState<Date | null>(null);
  const [hasMoreOlder, setHasMoreOlder] = useState(true);
  const [hasMoreNewer, setHasMoreNewer] = useState(false);

  const isLoadingRef = useRef(false);

  // Initial load
  useEffect(() => {
    if (!chatId || initialLoadDone) return;

    const loadInitialMessages = async () => {
      if (isLoadingRef.current) return;
      isLoadingRef.current = true;
      
      dispatch(setMessagesLoading({ chatId, loading: true }));
      
      try {
        const unreadCount = currentChat?.unreadCount || 0;

        const response = await messageService.getMessages(
          {
            chatId,
            isDescending: false,
            fromUnread: unreadCount > 0,
            from: unreadCount > 0 ? null : new Date(Date.now() - 155 * 60 * 1000)
          },
          {
            PageNumber: 1,
            PageSize: PAGE_SIZE + unreadCount,
          }
        );

        if (response.success && response.data) {
          const fetchedMessages = response.data.items;
          
          dispatch(setMessages({
            chatId,
            messages: fetchedMessages,
            hasMore: fetchedMessages.length >= PAGE_SIZE,
            nextCursor: undefined, // TODO: nextCursor может мне нехило помочь, надо его юзать
          }));

          if (fetchedMessages.length > 0) {
            setOldestMessageDate(new Date(fetchedMessages[0].createdAt));
            setNewestMessageDate(new Date(fetchedMessages[fetchedMessages.length - 1].createdAt));
            setHasMoreOlder(fetchedMessages.length >= PAGE_SIZE);
          }
          
          setInitialLoadDone(true);
        }
      }finally {
        dispatch(setMessagesLoading({ chatId, loading: false }));
        isLoadingRef.current = false;
      }
    };

    loadInitialMessages();
  }, [chatId]);

  // Scroll to unread after initial load
  // useEffect(() => {
  //   if (!initialLoadDone || !messages.length) return;
    
  //   const unreadCount = currentChat?.unreadCount || 0;
    
  //   if (unreadCount > 0 && unreadMessageRef.current) {
  //     setTimeout(() => {
  //       unreadMessageRef.current?.scrollIntoView({ 
  //         behavior: 'auto', 
  //         block: 'center' 
  //       });
  //     }, 100);
  //   } else {
  //     scrollToBottom();
  //   }
  // }, [initialLoadDone]);

  // // ✅ IntersectionObserver для загрузки старых сообщений
  // useEffect(() => {
  //   if (!topObserverRef.current || !initialLoadDone || !hasMoreOlder) return;

  //   const loadOlderMessages = async () => {
  //     if (isLoadingRef.current || loading || !oldestMessageDate) return;
      
  //     isLoadingRef.current = true;
  //     dispatch(setMessagesLoading({ chatId, loading: true }));
      
  //     const container = scrollContainerRef.current;
  //     const previousScrollHeight = container?.scrollHeight || 0;

  //     try {
  //       const response = await messageService.getMessages(
  //         {
  //           chatId,
  //           isDescending: true,
  //           from: oldestMessageDate,
  //           fromUnread: false
  //         },
  //         {
  //           PageNumber: 1,
  //           PageSize: PAGE_SIZE,
  //         }
  //       );

  //       if (response.success && response.data) {
  //         const fetchedMessages = response.data.items.reverse();
          
  //         if (fetchedMessages.length > 0) {
  //           dispatch(addMessagesToTop({
  //             chatId,
  //             messages: fetchedMessages,
  //             hasMore: fetchedMessages.length >= PAGE_SIZE,
  //           }));

  //           setOldestMessageDate(new Date(fetchedMessages[0].createdAt));
  //           setHasMoreOlder(fetchedMessages.length >= PAGE_SIZE);

  //           setTimeout(() => {
  //             if (container) {
  //               container.scrollTop = container.scrollHeight - previousScrollHeight;
  //             }
  //           }, 0);
  //         } else {
  //           setHasMoreOlder(false);
  //         }
  //       }
  //     } catch (error) {
  //       console.error('Failed to load older messages:', error);
  //     } finally {
  //       dispatch(setMessagesLoading({ chatId, loading: false }));
  //       isLoadingRef.current = false;
  //     }
  //   };

  //   const observer = new IntersectionObserver(
  //     (entries) => {
  //       if (entries[0].isIntersecting) {
  //         loadOlderMessages();
  //       }
  //     },
  //     { threshold: 0.1, rootMargin: '100px' }
  //   );

  //   observer.observe(topObserverRef.current);

  //   return () => observer.disconnect();
  // }, [initialLoadDone, hasMoreOlder, oldestMessageDate, chatId]);

  // // ✅ IntersectionObserver для загрузки новых сообщений
  // useEffect(() => {
  //   if (!bottomObserverRef.current || !initialLoadDone || !hasMoreNewer) return;

  //   const loadNewerMessages = async () => {
  //     if (isLoadingRef.current || loading || !newestMessageDate) return;
      
  //     isLoadingRef.current = true;
  //     dispatch(setMessagesLoading({ chatId, loading: true }));

  //     try {
  //       const response = await messageService.getMessages(
  //         {
  //           chatId,
  //           isDescending: false,
  //           from: newestMessageDate,
  //           fromUnread: false
  //         },
  //         {
  //           PageNumber: 1,
  //           PageSize: PAGE_SIZE,
  //         }
  //       );

  //       if (response.success && response.data) {
  //         const fetchedMessages = response.data.items;
          
  //         if (fetchedMessages.length > 0) {
  //           // ✅ Добавляем к существующим через action
  //           dispatch(addMessagesToTop({ // или создайте addMessagesToBottom
  //             chatId,
  //             messages: fetchedMessages,
  //             hasMore: fetchedMessages.length >= PAGE_SIZE,
  //           }));

  //           setNewestMessageDate(new Date(fetchedMessages[fetchedMessages.length - 1].createdAt));
  //           setHasMoreNewer(fetchedMessages.length >= PAGE_SIZE);
  //         } else {
  //           setHasMoreNewer(false);
  //         }
  //       }
  //     } catch (error) {
  //       console.error('Failed to load newer messages:', error);
  //     } finally {
  //       dispatch(setMessagesLoading({ chatId, loading: false }));
  //       isLoadingRef.current = false;
  //     }
  //   };

  //   const observer = new IntersectionObserver(
  //     (entries) => {
  //       if (entries[0].isIntersecting) {
  //         loadNewerMessages();
  //       }
  //     },
  //     { threshold: 0.1, rootMargin: '100px' }
  //   );

  //   observer.observe(bottomObserverRef.current);

  //   return () => observer.disconnect();
  // }, [initialLoadDone, hasMoreNewer, newestMessageDate, chatId]);

  // // Auto-scroll on new message
  // useEffect(() => {
  //   const container = scrollContainerRef.current;
  //   if (!container || !initialLoadDone) return;

  //   const isAtBottom = container.scrollHeight - container.scrollTop - container.clientHeight < 100;

  //   if (isAtBottom) {
  //     scrollToBottom();
  //   }
  // }, [messages.length, initialLoadDone]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  // // Reset state on chatId change
  // useEffect(() => {
  //   setInitialLoadDone(false);
  //   setOldestMessageDate(null);
  //   setNewestMessageDate(null);
  //   setHasMoreOlder(true);
  //   setHasMoreNewer(false);
  //   isLoadingRef.current = false;
  // }, [chatId]);

  if (!initialLoadDone && loading) {
    return (
      <div className="flex items-center justify-center h-full">
        <Spin size="large" />
      </div>
    );
  }

  if (messages.length === 0 && !loading) {
    return (
      <div className="flex items-center justify-center h-full text-gray-400">
        <div className="text-center">
          <div className="text-5xl mb-4">📭</div>
          <p className="text-lg">Пока нет сообщений</p>
          <p className="text-sm">Начните беседу!</p>
        </div>
      </div>
    );
  }

  const unreadCount = currentChat?.unreadCount || 0;
  const firstUnreadIndex = unreadCount > 0 
    ? messages.length - unreadCount 
    : -1;

  return (
    <div
      ref={scrollContainerRef}
      className="h-full overflow-y-auto bg-gray-50"
    >
      {hasMoreOlder && (
        <div ref={topObserverRef} className="text-center py-4">
          {loading && <Spin />}
        </div>
      )}

      <div className="px-4 py-4 space-y-3">
        {messages.map((message, index) => {
          const prevMessage = index > 0 ? messages[index - 1] : null;
          const showAvatar =
            !prevMessage || prevMessage.createdBy.id !== message.createdBy.id;

          const isFirstUnread = index === firstUnreadIndex;

          return (
            <div key={message.id}>
              {isFirstUnread && (
                <div 
                  ref={unreadMessageRef}
                  className="flex items-center my-4"
                >
                  <div className="flex-1 h-px bg-red-400" />
                  <span className="px-3 text-xs text-red-500 font-semibold">
                    Непрочитанные сообщения
                  </span>
                  <div className="flex-1 h-px bg-red-400" />
                </div>
              )}

              <MessageItem
                message={message}
                showAvatar={showAvatar}
              />
            </div>
          );
        })}
        <div ref={messagesEndRef} />
      </div>

      {hasMoreNewer && (
        <div ref={bottomObserverRef} className="text-center py-4">
          {loading && <Spin />}
        </div>
      )}
    </div>
  );
};
