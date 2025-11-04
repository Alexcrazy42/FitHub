import { useNavigate, useSearchParams } from "react-router-dom";
import { ConfirmEmailRequest, LoginResponse, SetPasswordRequest } from "../../types/auth";
import { useApiService } from "../../api/useApiService";
import { toast } from "react-toastify";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";

interface FormValues {
  newPassword: string;
  confirmPassword: string;
}


export const SetPassword: React.FC = () => {
    const [searchParams] = useSearchParams();
    const userId = searchParams.get("userId");
    const token = searchParams.get("token");
    const apiService = useApiService();
    const navigate = useNavigate();
    const [isValid, setIsValid] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm<FormValues>();

    const setPassword = async (request: SetPasswordRequest) => {
        try {
          const response = await apiService.post<LoginResponse>('v1/auth/set-password', request);
          if (response.success) {
            toast.success("Пароль успешно выставлен!");
            navigate("/login");
          } else {
            toast.error(response.error?.detail || "Ошибка сброса пароля");
          }
        } catch {
          toast.error("Ошибка!");
        }
      };

    const check = async () => {
        try {
            if(!userId || !token) {
                toast.error("Неправильная ссылка!");
                return;
            }

            const request: ConfirmEmailRequest = { userId, token };
            const response = await apiService.post<boolean>('v1/auth/check-confirm-email', request);

            if(response.success) {
                setIsValid(true);
            } else {
                //setError(response.error?.detail || "Ошибка подтверждения почты!");
                toast.error(response.error?.detail || "Ошибка подтверждения почты!");
            }
        } catch {
            //setError("Ошибка сервера!");
            toast.error("Ошибка сервера!");
        } finally {
            setIsLoading(false);
        }
    }

    useEffect(() => {
        check();
    }, []);

    if (isLoading) {
        return <div>Проверка ссылки...</div>;
    }

    if (!isValid) {
        return <div>Ссылка недействительна или уже использована</div>;
    }

    const onSubmit = async (data: FormValues) => {
        if (!userId || !token) return;
        const request: SetPasswordRequest = {
          userId,
          token,
          password: data.newPassword
        };
        await setPassword(request);
      };

    const newPassword = watch("newPassword");

    return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-gray-100">
      <div className="w-full max-w-md p-8 rounded-xl shadow-lg bg-white space-y-6">
        <h2 className="text-2xl font-bold text-center">Выставление пароля</h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Новый пароль */}
          <div>
            <label className="block mb-1 text-sm font-medium">Пароль</label>
            <input
              type="password"
              {...register("newPassword", { required: "Введите пароль" })}
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
            {isSubmitting ? "Выполнение..." : "Выставить пароль"}
          </button>
        </form>
      </div>
    </div>
  );
}