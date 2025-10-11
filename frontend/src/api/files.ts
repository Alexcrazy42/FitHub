import { API_URL } from "./useApiService";

export function getFileRoute(key: string) : string {
    return API_URL + `/v1/files/${key}`;
}