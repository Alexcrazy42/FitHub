import React, { useState, useRef, useEffect } from 'react';
import { Input, Button, Tooltip, Popover } from 'antd';
import {
  SendOutlined,
  SmileOutlined,
  PaperClipOutlined,
  CloseOutlined,
} from '@ant-design/icons';
import EmojiPicker, { EmojiClickData, EmojiStyle, Theme } from 'emoji-picker-react';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import {
  selectReplyingToMessage,
  selectEditingMessage,
  selectAllChatMessages,
  selectCurrentChat,
} from '../../../../store/selectors';
import { cancelReply, cancelEdit } from '../../../../store/uiSlice';
import { addMessage, updateMessage } from '../../../../store/messagesSlice';
import { updateLastMessage } from '../../../../store/chatSlice';
import { getFirstName } from '../../mocks/fakeData';
import { debounce } from 'lodash';
import { useSignalR } from '../../../../WebSocketProvider';
import { TextAreaRef } from 'antd/es/input/TextArea';
import { ICreateMessageRequest } from '../../../../types/messaging';
import { useApiService } from '../../../../api/useApiService';
import { useMessageService } from '../../../../api/services/messageService';
import { toast } from 'react-toastify';

const { TextArea } = Input;

interface MessageInputProps {
  chatId: string;
}

