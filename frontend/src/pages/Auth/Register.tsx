import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useApiServiceWithoutNavigate } from "../../api/useApiService";
import { StartRegisterRequest, UserResponse } from "../../types/auth";
import { IGymResponse } from "../../types/gyms";
import { ListResponse } from "../../types/common";
import { toast } from "react-toastify";

export const Register: React.FC = () => {
  const navigate = useNavigate();
  const apiService = useApiServiceWithoutNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [gyms, setGyms] = useState<IGymResponse[]>([]);
  const [gymsLoading, setGymsLoading] = useState(true);
  const isDark = false;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<StartRegisterRequest>({ mode: "onBlur" });

  useEffect(() => {
    const fetchGyms = async () => {
      try {
        const response = await apiService.get<ListResponse<IGymResponse>>(
          "v1/gyms?PageNumber=1&PageSize=100"
        );
        if (response.success && response.data) {
          setGyms(response.data.items);
        }
      } catch {
        toast.error("Не удалось загрузить список залов");
      } finally {
        setGymsLoading(false);
      }
    };

    fetchGyms();
  }, []);

  const onSubmit = async (data: StartRegisterRequest) => {
    document.cookie =
      "FitHub.Identity.Cookie=; path=/; domain=example.com; expires=Thu, 01 Jan 1970 00:00:00 GMT;";
    try {
      setIsLoading(true);

      const response = await apiService.post<UserResponse>(
        "v1/auth/start-register",
        data
      );

      if (response.success) {
        toast.success("Ожидайте сообщения на почту!");
      } else {
        toast.error(response.error?.detail || "Ошибка");
      }
    } catch {
      toast.error("Ошибка сервера!");
    } finally {
      setIsLoading(false);
    }
  };

  const inputClass = `w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500`;
  const labelClass = `block text-sm font-medium mb-2 text-gray-700`;

  return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-gray-100">
      <div className="w-full max-w-md rounded-2xl p-8 shadow-xl space-y-6 bg-white text-gray-800">
        <div className="text-center">
          <h2 className="text-3xl font-bold">Регистрация</h2>
          <p className="mt-2 text-gray-500">Создайте новый аккаунт</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* Фамилия */}
          <div>
            <label className={labelClass}>Фамилия</label>
            <input
              {...register("surname", { required: "Введите фамилию" })}
              className={inputClass}
              placeholder="Введите вашу фамилию"
              disabled={isLoading}
            />
            {errors.surname && (
              <p className="text-sm text-red-500 mt-1">{errors.surname.message}</p>
            )}
          </div>

          {/* Имя */}
          <div>
            <label className={labelClass}>Имя</label>
            <input
              {...register("name", { required: "Введите имя" })}
              className={inputClass}
              placeholder="Введите ваше имя"
              disabled={isLoading}
            />
            {errors.name && (
              <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
            )}
          </div>

          {/* Email */}
          <div>
            <label className={labelClass}>Email</label>
            <input
              type="email"
              {...register("email", {
                required: "Введите email",
                pattern: {
                  value: /\S+@\S+\.\S+/,
                  message: "Некорректный email",
                },
              })}
              className={inputClass}
              placeholder="Введите ваш email"
              disabled={isLoading}
            />
            {errors.email && (
              <p className="text-sm text-red-500 mt-1">{errors.email.message}</p>
            )}
          </div>

          {/* Зал */}
          <div>
            <label className={labelClass}>Зал</label>
            <select
              {...register("gymId", { required: "Выберите зал" })}
              className={`${inputClass} ${gymsLoading ? "opacity-50 cursor-not-allowed" : ""}`}
              disabled={isLoading || gymsLoading}
              defaultValue=""
            >
              <option value="" disabled>
                {gymsLoading ? "Загрузка залов..." : "Выберите зал"}
              </option>
              {gyms.map((gym) => (
                <option key={gym.id} value={gym.id}>
                  {gym.name}
                </option>
              ))}
            </select>
            {errors.gymId && (
              <p className="text-sm text-red-500 mt-1">{errors.gymId.message}</p>
            )}
          </div>

          {/* Button */}
          <button
            type="submit"
            disabled={isLoading || gymsLoading}
            className={`w-full font-semibold py-2.5 rounded-lg transition duration-200 shadow ${
              isLoading || gymsLoading
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700 text-white"
            }`}
          >
            {isLoading ? "Отправка..." : "Зарегистрироваться"}
          </button>
        </form>

        <div className="text-center pt-1">
          <span className="text-gray-500 text-sm">Уже есть аккаунт?</span>
          <button
            onClick={() => navigate("/login")}
            className="ml-1 text-sm font-medium underline text-blue-600 hover:text-blue-700"
          >
            Войти
          </button>
        </div>
      </div>
    </div>
  );
};
