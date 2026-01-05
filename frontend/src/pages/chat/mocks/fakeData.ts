import { 
  IChatMessageResponse, 
  IMessageResponse, 
  IChatResponse,
  IChatParticipantResponse,
  ChatType
} from '../../../types/messaging';
import { UserResponse } from '../../../types/auth';

// Расширенный список пользователей
export const fakeUsers: UserResponse[] = [
  {
    id: 'user-1',
    surname: 'Иванов',
    name: 'Иван',
    email: 'ivan@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-15'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-2',
    surname: 'Петрова',
    name: 'Мария',
    email: 'maria@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-10'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-3',
    surname: 'Сидоров',
    name: 'Алексей',
    email: 'alex@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-05'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-4',
    surname: 'Смирнова',
    name: 'Анна',
    email: 'anna@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-20'),
    roleNames: ['GymAdmin'],
  },
  {
    id: 'user-5',
    surname: 'Козлов',
    name: 'Дмитрий',
    email: 'dmitry@example.com',
    isActive: true,
    startActiveAt: new Date('2024-04-01'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-6',
    surname: 'Соколова',
    name: 'Елена',
    email: 'elena@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-15'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-7',
    surname: 'Морозов',
    name: 'Михаил',
    email: 'mikhail@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-20'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-8',
    surname: 'Волкова',
    name: 'Ольга',
    email: 'olga@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-10'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-9',
    surname: 'Новиков',
    name: 'Андрей',
    email: 'andrey@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-25'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-10',
    surname: 'Федорова',
    name: 'Екатерина',
    email: 'ekaterina@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-15'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-11',
    surname: 'Попов',
    name: 'Сергей',
    email: 'sergey@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-25'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-12',
    surname: 'Лебедева',
    name: 'Наталья',
    email: 'natalya@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-20'),
    roleNames: ['GymAdmin'],
  },
  {
    id: 'user-13',
    surname: 'Васильев',
    name: 'Владимир',
    email: 'vladimir@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-10'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-14',
    surname: 'Романова',
    name: 'Татьяна',
    email: 'tatyana@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-30'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-15',
    surname: 'Кузнецов',
    name: 'Николай',
    email: 'nikolay@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-05'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-16',
    surname: 'Павлова',
    name: 'Дарья',
    email: 'darya@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-25'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-17',
    surname: 'Захаров',
    name: 'Артём',
    email: 'artem@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-05'),
    roleNames: ['Trainer'],
  },
  {
    id: 'user-18',
    surname: 'Григорьева',
    name: 'София',
    email: 'sofia@example.com',
    isActive: true,
    startActiveAt: new Date('2024-02-12'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-19',
    surname: 'Макаров',
    name: 'Максим',
    email: 'maxim@example.com',
    isActive: true,
    startActiveAt: new Date('2024-03-08'),
    roleNames: ['GymVisitor'],
  },
  {
    id: 'user-20',
    surname: 'Степанова',
    name: 'Вера',
    email: 'vera@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-18'),
    roleNames: ['GymAdmin'],
  },
  {
    id: 'current-user',
    surname: 'Тестов',
    name: 'Тест',
    email: 'me@example.com',
    isActive: true,
    startActiveAt: new Date('2024-01-01'),
    roleNames: ['CmsAdmin'],
  },
];

// Current user
export const currentUser = fakeUsers[20];

// Helper functions
export const getFullName = (user: UserResponse): string => {
  return `${user.name} ${user.surname}`;
};

export const getFirstName = (user: UserResponse): string => {
  return user.name;
};

// Fake messages helper
const createMessage = (
  id: string,
  chatId: string,
  text: string,
  authorId: string,
  createdAt: Date,
  replyMessage?: IMessageResponse
): IMessageResponse => {
  const author = fakeUsers.find((u) => u.id === authorId)!;
  
  return {
    id,
    chatId,
    messageText: text,
    replyMessage: replyMessage || null,
    forwardedMessage: null,
    attachments: [],
    createdAt: createdAt.toISOString(),
    createdBy: author,
    updatedAt: createdAt.toISOString(),
    updatedBy: author,
  };
};

