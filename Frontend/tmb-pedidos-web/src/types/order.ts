export type OrderStatus =
    | "Pending"
    | "Processing"
    | "Completed";

export interface OrderRequest {
    customer: string;
    product: string;
    value: number;
}

export interface OrderResponse {
    id: string;
    customer: string;
    product: string;
    value: number;
    creatOn: string;
    orderStatus: OrderStatus | string;
}

export interface OrderStatusHistoryResponse {
    previousStatus: string | null;
    newStatus: string;
    changedAt: string;
}

export interface OrderDetailsResponse extends OrderResponse {
    statusHistory: OrderStatusHistoryResponse[];
}
