import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { FieldValues, UseFormSetError } from "react-hook-form";

export type ValidationError = {
  message: string;
  propertyName: string;
};

export type ProblemDetails = {
    detail: string;
    status: number;
    title: string;
    type: string;
    traceId?: string;
    errors?: ValidationError[];
}

export type ApiResponse<T> = {
    success: boolean;
    data?: T | null;
    error?: ProblemDetails | null;
}

export class ApiService {
    private api: AxiosInstance;
    private onUnauthorized: () => void;
    private onForbidden: () => void;

    constructor(baseUrl: string, onUnauthorized: () => void, onForbidden: () => void) {
        this.api = axios.create({
            baseURL: baseUrl,
            withCredentials: true,
            headers: {
                'Content-Type': 'application/json',
            }
        });
        this.onUnauthorized = onUnauthorized;
        this.onForbidden = onForbidden;

        this.api.interceptors.request.use(
            (config) => {
                return config;
            },
            (error) => Promise.reject(error)
        );

        this.api.interceptors.response.use(
            (response) => response,
            async (error: AxiosError) => {
                return this.handleError(error);
            }
        );
    }

    private async handleError(error: AxiosError): Promise<never> {
        return Promise.reject(error);
    }

    private async query<T>(
        httpMethod: (
            url: string, 
            data?: unknown, 
            config?: AxiosRequestConfig
        ) => Promise<AxiosResponse<T>>,
        url: string,
        data?: unknown,
        config?: AxiosRequestConfig
    ): Promise<ApiResponse<T>> {
        try {
            const response: AxiosResponse<T> = await httpMethod(url, data, config);

            return {
                success: true,
                data: response.data
            };
        } catch (error: unknown) {
            const axiosError = error as AxiosError<ProblemDetails>;
            const status = axiosError.response?.status;
            const problem = axiosError.response?.data;
            if (status === 401) {
                this.onUnauthorized();
            } else if (status === 403) {
                this.onForbidden();
            }

            const problemDetails: ProblemDetails = {
                type: problem?.type ?? 'unknown_error',
                title: problem?.title ?? 'Request failed',
                status: status ?? 500,
                detail: problem?.detail ?? axiosError.message,
                traceId: problem?.traceId,
                errors: problem?.errors ?? [],
            };

            return {
                success: false,
                error: problemDetails,
            };
        }
    }

    public async get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        return await this.query<T>(this.api.get.bind(this.api), url, undefined, config);
    }

    public async post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        return await this.query<T>(this.api.post.bind(this.api), url, data, config);
    }

    public async postFormData<T>(url: string, data?: FormData, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        const formDataConfig: AxiosRequestConfig = {
            headers: { 
                'Content-Type': 'multipart/form-data' 
            },
            ...config
        };

        return await this.query<T>(this.api.post.bind(this.api), url, data, formDataConfig);
    }

    public async put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        return await this.query<T>(this.api.put.bind(this.api), url, data, config);
    }

    public async patch<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        return await this.query<T>(this.api.patch.bind(this.api), url, data, config);
    }

    public async delete<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
        return await this.query<T>(this.api.delete.bind(this.api), url, undefined, config);
    }

    public async getBlob(url: string): Promise<Blob | null> {
        try {
            const response = await this.api.get<Blob>(url, { responseType: 'blob' });
            return response.data;
        } catch {
            return null;
        }
    }
}

/**
 * Маппит серверные ошибки валидации на поля формы react-hook-form
 * @param errors - массив ошибок с бэкенда
 * @param setError - функция setError из useForm
 */
export function mapServerValidationErrors<T extends FieldValues>(
  errors: ValidationError[] | undefined,
  setError: UseFormSetError<T>
): void {
  if (!errors || errors.length === 0) return;

  const mapPropertyToField = (propertyName: string): string => {
    if (!propertyName) return propertyName;
    return propertyName.charAt(0).toLowerCase() + propertyName.slice(1);
  };

  for (const err of errors) {
    const field = mapPropertyToField(err.propertyName);
    if (!field) continue;
    
    setError(field as unknown, { 
      type: "server", 
      message: err.message 
    });
  }
}