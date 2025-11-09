import { useAuth } from '../../../context/useAuth';
import { useTheme } from '../../../context/useTheme';
import { roleMapping } from '../../../types/auth';

const UserProfile: React.FC = () => {
  const { user } = useAuth();
  const { theme } = useTheme();
  const isDark = theme === 'dark';

  return (
    <div className="p-4">
      <h1
        className={`text-2xl font-bold mb-4 ${
          isDark ? 'text-white' : 'text-gray-800'
        }`}
      >
        Профиль
      </h1>
      {user && (
        <div
          className={`p-6 rounded shadow ${
            isDark
              ? 'bg-gray-800 text-gray-100 shadow-gray-900/50'
              : 'bg-white text-gray-800 shadow-gray-200'
          }`}
        >
          <p>
            <strong>ID:</strong> {user.id}
          </p>
          <p>
            <strong>Email:</strong> {user.email}
          </p>
          <p>
            <strong>Роль:</strong> {roleMapping[user.currentRole]}
          </p>
        </div>
      )}
    </div>
  );
};

export default UserProfile;