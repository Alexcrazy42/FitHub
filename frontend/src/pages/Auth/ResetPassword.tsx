import { useNavigate, useSearchParams } from "react-router-dom";
import { useApiService } from "../../api/useApiService";
import { toast } from "react-toastify";
import { LoginResponse, ResetPasswordRequest } from "../../types/auth";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";

interface FormValues {
  newPassword: string;
  confirmPassword: string;
}

export const ResetPassword: React.FC = () => {
  const [valid, setValid] = useState(true);
  const [loading, setLoading] = useState(true);
  const [searchParams] = useSearchParams();
  const userId = searchParams.get("userId");
  const token = searchParams.get("token");
  const apiService = useApiService();
  const navigate = useNavigate();

  const check = async () => {
    if (!userId || !token) {
      toast.error("Неправильная ссылка!");
      setLoading(false);
      return;
    }

    try {
      const request: ResetPasswordRequest = {
        userId,
        token,
        newPassword: null
      };
      const response = await apiService.post<LoginResponse>('v1/auth/check-reset-password', request);
      if (response.success) {
        setValid(true);
      } else {
        toast.error(response.error?.detail || "Ошибка проверки");
      }
    } catch {
      toast.error("Ошибка сервера");
    } finally {
      setLoading(false);
    }
  };

  const resetPassword = async (request: ResetPasswordRequest) => {
    try {
      const response = await apiService.post<LoginResponse>('v1/auth/reset-password', request);
      if (response.success) {
        toast.success("Пароль успешно сброшен!");
        navigate("/login");
      } else {
        toast.error(response.error?.detail || "Ошибка сброса пароля");
      }
    } catch {
      toast.error("Ошибка!");
    }
  };

  useEffect(() => {
    check();
  }, []);

  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm<FormValues>();

  const onSubmit = async (data: FormValues) => {
    if (!userId || !token) return;
    const request: ResetPasswordRequest = {
      userId,
      token,
      newPassword: data.newPassword
    };
    await resetPassword(request);
  };

  const newPassword = watch("newPassword");

  if (loading) {
    return <div>Проверка ссылки...</div>;
  }

  if (!valid) {
    return <div>Ссылка недействительна или уже использована</div>;
  }

  return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-gray-100">
      <div className="w-full max-w-md p-8 rounded-xl shadow-lg bg-white space-y-6">
        <h2 className="text-2xl font-bold text-center">Сброс пароля</h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Новый пароль */}
          <div>
            <label className="block mb-1 text-sm font-medium">Новый пароль</label>
            <input
              type="password"
              {...register("newPassword", { required: "Введите новый пароль" })}
              className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring focus:ring-blue-500 outline-none"
            />
            {errors.newPassword && <p className="text-red-500 text-sm mt-1">{errors.newPassword.message}</p>}
          </div>

          {/* Подтверждение пароля */}
          <div>
            <label className="block mb-1 text-sm font-medium">Подтвердите пароль</label>
            <input
              type="password"
              {...register("confirmPassword", {
                required: "Подтвердите пароль",
                validate: value => value === newPassword || "Пароли не совпадают"
              })}
              className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring focus:ring-blue-500 outline-none"
            />
            {errors.confirmPassword && <p className="text-red-500 text-sm mt-1">{errors.confirmPassword.message}</p>}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className={`w-full py-2.5 rounded-lg font-semibold text-white ${
              isSubmitting ? "bg-gray-400 cursor-not-allowed" : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {isSubmitting ? "Сброс..." : "Сбросить пароль"}
          </button>
        </form>
      </div>
    </div>
  );
};
