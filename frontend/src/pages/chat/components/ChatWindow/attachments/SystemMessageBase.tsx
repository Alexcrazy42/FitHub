import React from 'react';
import { Avatar } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { format } from 'date-fns';
import { IMessageResponse } from '../../../../../types/messaging';

interface SystemMessageBaseProps {
  message: IMessageResponse;
  icon?: React.ReactNode;
  children: React.ReactNode;
}

export const SystemMessageBase: React.FC<SystemMessageBaseProps> = ({ 
  message, 
  icon,
  children 
}) => {
  const formatTime = (dateString: string) => {
    return format(new Date(dateString), 'HH:mm');
  };

  return (
    <div className="flex justify-center my-4">
      <div className="max-w-2xl bg-gray-100 rounded-lg px-4 py-3 flex items-center gap-3">
        {icon || (
          <Avatar
            size={32}
            src={`https://ui-avatars.com/api/?name=${message.createdBy.name[0]}${message.createdBy.surname[0]}&background=random`}
            icon={<UserOutlined />}
          />
        )}
        
        
        <div className="flex-1">
          <div className="text-sm text-gray-700">
            {children}
          </div>
          <div className="text-xs text-gray-500 mt-1">
            {formatTime(message.createdAt)}
          </div>
        </div>
      </div>
    </div>
  );
};
