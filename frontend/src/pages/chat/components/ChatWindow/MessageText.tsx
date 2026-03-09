import { useState } from 'react';
import { Popover, Button } from 'antd';
import { LinkOutlined, StopOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { LinkSafetyModal } from './LinkSafetyModal';

const URL_REGEX = /https?:\/\/[^\s<>"{}|\\^`[\]]+/gi;

interface MessageLinkProps {
  url: string;
  isMyMessage: boolean;
  onExternalClick: (url: string) => void;
}

const MessageLink: React.FC<MessageLinkProps> = ({ url, isMyMessage, onExternalClick }) => {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();

  const isInternal = (() => {
    try {
      return new URL(url).origin === window.location.origin;
    } catch {
      return false;
    }
  })();

  const handleGo = () => {
    setOpen(false);
    if (isInternal) {
      try {
        const { pathname, search, hash } = new URL(url);
        navigate(pathname + search + hash);
      } catch {
        window.open(url, '_blank', 'noopener,noreferrer');
      }
    } else {
      onExternalClick(url);
    }
  };

  const content = (
    <div className="flex flex-col gap-2 min-w-36">
      <div className="text-xs text-gray-400 font-mono truncate max-w-48">
        {(() => { try { return new URL(url).hostname; } catch { return url; } })()}
      </div>
      <Button type="primary" size="small" icon={<LinkOutlined />} onClick={handleGo}>
        Перейти по ссылке
      </Button>
      <Button size="small" icon={<StopOutlined />} onClick={() => setOpen(false)}>
        Остаться на странице
      </Button>
    </div>
  );

  return (
    <Popover
      content={content}
      open={open}
      onOpenChange={setOpen}
      trigger="hover"
      mouseEnterDelay={0.3}
      mouseLeaveDelay={0.2}
    >
      <span
        className={`underline cursor-pointer break-all ${
          isMyMessage
            ? 'text-blue-100 hover:text-white'
            : 'text-blue-500 hover:text-blue-700'
        }`}
      >
        {url}
      </span>
    </Popover>
  );
};

interface MessageTextProps {
  text: string;
  isMyMessage: boolean;
}

export const MessageText: React.FC<MessageTextProps> = ({ text, isMyMessage }) => {
  const [safetyUrl, setSafetyUrl] = useState<string | null>(null);

  const parts: React.ReactNode[] = [];
  let lastIndex = 0;

  for (const match of text.matchAll(URL_REGEX)) {
    const url = match[0];
    const start = match.index!;

    if (start > lastIndex) {
      parts.push(text.slice(lastIndex, start));
    }

    parts.push(
      <MessageLink
        key={start}
        url={url}
        isMyMessage={isMyMessage}
        onExternalClick={setSafetyUrl}
      />
    );

    lastIndex = start + url.length;
  }

  if (lastIndex < text.length) {
    parts.push(text.slice(lastIndex));
  }

  return (
    <>
      {parts}
      <LinkSafetyModal url={safetyUrl} onClose={() => setSafetyUrl(null)} />
    </>
  );
};
