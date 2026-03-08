import { useEffect, useState } from 'react';
import { useAppSelector } from '../../../../store/hooks';
import { selectConnectionState } from '../../../../store/selectors';
import { ConnectionState } from '../../../../types/messaging';

type BannerState = 'hidden' | 'reconnecting' | 'connected';

const ConnectionBanner: React.FC = () => {
  const connectionState = useAppSelector(selectConnectionState);
  const [bannerState, setBannerState] = useState<BannerState>('hidden');

  useEffect(() => {
    if (connectionState === ConnectionState.RECONNECTING) {
      setBannerState('reconnecting');
    } else if (connectionState === ConnectionState.CONNECTED && bannerState === 'reconnecting') {
      setBannerState('connected');
      const timer = setTimeout(() => setBannerState('hidden'), 2000);
      return () => clearTimeout(timer);
    }
  }, [connectionState]);

  if (bannerState === 'hidden') return null;

  return (
    <div
      className={`flex items-center justify-center gap-2 px-4 py-2 text-sm font-medium text-white transition-all duration-300 ${
        bannerState === 'reconnecting' ? 'bg-yellow-500' : 'bg-green-500'
      }`}
    >
      {bannerState === 'reconnecting' ? (
        <>
          <span className="inline-block w-3 h-3 border-2 border-white border-t-transparent rounded-full animate-spin" />
          Подключение...
        </>
      ) : (
        <>
          <span>✓</span>
          Соединение восстановлено
        </>
      )}
    </div>
  );
};

export default ConnectionBanner;
