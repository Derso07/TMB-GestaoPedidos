import type {
    OrderDetailsResponse,
    OrderRequest,
    OrderResponse,
} from "../types/order";

const apiUrl = import.meta.env.VITE_API_URL;

if (!apiUrl) {
    throw new Error(
        "A variável VITE_API_URL não foi configurada."
    );
}

async function handleResponse<T>(
    response: Response
): Promise<T> {
    if (!response.ok) {
        let message = "Não foi possível realizar a operação.";

        try {
            const error = await response.json();

            if (error.title) {
                message = error.title;
            }
        } catch {
            // Mantém a mensagem padrão caso a API não retorne JSON.
        }

        throw new Error(message);
    }

    return response.json() as Promise<T>;
}

export async function getOrders(): Promise<OrderResponse[]> {
    const response = await fetch(`${apiUrl}/api/v1/Order`);

    return handleResponse<OrderResponse[]>(response);
}

export async function getOrderById(
    id: string
): Promise<OrderDetailsResponse> {
    const response = await fetch(
        `${apiUrl}/api/v1/Order/${id}`
    );

    return handleResponse<OrderDetailsResponse>(response);
}

export async function createOrder(
    request: OrderRequest
): Promise<OrderResponse> {
    const response = await fetch(`${apiUrl}/api/v1/Order`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });

    return handleResponse<OrderResponse>(response);
}
