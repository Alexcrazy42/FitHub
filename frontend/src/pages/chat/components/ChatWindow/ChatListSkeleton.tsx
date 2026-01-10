// src/features/chat/components/ChatList/ChatListSkeleton.tsx

import React from 'react';
import { Skeleton } from 'antd';

const ChatListSkeleton: React.FC = () => {
  return (
    <div className="bg-white">
      {/* Header skeleton */}
      <div className="p-4 border-b border-gray-200">
        <Skeleton.Input 
          active 
          size="large" 
          className="mb-4 w-24" 
        />
        <Skeleton.Input 
          active 
          size="large" 
          className="w-full" 
          style={{ height: 40 }}
        />
      </div>

      {/* Chat list items skeleton */}
      <div className="divide-y divide-gray-100">
        {[...Array(8)].map((_, index) => (
          <div key={index} className="flex items-center gap-3 p-4">
            {/* Avatar skeleton */}
            <Skeleton.Avatar 
              active 
              size={48} 
              shape="circle" 
            />
            
            {/* Content skeleton */}
            <div className="flex-1 space-y-2">
              {/* Name and time */}
              <div className="flex items-center justify-between">
                <Skeleton.Input 
                  active 
                  size="small" 
                  style={{ width: 120, height: 16 }} 
                />
                <Skeleton.Input 
                  active 
                  size="small" 
                  style={{ width: 40, height: 12 }} 
                />
              </div>
              
              {/* Last message */}
              <Skeleton.Input 
                active 
                size="small" 
                style={{ width: '100%', height: 14 }} 
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ChatListSkeleton;
