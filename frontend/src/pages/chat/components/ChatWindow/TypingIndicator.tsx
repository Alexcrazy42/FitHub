import React from 'react';

interface TypingUser {
  userId: string;
  userName: string;
}

interface TypingIndicatorProps {
  users: TypingUser[];
}

const TypingIndicator: React.FC<TypingIndicatorProps> = ({ users }) => {
  if (users.length === 0) return null;

  const getText = () => {
    if (users.length === 1) {
      return `${users[0].userName} печатает...`;
    } else if (users.length === 2) {
      return `${users[0].userName} и ${users[1].userName} печатают...`;
    } else {
      return `${users[0].userName} и еще ${users.length - 1} печатают...`;
    }
  };

  return (
    <div className="px-6 py-2 bg-gray-50 border-t border-gray-200">
      <div className="flex items-center gap-2 text-sm text-gray-600">
        <div className="flex gap-1">
          <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }} />
          <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }} />
          <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }} />
        </div>
        <span>{getText()}</span>
      </div>
    </div>
  );
};

export default TypingIndicator;