// Chat messages
const chat1Messages: IMessageResponse[] = [
  createMessage('msg-1-1', 'chat-1', 'Привет! Как дела?', 'user-2', new Date(Date.now() - 3600000 * 2)),
  createMessage('msg-1-2', 'chat-1', 'Привет! Все отлично, спасибо! А у тебя?', 'current-user', new Date(Date.now() - 3600000 * 2 + 60000)),
  createMessage('msg-1-3', 'chat-1', 'Тоже хорошо! Готовишься к тренировке завтра?', 'user-2', new Date(Date.now() - 3600000 * 2 + 120000)),
  createMessage('msg-1-4', 'chat-1', 'Да, уже почти все готово. Осталось только программу составить', 'current-user', new Date(Date.now() - 3600000 * 1)),
  createMessage('msg-1-5', 'chat-1', 'Отлично! Если нужна помощь - пиши', 'user-2', new Date(Date.now() - 60000 * 5)),
];

const chat2Messages: IMessageResponse[] = [
  createMessage('msg-2-1', 'chat-2', 'Добрый день! Не могли бы вы помочь с техникой упражнений?', 'current-user', new Date(Date.now() - 86400000)),
  createMessage('msg-2-2', 'chat-2', 'Конечно! Какие именно упражнения вас интересуют?', 'user-3', new Date(Date.now() - 86400000 + 300000)),
  createMessage('msg-2-3', 'chat-2', 'Приседания со штангой, хочу убедиться что делаю правильно', 'current-user', new Date(Date.now() - 86400000 + 600000)),
  createMessage('msg-2-4', 'chat-2', 'Отлично! Давайте встретимся завтра в зале, я покажу', 'user-3', new Date(Date.now() - 3600000 * 2)),
];

const chat3Messages: IMessageResponse[] = [
  createMessage('msg-3-1', 'chat-3', 'Всем привет! Напоминаю, что завтра спортзал работает до 20:00', 'user-4', new Date(Date.now() - 7200000)),
  createMessage('msg-3-2', 'chat-3', 'Спасибо за напоминание!', 'user-1', new Date(Date.now() - 7200000 + 120000)),
  createMessage('msg-3-3', 'chat-3', 'Понял, учту', 'user-5', new Date(Date.now() - 7200000 + 180000)),
  createMessage('msg-3-4', 'chat-3', 'А послезавтра режим работы стандартный?', 'current-user', new Date(Date.now() - 7200000 + 240000)),
  createMessage('msg-3-5', 'chat-3', 'Да, с 8:00 до 22:00 как обычно', 'user-4', new Date(Date.now() - 300000)),
];

const chat4Messages: IMessageResponse[] = [
  createMessage('msg-4-1', 'chat-4', 'Здравствуйте! Хочу продлить абонемент', 'current-user', new Date(Date.now() - 43200000)),
  createMessage('msg-4-2', 'chat-4', 'Добрый день! Какой абонемент вас интересует?', 'user-4', new Date(Date.now() - 43200000 + 600000)),
  createMessage('msg-4-3', 'chat-4', 'На 3 месяца, безлимитный', 'current-user', new Date(Date.now() - 43200000 + 900000)),
];

const chat5Messages: IMessageResponse[] = [
  createMessage('msg-5-1', 'chat-5', 'Не забудь взять с собой полотенце на тренировку!', 'user-5', new Date(Date.now() - 172800000)),
  createMessage('msg-5-2', 'chat-5', 'Хорошо, спасибо!', 'current-user', new Date(Date.now() - 172800000 + 300000)),
];

const chat6Messages: IMessageResponse[] = [
  createMessage('msg-6-1', 'chat-6', 'Подскажите, пожалуйста, какие у вас групповые занятия?', 'current-user', new Date(Date.now() - 259200000)),
  createMessage('msg-6-2', 'chat-6', 'У нас йога, пилатес, зумба и функциональный тренинг', 'user-6', new Date(Date.now() - 259200000 + 180000)),
];

const chat7Messages: IMessageResponse[] = [
  createMessage('msg-7-1', 'chat-7', 'Запишите меня на персональную тренировку, пожалуйста', 'user-8', new Date(Date.now() - 432000000)),
  createMessage('msg-7-2', 'chat-7', 'Когда вам удобно?', 'current-user', new Date(Date.now() - 432000000 + 120000)),
  createMessage('msg-7-3', 'chat-7', 'Завтра в 18:00', 'user-8', new Date(Date.now() - 432000000 + 240000)),
];

const chat8Messages: IMessageResponse[] = [
  createMessage('msg-8-1', 'chat-8', 'Михаил, можете составить программу тренировок для набора массы?', 'current-user', new Date(Date.now() - 518400000)),
  createMessage('msg-8-2', 'chat-8', 'Конечно! Встретимся в зале и обсудим детали', 'user-7', new Date(Date.now() - 518400000 + 360000)),
];

