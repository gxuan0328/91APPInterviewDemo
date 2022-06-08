import { Product } from './product';

export interface OrderOverview {
    Id: number;
    Buyer: string;
    PaymentType: string;
    Total: number;
    Address: string;
    CreateTime: string;
    OrderStatusType_Id: number;
    OrderStatusType: string;
}
export interface OrderDetail {
    Id: number;
    Buyer: string;
    Seller: string;
    Products: Product[];
    PaymentType: string;
    Address: string;
    CreateTime: string;
    OrderStatusType: string;
    Total: number;
    Cost: number;
}