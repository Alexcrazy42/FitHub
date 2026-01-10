### JSONB вместо реляционной модели для динамических полей

Кейс: нужно внедрить множество вложений к сообщениям

Пример:
![create-group-attachment](../../images/jsonb-create-group.png)

В вк море таких вложений: создание группы, вложение с файлов, приглашение в группу, скидывание поста и тд

Используют подход с типом вложения + его метаданными в json
![vk-attachments](../../images/vk-attachments.png)

В .NET можно реализовать такой же json, но проще использовать json внутри строки, как мне кажется

![attachments](../../images/attachments.png)

#### Плюсы:
- легко хранить на бэкенде
- улучшенная производительно из-за отсутствия join (похоже на read-model)
- легко добавлять новые типы вложений
#### Минусы:
- для большинства типов вложений придется инвалидировать содержимое (собственно как и для read-model и для любого кэша)

Далее фронтенд парсит этот attachment и решает какой компонент будет рендерить его и каким образом. Базовый компонент, который регает, кто будет рендерить:
```tsx
export const CustomMessageAttachment: React.FC<CustomMessageAttachmentProps> = ({ message }) => {
  // Берем первый attachment (системное сообщение имеет только один всегда)
  const attachment = message.attachments[0];

  if (!attachment) {
    return null;
  }

  // Роутинг на конкретный компонент в зависимости от типа
  switch (attachment.type) {
    case MessageAttachmentType.CreateGroup:
      return <CreateGroupAttachment message={message} attachment={attachment} />;
    
    // case MessageAttachmentType.InviteUser:
    //   return <AddUserAttachment message={message} attachment={attachment} />;

    default:
      return (
        <div className="text-center py-2 px-4 bg-gray-100 rounded text-sm text-gray-600">
          Не поддерживается вашей версией
        </div>
      );
  }
};
```

Конкретный компонент конкретного attachment:
```tsx
interface CreateGroupAttachmentProps {
  message: IMessageResponse;
  attachment: IMessageAttachmentResponse;
}

export const CreateGroupAttachment: React.FC<CreateGroupAttachmentProps> = ({ 
  message,
  attachment 
}) => {
  const data = JSON.parse(attachment.data || '{}');
  const groupName = data.groupName || '';

  return (
    <SystemMessageBase 
      message={message}
      icon={<TeamOutlined className="text-2xl text-blue-500" />}
    >
      <span className="font-semibold">{getFullName(message.createdBy)}</span>
      {' '}создал(а) группу {groupName}
    </SystemMessageBase>
  );
};
```

Это конечно еще не BDUI, но впринцип очень похож: приходит динамический JSON на базе которого рендериться компонент в каком-то месте экрана. Только на всякидку раз в 5-7 больше человеко-часов чтобы реализовать BDUI с нуля