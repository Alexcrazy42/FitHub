export interface ListResponse<T> {
    items: T[];
    currentPage: number | null;
    pageSize: number | null;
    totalItems: number | null;
    totalPages: number | null;
}