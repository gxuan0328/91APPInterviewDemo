export interface Response<T> {
    StatusCode: number;
    Message: string;
    Data: T;
}