const chat9Messages: IMessageResponse[] = [
  createMessage('msg-9-1', 'chat-9', 'Когда следующее занятие по йоге?', 'user-10', new Date(Date.now() - 604800000)),
  createMessage('msg-9-2', 'chat-9', 'В среду в 19:00', 'current-user', new Date(Date.now() - 604800000 + 120000)),
];

const chat10Messages: IMessageResponse[] = [
  createMessage('msg-10-1', 'chat-10', 'Добрый день! Есть ли у вас свободные шкафчики?', 'user-11', new Date(Date.now() - 691200000)),
];

const chat11Messages: IMessageResponse[] = [
  createMessage('msg-11-1', 'chat-11', 'Андрей, подскажите упражнения для спины', 'current-user', new Date(Date.now() - 777600000)),
  createMessage('msg-11-2', 'chat-11', 'Подтягивания, тяга штанги и становая - базовые упражнения', 'user-9', new Date(Date.now() - 777600000 + 240000)),
];

const chat12Messages: IMessageResponse[] = [
  createMessage('msg-12-1', 'chat-12', 'Можно перенести тренировку на другой день?', 'user-14', new Date(Date.now() - 864000000)),
  createMessage('msg-12-2', 'chat-12', 'Да, конечно. На какой день?', 'current-user', new Date(Date.now() - 864000000 + 180000)),
];

const chat13Messages: IMessageResponse[] = [
  createMessage('msg-13-1', 'chat-13', 'Владимир, какие добавки вы рекомендуете?', 'current-user', new Date(Date.now() - 950400000)),
  createMessage('msg-13-2', 'chat-13', 'Протеин, креатин и витамины - это основа', 'user-13', new Date(Date.now() - 950400000 + 300000)),
];

const chat14Messages: IMessageResponse[] = [
  createMessage('msg-14-1', 'chat-14', 'У вас есть детские секции?', 'user-16', new Date(Date.now() - 1036800000)),
  createMessage('msg-14-2', 'chat-14', 'Да, с 10 лет. Детский фитнес по вторникам и четвергам', 'current-user', new Date(Date.now() - 1036800000 + 240000)),
];

const chat15Messages: IMessageResponse[] = [
  createMessage('msg-15-1', 'chat-15', 'Артём, можно консультацию по питанию?', 'current-user', new Date(Date.now() - 1123200000)),
  createMessage('msg-15-2', 'chat-15', 'Да, записывайтесь. У меня есть время в пятницу', 'user-17', new Date(Date.now() - 1123200000 + 420000)),
];

const chat16Messages: IMessageResponse[] = [
  createMessage('msg-16-1', 'chat-16', 'Спасибо за тренировку! Было очень продуктивно 💪', 'user-18', new Date(Date.now() - 1209600000)),
];

const chat17Messages: IMessageResponse[] = [
  createMessage('msg-17-1', 'chat-17', 'Привет! Хочу начать заниматься боксом', 'user-19', new Date(Date.now() - 1296000000)),
  createMessage('msg-17-2', 'chat-17', 'Отлично! Приходите на пробное занятие', 'current-user', new Date(Date.now() - 1296000000 + 180000)),
];

const chat18Messages: IMessageResponse[] = [
  createMessage('msg-18-1', 'chat-18', 'Вера, нужно обновить расписание на сайте', 'current-user', new Date(Date.now() - 1382400000)),
  createMessage('msg-18-2', 'chat-18', 'Хорошо, займусь этим сегодня', 'user-20', new Date(Date.now() - 1382400000 + 120000)),
];

const chat19Messages: IMessageResponse[] = [
  createMessage('msg-19-1', 'chat-19', 'Николай, не забудьте про завтрашнюю тренировку', 'current-user', new Date(Date.now() - 1468800000)),
];

const chat20Messages: IMessageResponse[] = [
  createMessage('msg-20-1', 'chat-20', 'Всем привет! Кто идет на утреннюю пробежку?', 'user-9', new Date(Date.now() - 1555200000)),
  createMessage('msg-20-2', 'chat-20', 'Я пойду!', 'user-7', new Date(Date.now() - 1555200000 + 60000)),
  createMessage('msg-20-3', 'chat-20', 'И я тоже', 'user-13', new Date(Date.now() - 1555200000 + 120000)),
];

