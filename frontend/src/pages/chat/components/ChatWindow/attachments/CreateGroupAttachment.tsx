import { TeamOutlined } from '@ant-design/icons';
import { IMessageResponse, IMessageAttachmentResponse } from '../../../../../types/messaging';
import { SystemMessageBase } from './SystemMessageBase';
import { getFullName } from '../../../mocks/fakeData';


interface CreateGroupAttachmentProps {
  message: IMessageResponse;
  attachment: IMessageAttachmentResponse;
}

export const CreateGroupAttachment: React.FC<CreateGroupAttachmentProps> = ({ 
  message,
  attachment 
}) => {
  const data = JSON.parse(attachment.data || '{}');
  const groupName = data.groupName || ''; // специально делаем тип any, чтобы можно было без типа спокойно парсить json

  return (
    <SystemMessageBase 
      message={message}
      icon={<TeamOutlined className="text-2xl text-blue-500" />}
    >
      <span className="font-semibold">{getFullName(message.createdBy)}</span>
      {' '}создал(а) группу
    </SystemMessageBase>
  );
};
