import { useEffect, useRef, useState } from 'react';
import { Spin, Button } from 'antd';
import { DownOutlined } from '@ant-design/icons';
import MessageItem from './MessageItem';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import {
  selectMessagesLoading,
  selectAllChatMessages,
  selectChats,
} from '../../../../store/selectors';
import {
  setMessages,
  addMessagesToTop,
  setMessagesLoading
} from '../../../../store/messagesSlice';
import { resetUnreadCount } from '../../../../store/chatSlice';
import { useApiService } from '../../../../api/useApiService';
import { useMessageService } from '../../../../api/services/messageService';
import { useAuth } from '../../../../context/useAuth';
import { isSystemMessage } from '../../../../types/utilities/messageUtilities';

interface MessageListProps {
  chatId: string;
}

const PAGE_SIZE = 10;

interface MessageListProps {
  chatId: string;
}

export const MessageList: React.FC<MessageListProps> = ({ chatId }) => {
  const apiService = useApiService();
  const messageService = useMessageService(apiService);
  const { user } = useAuth();
  
  const dispatch = useAppDispatch();
  const messages = useAppSelector((state) => selectAllChatMessages(state, chatId));
  const loading = useAppSelector((state) => selectMessagesLoading(state, chatId));
  const chats = useAppSelector(selectChats);
  
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const topObserverRef = useRef<HTMLDivElement>(null);
  const bottomObserverRef = useRef<HTMLDivElement>(null);
  const scrollContainerRef = useRef<HTMLDivElement>(null);
  const unreadMessageRef = useRef<HTMLDivElement>(null);
  
  const [oldestMessageDate, setOldestMessageDate] = useState<Date | null>(null);
  const [newestMessageDate, setNewestMessageDate] = useState<Date | null>(null);
  const [hasMoreOlder, setHasMoreOlder] = useState(true);
  const [hasMoreNewer, setHasMoreNewer] = useState(false);
  const [initialLoadDone, setInitialLoadDone] = useState(false);
  const [newMessagesCount, setNewMessagesCount] = useState(0);
  const [isAtBottom, setIsAtBottom] = useState(true);

  const isLoadingRef = useRef(false);
  const renderCount = useRef(0);
  const prevLastMessageIdRef = useRef<string | null>(null);
  const markMessagesAsReadRef = useRef<(() => Promise<void>) | null>(null);
  renderCount.current++;

  // Keep ref up-to-date with latest closure so scroll handler never goes stale
  markMessagesAsReadRef.current = async () => {
    const lastMessage = messages[messages.length - 1];
    if (!lastMessage) return;
    const currentChat = chats.find(c => c.chat.id === chatId);
    if (!currentChat || currentChat.unreadCount === 0) return;
    dispatch(resetUnreadCount(currentChat.id));
    try {
      await messageService.readMessages({ maxMessageId: lastMessage.id });
    } catch (err) {
      console.error('Failed to mark messages as read:', err);
    }
  };

  const loadInitMessages = async () => {
    if (isLoadingRef.current) return;
    
    isLoadingRef.current = true;
    setInitialLoadDone(false);
    dispatch(setMessagesLoading({ chatId, loading: true }));

    try {
      const currentChat = chats.find(chat => chat.chat.id === chatId);
      const unreadCount = currentChat?.unreadCount || 0;
      

      const response = await messageService.getMessages(
        {
          chatId,
          isDescending: false,
          fromUnread: unreadCount > 0,
          loadLastMessages: unreadCount === 0,
          from: null
        }, 
        {
          PageNumber: 1,
          PageSize: PAGE_SIZE
        }
      );

      if (response.success && response.data) {
        const fetchedMessages = response.data.items.sort((a, b) => 
          a.createdAt.localeCompare(b.createdAt)
        );
        dispatch(setMessages({
          chatId,
          messages: fetchedMessages,
          hasMore: fetchedMessages.length >= PAGE_SIZE
        }));

        if (fetchedMessages.length > 0) {
          setOldestMessageDate(new Date(fetchedMessages[0].createdAt));
          setNewestMessageDate(new Date(fetchedMessages[fetchedMessages.length - 1].createdAt));
          setHasMoreOlder(fetchedMessages.length >= PAGE_SIZE);
        }

        setInitialLoadDone(true);
      }
    } catch (err) {
      console.error('❌ Load failed:', err);
    } finally {
      dispatch(setMessagesLoading({ chatId, loading: false }));
      isLoadingRef.current = false;
    }
  };

  // Загрузка при смене чата
  useEffect(() => {
    loadInitMessages();
  }, [chatId]);

  // Сброс счётчика и состояния при смене чата
  useEffect(() => {
    setNewMessagesCount(0);
    setIsAtBottom(true);
    prevLastMessageIdRef.current = null;
  }, [chatId]);

  // Слушатель скролла: сбрасываем счётчик когда пользователь долистал до низа
  useEffect(() => {
    const container = scrollContainerRef.current;
    if (!container) return;

    const handleScroll = () => {
      const atBottom = container.scrollHeight - container.scrollTop - container.clientHeight < 100;
      setIsAtBottom(atBottom);
      if (atBottom) {
        setNewMessagesCount(0);
        markMessagesAsReadRef.current?.();
      }
    };

    container.addEventListener('scroll', handleScroll, { passive: true });
    return () => container.removeEventListener('scroll', handleScroll);
  }, []);

  // Скролл к первому непрочитанному или в конец после загрузки
  useEffect(() => {
    if (!initialLoadDone || !messages.length) return;
    
    const currentChat = chats.find(c => c.chat.id === chatId);
    const unreadCount = currentChat?.unreadCount || 0;
    
    if (unreadCount > 0 && unreadMessageRef.current) {
      setTimeout(() => {
        unreadMessageRef.current?.scrollIntoView({
          behavior: 'auto',
          block: 'center'
        });
      }, 100);
    } else {
      setTimeout(() => {
        scrollToBottom('instant');
      }, 0);
    }
  }, [initialLoadDone]);

  useEffect(() => {
    if (!topObserverRef.current || !initialLoadDone || !hasMoreOlder) return;

    const loadOlderMessages = async () => {
      if (isLoadingRef.current || loading || !oldestMessageDate) return;
      
      isLoadingRef.current = true;
      dispatch(setMessagesLoading({ chatId, loading: true }));
      
      // Save scroll position
      const container = scrollContainerRef.current;
      const previousScrollHeight = container?.scrollHeight || 0;

      try {
        const response = await messageService.getMessages(
          {
            chatId,
            isDescending: true,
            from: oldestMessageDate,
            fromUnread: false,
            loadLastMessages: false
          },
          {
            PageNumber: 1,
            PageSize: PAGE_SIZE,
          }
        );

        if (response.success && response.data) {
          const fetchedMessages = response.data.items.sort((a, b) => 
            a.createdAt.localeCompare(b.createdAt)
          );
          
          if (fetchedMessages.length > 0) {
            dispatch(addMessagesToTop({
              chatId,
              messages: fetchedMessages,
              hasMore: fetchedMessages.length >= PAGE_SIZE,
            }));

            setOldestMessageDate(new Date(fetchedMessages[0].createdAt));
            setHasMoreOlder(fetchedMessages.length >= PAGE_SIZE);

            // Restore scroll position
            setTimeout(() => {
              if (container) {
                container.scrollTop = container.scrollHeight - previousScrollHeight;
              }
            }, 0);
          } else {
            setHasMoreOlder(false);
          }
        }
      } catch (error) {
        console.error('Failed to load older messages:', error);
      } finally {
        dispatch(setMessagesLoading({ chatId, loading: false }));
        isLoadingRef.current = false;
      }
    };

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadOlderMessages();
        }
      },
      { threshold: 0.1, rootMargin: '100px' }
    );

    observer.observe(topObserverRef.current);

    return () => observer.disconnect();
  }, [initialLoadDone, hasMoreOlder, oldestMessageDate, chatId]);

  // ✅ IntersectionObserver для загрузки новых сообщений
  useEffect(() => {
    if (!bottomObserverRef.current || !initialLoadDone || !hasMoreNewer) return;

    const loadNewerMessages = async () => {
      if (isLoadingRef.current || loading || !newestMessageDate) return;
      
      isLoadingRef.current = true;
      dispatch(setMessagesLoading({ chatId, loading: true }));

      try {
        const response = await messageService.getMessages(
          {
            chatId,
            isDescending: false,
            from: newestMessageDate,
            fromUnread: false,
            loadLastMessages: false
          },
          {
            PageNumber: 1,
            PageSize: PAGE_SIZE,
          }
        );

        if (response.success && response.data) {
          const fetchedMessages = response.data.items.sort((a, b) => 
            a.createdAt.localeCompare(b.createdAt)
          );
          
          if (fetchedMessages.length > 0) {
            dispatch(addMessagesToTop({
              chatId,
              messages: fetchedMessages,
              hasMore: fetchedMessages.length >= PAGE_SIZE,
            }));

            setNewestMessageDate(new Date(fetchedMessages[fetchedMessages.length - 1].createdAt));
            setHasMoreNewer(fetchedMessages.length >= PAGE_SIZE);
          } else {
            setHasMoreNewer(false);
          }
        }
      } catch (error) {
        console.error('Failed to load newer messages:', error);
      } finally {
        dispatch(setMessagesLoading({ chatId, loading: false }));
        isLoadingRef.current = false;
      }
    };

    // TODO: что это такое?
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadNewerMessages();
        }
      },
      { threshold: 0.1, rootMargin: '100px' }
    );

    observer.observe(bottomObserverRef.current);

    return () => observer.disconnect();
  }, [initialLoadDone, hasMoreNewer, newestMessageDate, chatId]);

  // При получении нового сообщения — скроллить вниз или показать кнопку
  useEffect(() => {
    if (!initialLoadDone) return;

    const container = scrollContainerRef.current;
    if (!container) return;

    const lastMessage = messages[messages.length - 1];
    const isNewMessageAppended = lastMessage?.id !== prevLastMessageIdRef.current;
    prevLastMessageIdRef.current = lastMessage?.id ?? null;

    // Игнорируем загрузку старых сообщений (пагинация вверх)
    if (!isNewMessageAppended) return;

    const isOwnMessage = lastMessage?.createdBy?.id === user?.id;
    const atBottom = container.scrollHeight - container.scrollTop - container.clientHeight < 100;

    if (atBottom || isOwnMessage) {
      scrollToBottom();
    } else {
      setNewMessagesCount((prev) => prev + 1);
    }
  }, [messages.length, initialLoadDone]);

  const scrollToBottom = (behavior: ScrollBehavior = 'smooth') => {
    messagesEndRef.current?.scrollIntoView({ behavior });
  };

  const handleScrollToBottom = () => {
    scrollToBottom();
    setNewMessagesCount(0);
    markMessagesAsReadRef.current?.();
  };

  if (messages.length === 0 && loading) {
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

  // Находим currentChat локально для определения непрочитанных
  const currentChat = chats.find(c => c.chat.id === chatId);
  const unreadCount = currentChat?.unreadCount || 0;
  const firstUnreadIndex = unreadCount > 0 
    ? messages.length - unreadCount 
    : -1;

  const newMessagesLabel =
    newMessagesCount > 10 ? '10+ новых' : `${newMessagesCount} ${newMessagesCount === 1 ? 'новое' : 'новых'}`;

  return (
    <div className="relative h-full">
      <div
        ref={scrollContainerRef}
        className="h-full overflow-y-auto bg-gray-50"
      >
        {/* Top loader */}
        {hasMoreOlder && (
          <div ref={topObserverRef} className="text-center py-4">
            {loading && <Spin />}
          </div>
        )}

        <div className="px-4 py-4 space-y-3">
          {messages.map((message, index) => {
            const prevMessage = index > 0 ? messages[index - 1] : null;
            const showAvatar =
              !prevMessage ||
              prevMessage.createdBy.id !== message.createdBy.id ||
              isSystemMessage(prevMessage);

            //const isFirstUnread = index === firstUnreadIndex;

            return (
              <div key={message.id}>
                {/* TODO: разобраться с этим */}
                {/* {isFirstUnread && (
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
                )} */}

                <MessageItem
                  message={message}
                  showAvatar={showAvatar}
                />
              </div>
            );
          })}
          <div ref={messagesEndRef} />
        </div>

        {/* Bottom loader */}
        {hasMoreNewer && (
          <div ref={bottomObserverRef} className="text-center py-4">
            {loading && <Spin />}
          </div>
        )}
      </div>

      {/* Scroll-to-bottom button */}
      {!isAtBottom && (
        <div className="absolute bottom-4 left-4 z-10">
          <div className="relative">
            <Button
              type="primary"
              shape="circle"
              icon={<DownOutlined />}
              onClick={handleScrollToBottom}
              className="shadow-lg"
            />
            {newMessagesCount > 0 && (
              <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full min-w-5 h-5 flex items-center justify-center px-1 pointer-events-none">
                {newMessagesCount > 99 ? '99+' : newMessagesCount}
              </span>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default MessageList;
