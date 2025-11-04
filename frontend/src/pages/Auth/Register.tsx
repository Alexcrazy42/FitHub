import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useApiService } from "../../api/useApiService";
import { StartRegisterRequest, UserResponse } from "../../types/auth";
import { toast } from "react-toastify";
import { useState } from "react";

export const Register: React.FC = () => {
  const navigate = useNavigate();
  const apiService = useApiService();
  const [isLoading, setIsLoading] = useState(false);
  const [isDark, setIsDark] = useState(false); // используй свою тему, если есть

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<StartRegisterRequest>({
    mode: "onBlur",
  });

  const onSubmit = async (data: StartRegisterRequest) => {
    document.cookie = "FitHub.Identity.Cookie=; path=/; domain=example.com; expires=Thu, 01 Jan 1970 00:00:00 GMT;";
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

  return (
    <div
      className={`min-h-screen flex items-center justify-center p-4 ${
        isDark ? "bg-gray-900" : "bg-gray-100"
      }`}
    >
      <div
        className={`w-full max-w-md rounded-2xl p-8 shadow-xl space-y-6 ${
          isDark ? "bg-gray-800 text-gray-100" : "bg-white text-gray-800"
        }`}
      >
        <div className="text-center">
          <h2 className="text-3xl font-bold">Регистрация</h2>
          <p className={`mt-2 ${isDark ? "text-gray-400" : "text-gray-500"}`}>
            Создайте новый аккаунт
          </p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* Фамилия */}
          <div>
            <label
              className={`block text-sm font-medium mb-2 ${
                isDark ? "text-gray-300" : "text-gray-700"
              }`}
            >
              Фамилия
            </label>
            <input
              {...register("surname", { required: "Введите фамилию" })}
              className={`w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 ${
                isDark
                  ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500"
              }`}
              placeholder="Введите вашу фамилию"
              disabled={isLoading}
            />
            {errors.surname && (
              <p className="text-sm text-red-500 mt-1">{errors.surname.message}</p>
            )}
          </div>

          {/* Имя */}
          <div>
            <label
              className={`block text-sm font-medium mb-2 ${
                isDark ? "text-gray-300" : "text-gray-700"
              }`}
            >
              Имя
            </label>
            <input
              {...register("name", { required: "Введите имя" })}
              className={`w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 ${
                isDark
                  ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500"
              }`}
              placeholder="Введите ваше имя"
              disabled={isLoading}
            />
            {errors.name && (
              <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
            )}
          </div>

          {/* Email */}
          <div>
            <label
              className={`block text-sm font-medium mb-2 ${
                isDark ? "text-gray-300" : "text-gray-700"
              }`}
            >
              Email
            </label>
            <input
              type="email"
              {...register("email", {
                required: "Введите email",
                pattern: {
                  value: /\S+@\S+\.\S+/,
                  message: "Некорректный email",
                },
              })}
              className={`w-full px-4 py-2.5 rounded-lg border outline-none transition focus:ring-2 ${
                isDark
                  ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:ring-blue-500"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-blue-500"
              }`}
              placeholder="Введите ваш email"
              disabled={isLoading}
            />
            {errors.email && (
              <p className="text-sm text-red-500 mt-1">{errors.email.message}</p>
            )}
          </div>

          {/* Button */}
          <button
            type="submit"
            disabled={isLoading}
            className={`w-full font-semibold py-2.5 rounded-lg transition duration-200 shadow ${
              isLoading
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700 text-white"
            }`}
          >
            {isLoading ? "Отправка..." : "Зарегистрироваться"}
          </button>
        </form>

        {/* Bottom redirect */}
        <div className="text-center pt-1">
          <span className={`${isDark ? "text-gray-400" : "text-gray-500"} text-sm`}>
            Уже есть аккаунт?
          </span>
          <button
            onClick={() => navigate("/login")}
            className={`ml-1 text-sm font-medium underline ${
              isDark ? "text-blue-400 hover:text-blue-300" : "text-blue-600 hover:text-blue-700"
            }`}
          >
            Войти
          </button>
        </div>
      </div>
    </div>
  );
};
