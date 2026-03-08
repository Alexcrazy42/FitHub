import { useEffect, useState } from 'react';
import { Modal, Button, Spin, Tag } from 'antd';
import {
  LockOutlined,
  UnlockOutlined,
  CheckCircleOutlined,
  WarningOutlined,
  StopOutlined,
  QuestionCircleOutlined,
} from '@ant-design/icons';
import { useApiService } from '../../../../api/useApiService';
import { CheckUrlResponse, useSecurityService } from '../../../../api/services/securityService';

interface LinkSafetyModalProps {
  url: string | null;
  onClose: () => void;
}

const STATUS_CONFIG: Record<
  CheckUrlResponse['status'],
  { label: string; tagColor: string; icon: React.ReactNode }
> = {
  Safe:       { label: 'Безопасно',      tagColor: 'green',   icon: <CheckCircleOutlined className="text-green-500" /> },
  Suspicious: { label: 'Подозрительно', tagColor: 'orange',  icon: <WarningOutlined className="text-orange-500" /> },
  Malicious:  { label: 'Опасно',         tagColor: 'red',     icon: <StopOutlined className="text-red-500" /> },
  Unknown:    { label: 'Неизвестно',     tagColor: 'default', icon: <QuestionCircleOutlined className="text-gray-400" /> },
};

const COUNTDOWN_SECONDS = 1;

export const LinkSafetyModal: React.FC<LinkSafetyModalProps> = ({ url, onClose }) => {
  const apiService = useApiService();
  const securityService = useSecurityService(apiService);

  const [checking, setChecking] = useState(false);
  const [result, setResult] = useState<CheckUrlResponse | null>(null);
  const [countdown, setCountdown] = useState(COUNTDOWN_SECONDS);
  const [canContinue, setCanContinue] = useState(false);

  // Reset and start scan whenever the URL changes
  useEffect(() => {
    if (!url) return;

    setResult(null);
    setChecking(true);
    setCountdown(COUNTDOWN_SECONDS);
    setCanContinue(false);

    securityService
      .checkUrl(url)
      .then((resp) => {
        if (resp.success && resp.data) setResult(resp.data);
      })
      .catch(() => {})
      .finally(() => setChecking(false));

    // Countdown that enables "Continue" after COUNTDOWN_SECONDS seconds
    let remaining = COUNTDOWN_SECONDS;
    const timer = setInterval(() => {
      remaining--;
      setCountdown(remaining);
      if (remaining <= 0) {
        clearInterval(timer);
        setCanContinue(true);
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [url]);

  const handleContinue = () => {
    if (!url) return;
    onClose();
    window.open(url, '_blank', 'noopener,noreferrer');
  };

  if (!url) return null;

  let domain = url;
  try { domain = new URL(url).hostname; } catch { /* keep raw url */ }

  const hasSsl = url.startsWith('https://');
  const vtStatus = result?.status ?? 'Unknown';
  const statusInfo = STATUS_CONFIG[vtStatus];

  return (
    <Modal
      open={!!url}
      onCancel={onClose}
      footer={null}
      title="Проверка безопасности ссылки"
      centered
      width={440}
    >
      <div className="flex flex-col gap-4 py-2">
        {/* Domain preview */}
        <div className="bg-gray-50 rounded-lg p-3 border border-gray-200">
          <p className="text-xs text-gray-400 mb-1 uppercase tracking-wide">Домен</p>
          <p className="font-mono text-sm font-semibold text-gray-800 break-all">{domain}</p>
        </div>

        {/* SSL status */}
        <div className="flex items-center gap-2">
          {hasSsl ? (
            <>
              <LockOutlined className="text-green-500 text-base" />
              <span className="text-sm text-green-600">SSL-сертификат активен (HTTPS)</span>
            </>
          ) : (
            <>
              <UnlockOutlined className="text-red-500 text-base" />
              <span className="text-sm text-red-600">Небезопасное соединение (HTTP)</span>
            </>
          )}
        </div>

        {/* VirusTotal scan */}
        <div className="border border-gray-200 rounded-lg p-3">
          <p className="text-xs text-gray-400 mb-2 uppercase tracking-wide">VirusTotal</p>
          {checking ? (
            <div className="flex items-center gap-2 text-gray-500">
              <Spin size="small" />
              <span className="text-sm">Сканирование...</span>
            </div>
          ) : result ? (
            <div className="flex items-center gap-2 flex-wrap">
              {statusInfo.icon}
              <Tag color={statusInfo.tagColor}>{statusInfo.label}</Tag>
              {result.totalEngines > 0 && (
                <span className="text-xs text-gray-500">
                  {result.maliciousCount + result.suspiciousCount} угроз из {result.totalEngines} проверок
                </span>
              )}
            </div>
          ) : (
            <span className="text-sm text-gray-400">Проверка недоступна</span>
          )}
        </div>

        {/* Actions */}
        <div className="flex gap-3 pt-1">
          <Button
            danger
            block
            icon={<StopOutlined />}
            onClick={onClose}
          >
            Заблокировать
          </Button>
          <Button
            type="primary"
            block
            disabled={!canContinue}
            onClick={handleContinue}
          >
            {canContinue ? 'Продолжить' : `Продолжить (${countdown})`}
          </Button>
        </div>
      </div>
    </Modal>
  );
};