// Fake chat participants
const createParticipants = (userIds: string[], chatId: string): IChatParticipantResponse[] => {
  return userIds.map((userId, index) => ({
    id: `participant-${chatId}-${index}`,
    chatId,
    user: fakeUsers.find((u) => u.id === userId)!,
    joinedAt: new Date(Date.now() - 86400000 * 30).toISOString(),
  }));
};

// Fake chats
const createChatResponse = (
  id: string,
  type: ChatType,
  participantIds: string[]
): IChatResponse => ({
  id,
  type,
  participants: createParticipants(participantIds, id),
});

// Fake chat list (20+ чатов)
export const fakeChatList: IChatMessageResponse[] = [
  {
    id: 'chat-1',
    chat: createChatResponse('chat-1', ChatType.OneToOne, ['current-user', 'user-2']),
    lastMessage: chat1Messages[chat1Messages.length - 1],
    unreadCount: 2,
    lastMessageTime: chat1Messages[chat1Messages.length - 1].createdAt,
  },
  {
    id: 'chat-2',
    chat: createChatResponse('chat-2', ChatType.OneToOne, ['current-user', 'user-3']),
    lastMessage: chat2Messages[chat2Messages.length - 1],
    unreadCount: 1,
    lastMessageTime: chat2Messages[chat2Messages.length - 1].createdAt,
  },
  {
    id: 'chat-3',
    chat: createChatResponse('chat-3', ChatType.Group, ['current-user', 'user-1', 'user-4', 'user-5']),
    lastMessage: chat3Messages[chat3Messages.length - 1],
    unreadCount: 5,
    lastMessageTime: chat3Messages[chat3Messages.length - 1].createdAt,
  },
  {
    id: 'chat-4',
    chat: createChatResponse('chat-4', ChatType.OneToOne, ['current-user', 'user-4']),
    lastMessage: chat4Messages[chat4Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat4Messages[chat4Messages.length - 1].createdAt,
  },
  {
    id: 'chat-5',
    chat: createChatResponse('chat-5', ChatType.OneToOne, ['current-user', 'user-5']),
    lastMessage: chat5Messages[chat5Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat5Messages[chat5Messages.length - 1].createdAt,
  },
  {
    id: 'chat-6',
    chat: createChatResponse('chat-6', ChatType.OneToOne, ['current-user', 'user-6']),
    lastMessage: chat6Messages[chat6Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat6Messages[chat6Messages.length - 1].createdAt,
  },
  {
    id: 'chat-7',
    chat: createChatResponse('chat-7', ChatType.OneToOne, ['current-user', 'user-8']),
    lastMessage: chat7Messages[chat7Messages.length - 1],
    unreadCount: 1,
    lastMessageTime: chat7Messages[chat7Messages.length - 1].createdAt,
  },
  {
    id: 'chat-8',
    chat: createChatResponse('chat-8', ChatType.OneToOne, ['current-user', 'user-7']),
    lastMessage: chat8Messages[chat8Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat8Messages[chat8Messages.length - 1].createdAt,
  },
  {
    id: 'chat-9',
    chat: createChatResponse('chat-9', ChatType.OneToOne, ['current-user', 'user-10']),
    lastMessage: chat9Messages[chat9Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat9Messages[chat9Messages.length - 1].createdAt,
  },
  {
    id: 'chat-10',
    chat: createChatResponse('chat-10', ChatType.OneToOne, ['current-user', 'user-11']),
    lastMessage: chat10Messages[chat10Messages.length - 1],
    unreadCount: 1,
    lastMessageTime: chat10Messages[chat10Messages.length - 1].createdAt,
  },
  {
    id: 'chat-11',
    chat: createChatResponse('chat-11', ChatType.OneToOne, ['current-user', 'user-9']),
    lastMessage: chat11Messages[chat11Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat11Messages[chat11Messages.length - 1].createdAt,
  },
  {
    id: 'chat-12',
    chat: createChatResponse('chat-12', ChatType.OneToOne, ['current-user', 'user-14']),
    lastMessage: chat12Messages[chat12Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat12Messages[chat12Messages.length - 1].createdAt,
  },
  {
    id: 'chat-13',
    chat: createChatResponse('chat-13', ChatType.OneToOne, ['current-user', 'user-13']),
    lastMessage: chat13Messages[chat13Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat13Messages[chat13Messages.length - 1].createdAt,
  },
  {
    id: 'chat-14',
    chat: createChatResponse('chat-14', ChatType.OneToOne, ['current-user', 'user-16']),
    lastMessage: chat14Messages[chat14Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat14Messages[chat14Messages.length - 1].createdAt,
  },
  {
    id: 'chat-15',
    chat: createChatResponse('chat-15', ChatType.OneToOne, ['current-user', 'user-17']),
    lastMessage: chat15Messages[chat15Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat15Messages[chat15Messages.length - 1].createdAt,
  },
  {
    id: 'chat-16',
    chat: createChatResponse('chat-16', ChatType.OneToOne, ['current-user', 'user-18']),
    lastMessage: chat16Messages[chat16Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat16Messages[chat16Messages.length - 1].createdAt,
  },
  {
    id: 'chat-17',
    chat: createChatResponse('chat-17', ChatType.OneToOne, ['current-user', 'user-19']),
    lastMessage: chat17Messages[chat17Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat17Messages[chat17Messages.length - 1].createdAt,
  },
  {
    id: 'chat-18',
    chat: createChatResponse('chat-18', ChatType.OneToOne, ['current-user', 'user-20']),
    lastMessage: chat18Messages[chat18Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat18Messages[chat18Messages.length - 1].createdAt,
  },
  {
    id: 'chat-19',
    chat: createChatResponse('chat-19', ChatType.OneToOne, ['current-user', 'user-15']),
    lastMessage: chat19Messages[chat19Messages.length - 1],
    unreadCount: 0,
    lastMessageTime: chat19Messages[chat19Messages.length - 1].createdAt,
  },
  {
    id: 'chat-20',
    chat: createChatResponse('chat-20', ChatType.Group, ['current-user', 'user-7', 'user-9', 'user-13', 'user-17']),
    lastMessage: chat20Messages[chat20Messages.length - 1],
    unreadCount: 3,
    lastMessageTime: chat20Messages[chat20Messages.length - 1].createdAt,
  },
];

// All messages mapped by chatId
export const fakeMessages: Record<string, IMessageResponse[]> = {
  'chat-1': chat1Messages,
  'chat-2': chat2Messages,
  'chat-3': chat3Messages,
  'chat-4': chat4Messages,
  'chat-5': chat5Messages,
  'chat-6': chat6Messages,
  'chat-7': chat7Messages,
  'chat-8': chat8Messages,
  'chat-9': chat9Messages,
  'chat-10': chat10Messages,
  'chat-11': chat11Messages,
  'chat-12': chat12Messages,
  'chat-13': chat13Messages,
  'chat-14': chat14Messages,
  'chat-15': chat15Messages,
  'chat-16': chat16Messages,
  'chat-17': chat17Messages,
  'chat-18': chat18Messages,
  'chat-19': chat19Messages,
  'chat-20': chat20Messages,
};

// Helper function to generate more fake messages
export const generateFakeMessage = (
  chatId: string,
  text: string,
  isMyMessage: boolean = false
): IMessageResponse => {
  const chat = fakeChatList.find((c) => c.id === chatId);
  if (!chat) throw new Error('Chat not found');

  const authorId = isMyMessage
    ? currentUser.id
    : chat.chat.participants.find((p) => p.user.id !== currentUser.id)!.user.id;

  return createMessage(
    `msg-${Date.now()}`,
    chatId,
    text,
    authorId,
    new Date()
  );
};

// Helper function to get chat name
export const getChatName = (chat: IChatMessageResponse): string => {
  if (chat.chat.type === ChatType.Group) {
    return chat.chat.participants
      .filter((p) => p.user.id !== currentUser.id)
      .map((p) => getFirstName(p.user))
      .join(', ');
  } else {
    const otherUser = chat.chat.participants.find((p) => p.user.id !== currentUser.id);
    return otherUser ? getFullName(otherUser.user) : 'Unknown';
  }
};

// Helper function to get chat avatar
export const getChatAvatar = (chat: IChatMessageResponse): string | undefined => {
  if (chat.chat.type === ChatType.OneToOne) {
    const otherUser = chat.chat.participants.find((p) => p.user.id !== currentUser.id);
    if (otherUser) {
      const initials = `${otherUser.user.name[0]}${otherUser.user.surname[0]}`;
      return `https://ui-avatars.com/api/?name=${initials}&background=random`;
    }
  }
  return undefined;
};

// Helper для отображения роли пользователя
export const getUserRoleBadge = (user: UserResponse): string => {
  return user.roleNames[0] || 'GymVisitor';
};
