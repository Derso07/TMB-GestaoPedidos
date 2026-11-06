import { useCallback, useEffect, useRef, useState } from "react";
import { getOrders } from "../services/api";
import type { OrderResponse } from "../types/order";

const POLLING_INTERVAL = 3000;

function isOrderInProgress(status: string): boolean {
    const normalizedStatus = status
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();

    return (
        normalizedStatus === "pending" ||
        normalizedStatus === "pendente" ||
        normalizedStatus === "processing" ||
        normalizedStatus === "processando"
    );
}

export function useOrdersPolling() {
    const [orders, setOrders] = useState<OrderResponse[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const requestInProgress = useRef(false);

    const loadOrders = useCallback(async () => {
        if (requestInProgress.current) {
            return;
        }

        requestInProgress.current = true;

        try {
            const result = await getOrders();

            setOrders(result);
            setError("");
        } catch (exception) {
            setError(
                exception instanceof Error
                    ? exception.message
                    : "Não foi possível carregar os pedidos."
            );
        } finally {
            requestInProgress.current = false;
            setLoading(false);
        }
    }, []);

    const hasOrdersInProgress = orders.some((order) =>
        isOrderInProgress(order.orderStatus)
    );

    useEffect(() => {
        void loadOrders();

        const intervalId = window.setInterval(() => {
            if (hasOrdersInProgress) {
                void loadOrders();
            }
        }, POLLING_INTERVAL);

        return () => {
            window.clearInterval(intervalId);
        };
    }, [loadOrders, hasOrdersInProgress]);

    return {
        orders,
        loading,
        error,
        refresh: loadOrders,
        hasOrdersInProgress,
    };
}
