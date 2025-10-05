import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";

type ProblemDetails = {
    detail: string;
    status: number;
    title: string;
    type: string;
}

type ApiResponse<T> = {
    success: boolean;
    data?: T | null;
    error?: ProblemDetails | null;
}

export class ApiService {
    private api: AxiosInstance;

    constructor(baseUrl: string) {
        this.api = axios.create({
            baseURL: baseUrl,
            headers: {
                'Content-Type': 'application/json',
            }
        });

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
            
            return {
                success: false,
                error: axiosError.response?.data || {
                    detail: axiosError.message,
                    status: axiosError.response?.status || 500,
                    title: 'Request failed',
                    type: 'unknown_error'
                }
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
}

export const apiService = new ApiService("http://localhost:5209/api/");