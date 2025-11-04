import { useNavigate, useSearchParams } from "react-router-dom";
import { useApiService } from "../../api/useApiService";
import { toast } from "react-toastify";
import { ConfirmEmailRequest, LoginResponse } from "../../types/auth";
import { useEffect, useState } from "react";
import { CheckCircleOutlined, CloseCircleOutlined } from "@ant-design/icons";

export const ConfirmEmail: React.FC = () => {
    const [searchParams] = useSearchParams();
    const userId = searchParams.get("userId");
    const token = searchParams.get("token");
    const navigate = useNavigate();
    const apiService = useApiService();
    const [isEmailConfirmed, setIsEmailConfirmed] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const check = async () => {
        try {
            if(!userId || !token) {
                setError("Неправильная ссылка!");
                toast.error("Неправильная ссылка!");
                return;
            }

            const request: ConfirmEmailRequest = { userId, token };
            const response = await apiService.post<LoginResponse>('v1/auth/confirm-email', request);

            if(response.success) {
                const loginResponse = response.data!;
                if(loginResponse.isTemporaryPassword) {
                    setIsEmailConfirmed(true);
                    toast.success("Почта успешно подтверждена!");
                    navigate(`/set-password?token=${token}&userId=${userId}`);
                    toast.info("Придумайте пароль!");
                }
            } else {
                setError(response.error?.detail || "Ошибка подтверждения почты!");
                toast.error(response.error?.detail || "Ошибка подтверждения почты!");
            }
        } catch {
            setError("Ошибка сервера!");
            toast.error("Ошибка сервера!");
        } finally {
            setIsLoading(false);
        }
    }

    useEffect(() => { check(); }, []);

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
            <div className="w-full max-w-lg p-8 bg-white rounded-xl shadow-lg text-center space-y-6">
                {isLoading ? (
                    <div className="flex flex-col items-center">
                        <div className="loader ease-linear rounded-full border-8 border-t-8 border-gray-200 h-16 w-16 mb-4"></div>
                        <p className="text-gray-500 text-lg">Проверка подтверждения...</p>
                    </div>
                ) : error ? (
                    <div className="flex flex-col items-center text-red-600 space-y-2">
                        <CheckCircleOutlined className="w-16 h-16"/>
                        <p className="text-lg font-semibold">{error}</p>
                        <button 
                            onClick={() => navigate("/")}
                            className="mt-4 px-6 py-2 rounded-lg bg-red-600 text-white hover:bg-red-700 transition"
                        >
                            На главную
                        </button>
                    </div>
                ) : isEmailConfirmed ? (
                    <div className="flex flex-col items-center text-green-600 space-y-4">
                        <CloseCircleOutlined className="w-16 h-16"/>
                        <h2 className="text-2xl font-bold">Почта успешно подтверждена!</h2>
                        <p className="text-gray-600">Теперь вы можете придумать свой пароль.</p>
                        <button 
                            onClick={() => navigate(`/set-password?token=${token}`)}
                            className="mt-4 px-6 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 transition"
                        >
                            Придумать пароль
                        </button>
                    </div>
                ) : (
                    <div className="text-gray-500">Неизвестное состояние</div>
                )}
            </div>

            {/* Loader CSS */}
            <style>{`
                .loader {
                    border-top-color: #3490dc;
                    animation: spin 1s linear infinite;
                }
                @keyframes spin {
                    0% { transform: rotate(0deg); }
                    100% { transform: rotate(360deg); }
                }
            `}</style>
        </div>
    );
};