const MessageInput: React.FC<MessageInputProps> = ({ chatId }) => {
  const apiService = useApiService();
  const messageService = useMessageService(apiService);

  const dispatch = useAppDispatch();
  const [messageText, setMessageText] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const textAreaRef = useRef<TextAreaRef | null>(null);
  const signalR = useSignalR();
  const currentChat = useAppSelector(selectCurrentChat);

  const typingTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  const TYPING_TIMEOUT = 1500;

  const replyingToMessageId = useAppSelector(selectReplyingToMessage(chatId));
  const editingMessageId = useAppSelector(selectEditingMessage(chatId));
  const messages = useAppSelector(selectAllChatMessages(chatId));

  const replyingToMessage = messages.find((m) => m.id === replyingToMessageId);
  const editingMessage = messages.find((m) => m.id === editingMessageId);

  useEffect(() => {
    if (editingMessage) {
      setMessageText(editingMessage.messageText);
      textAreaRef.current?.focus();
    }
  }, [editingMessage]);

  useEffect(() => {
    return () => {
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }
    };
  }, []);

  const notifyTyping = debounce((typing: boolean) => {
    if(currentChat?.chat.id) {
      if(typing) {
        signalR.notifyTyping(currentChat?.chat.id);
      } else {
        signalR.notifyStopTyping(currentChat?.chat.id);
      }
    }
  }, 300);

  const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    const value = e.target.value;
    setMessageText(value);

    // Если начал печатать
    if (!isTyping && value.length > 0) {
      setIsTyping(true);
      notifyTyping(true);
    }

    // Если полностью очистил поле
    if (value.length === 0) {
      setIsTyping(false);
      notifyTyping(false);
      
      // Очищаем таймер
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
        typingTimeoutRef.current = null;
      }
      return;
    }

    // Сбрасываем предыдущий таймер
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    // Ставим новый таймер
    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      notifyTyping(false);
    }, TYPING_TIMEOUT);
  };

  // Handle emoji click
  const handleEmojiClick = (emojiData: EmojiClickData) => {
    const emoji = emojiData.emoji;
    const textarea = textAreaRef.current?.resizableTextArea?.textArea;
    
    if (textarea) {
      const start = textarea.selectionStart;
      const end = textarea.selectionEnd;
      const text = messageText;
      const before = text.substring(0, start);
      const after = text.substring(end, text.length);
      
      const newText = before + emoji + after;
      setMessageText(newText);
      
      // Set cursor position after emoji
      setTimeout(() => {
        textarea.selectionStart = textarea.selectionEnd = start + emoji.length;
        textarea.focus();
      }, 0);
    } else {
      // Fallback - add to end
      setMessageText(messageText + emoji);
    }
    
    // Не закрываем picker, чтобы можно было добавить несколько эмодзи
    // setShowEmojiPicker(false);
  };

  const handleSend = async () => {
    if (!messageText.trim()) return;

    if (editingMessage) {
      // TODO: извенить сообщение
      // Edit message
      dispatch(
        updateMessage({
          chatId,
          messageId: editingMessage.id,
          updates: {
            messageText: messageText.trim(),
            updatedAt: new Date().toISOString(),
          },
        })
      );
      dispatch(cancelEdit(chatId));
    } else {
      // TODO: отправить сообщение

      const createMessageRequest : ICreateMessageRequest = {
        chatId: chatId,
        messageText: messageText.trim(),
        replyMessageId: replyingToMessage?.id ?? null,
        links: [],
        tags: [],
        photos: []
        // TODO: links, tags, photos
      }

      const response = await messageService.createMessage(createMessageRequest);

      if(response.success && response.data) {
        const newMessage = response.data;

        dispatch(addMessage({ chatId, message: newMessage }));

        // Update chat list
        dispatch(
          updateLastMessage({
            chatId,
            lastMessage: newMessage,
            lastMessageTime: newMessage.createdAt,
          })
        );

        if (replyingToMessage) {
          dispatch(cancelReply(chatId));
        }
      } else {
        toast.error(response.error?.detail ?? "Ошибка при отправке сообщения!")
      }
    }

    setMessageText('');
    setIsTyping(false);
    notifyTyping(false);
    setShowEmojiPicker(false);
    textAreaRef.current?.focus();
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleCancelReply = () => {
    dispatch(cancelReply(chatId));
  };

  const handleCancelEdit = () => {
    dispatch(cancelEdit(chatId));
    setMessageText('');
  };

  // Emoji picker content
  const emojiPickerContent = (
    <div className="emoji-picker-wrapper">
      <EmojiPicker
        onEmojiClick={handleEmojiClick}
        theme={Theme.LIGHT}
        width={350}
        height={400}
        searchPlaceHolder="Поиск эмодзи..."
        previewConfig={{
          showPreview: false,
        }}
        emojiStyle={EmojiStyle.NATIVE}
      />
    </div>
  );

  return (
    <div className="border-t border-gray-200 bg-white">
      {/* Reply/Edit preview */}
      {(replyingToMessage || editingMessage) && (
        <div className="px-4 py-2 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
          <div className="flex-1 min-w-0">
            {replyingToMessage && (
              <>
                <div className="text-sm font-semibold text-blue-600">
                  Ответ на сообщение {getFirstName(replyingToMessage.createdBy)}
                </div>
                <div className="text-sm text-gray-600 truncate">
                  {replyingToMessage.messageText}
                </div>
              </>
            )}
            {editingMessage && (
              <>
                <div className="text-sm font-semibold text-orange-600">
                  Редактирование сообщения
                </div>
                <div className="text-sm text-gray-600 truncate">
                  {editingMessage.messageText}
                </div>
              </>
            )}
          </div>
          <Button
            type="text"
            size="small"
            icon={<CloseOutlined />}
            onClick={replyingToMessage ? handleCancelReply : handleCancelEdit}
          />
        </div>
      )}

      {/* Input area */}
      <div className="p-4 flex items-end gap-2">
        {/* Attachments button */}
        <Tooltip title="Прикрепить файл">
          <Button
            type="text"
            icon={<PaperClipOutlined className="text-xl" />}
            className="flex-shrink-0"
            onClick={() => console.log('TODO: Attach file')}
          />
        </Tooltip>

        {/* Text input */}
        <TextArea
          ref={textAreaRef}
          value={messageText}
          onChange={handleInputChange}
          onKeyPress={handleKeyPress}
          placeholder="Введите сообщение..."
          autoSize={{ minRows: 1, maxRows: 4 }}
          className="flex-1"
        />

        {/* Emoji button with popover */}
        <Popover
          content={emojiPickerContent}
          trigger="click"
          open={showEmojiPicker}
          onOpenChange={setShowEmojiPicker}
          placement="topRight"
          overlayClassName="emoji-popover"
        >
          <Tooltip title="Эмодзи">
            <Button
              type="text"
              icon={<SmileOutlined className="text-xl" />}
              className="flex-shrink-0"
            />
          </Tooltip>
        </Popover>

        {/* Send button */}
        <Button
          type="primary"
          icon={<SendOutlined />}
          onClick={handleSend}
          disabled={!messageText.trim()}
          className="flex-shrink-0"
        >
          Отправить
        </Button>
      </div>
    </div>
  );
};

export default MessageInput;
