import {
    useCallback,
    useEffect,
    useRef,
    useState,
} from "react";

import { OrderForm } from "./components/OrderForm";
import { OrderTable } from "./components/OrderTable";
import { OrderDetailsModal } from "./components/OrderDetailsModal";

import {
    getOrderById,
    getOrders,
} from "./services/api";

import type {
    OrderDetailsResponse,
    OrderResponse,
} from "./types/order";

const POLLING_INTERVAL = 3000;

function normalizeStatus(status: string): string {
    return status
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

function isOrderInProgress(status: string): boolean {
    const normalizedStatus = normalizeStatus(status);

    return (
        normalizedStatus === "pending" ||
        normalizedStatus === "pendente" ||
        normalizedStatus === "processing" ||
        normalizedStatus === "processando"
    );
}

function App() {
    const [orders, setOrders] = useState<OrderResponse[]>([]);

    const [selectedOrder, setSelectedOrder] =
        useState<OrderDetailsResponse | null>(null);

    const [modalOpen, setModalOpen] = useState(false);

    const [detailsLoading, setDetailsLoading] =
        useState(false);

    const [detailsError, setDetailsError] =
        useState("");

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState("");

    const [notification, setNotification] =
        useState("");

    const ordersRequestInProgress = useRef(false);

    const previousStatuses = useRef(
        new Map<string, string>()
    );

    const notificationTimer = useRef<number | null>(
        null
    );

    const loadOrders = useCallback(async () => {
        if (ordersRequestInProgress.current) {
            return;
        }

        ordersRequestInProgress.current = true;

        try {
            const result = await getOrders();

            setOrders(result);
            setError("");
        } catch (exception) {
            setError(
                exception instanceof Error
                    ? exception.message
                    : "Erro ao carregar pedidos."
            );
        } finally {
            ordersRequestInProgress.current = false;
            setLoading(false);
        }
    }, []);

    const showNotification = useCallback(
        (message: string) => {
            setNotification(message);

            if (notificationTimer.current !== null) {
                window.clearTimeout(
                    notificationTimer.current
                );
            }

            notificationTimer.current =
                window.setTimeout(() => {
                    setNotification("");
                    notificationTimer.current = null;
                }, 4000);
        },
        []
    );

    const updateSelectedOrder = useCallback(
        async (id: string) => {
            try {
                const updatedOrder = await getOrderById(id);

                setSelectedOrder((currentOrder) => {
                    if (!currentOrder) {
                        return updatedOrder;
                    }

                    if (
                        currentOrder.orderStatus !==
                        updatedOrder.orderStatus
                    ) {
                        showNotification(
                            `Status do pedido ${id.slice(
                                0,
                                8
                            )} atualizado para ${updatedOrder.orderStatus}.`
                        );
                    }

                    return updatedOrder;
                });
            } catch (exception) {
                console.error(
                    "Erro ao atualizar detalhes do pedido:",
                    exception
                );
            }
        },
        [showNotification]
    );

    const hasOrdersInProgress = orders.some((order) =>
        isOrderInProgress(order.orderStatus)
    );
    
    useEffect(() => {
        void loadOrders();

        if (!hasOrdersInProgress) {
            return;
        }

        const intervalId = window.setInterval(() => {
            void loadOrders();
        }, POLLING_INTERVAL);

        return () => {
            window.clearInterval(intervalId);
        };
    }, [loadOrders, hasOrdersInProgress]);

    useEffect(() => {
        if (orders.length === 0) {
            return;
        }

        orders.forEach((order) => {
            const previousStatus =
                previousStatuses.current.get(order.id);

            if (
                previousStatus !== undefined &&
                previousStatus !== order.orderStatus
            ) {
                showNotification(
                    `Status do pedido ${order.id.slice(
                        0,
                        8
                    )} atualizado para ${order.orderStatus}.`
                );
            }

            previousStatuses.current.set(
                order.id,
                order.orderStatus
            );
        });
    }, [orders, showNotification]);

    useEffect(() => {
        if (
            !modalOpen ||
            !selectedOrder ||
            !isOrderInProgress(
                selectedOrder.orderStatus
            )
        ) {
            return;
        }

        const intervalId = window.setInterval(() => {
            void updateSelectedOrder(selectedOrder.id);
        }, POLLING_INTERVAL);

        return () => {
            window.clearInterval(intervalId);
        };
    }, [
        modalOpen,
        selectedOrder?.id,
        selectedOrder?.orderStatus,
        updateSelectedOrder,
    ]);

    async function handleSelectOrder(id: string) {
        setModalOpen(true);
        setDetailsLoading(true);
        setDetailsError("");
        setSelectedOrder(null);

        try {
            const order = await getOrderById(id);

            setSelectedOrder(order);
        } catch (exception) {
            setDetailsError(
                exception instanceof Error
                    ? exception.message
                    : "Erro ao carregar detalhes."
            );
        } finally {
            setDetailsLoading(false);
        }
    }

    const handleCloseModal = useCallback(() => {
        setModalOpen(false);
        setSelectedOrder(null);
        setDetailsError("");
    }, []);

    useEffect(() => {
        return () => {
            if (notificationTimer.current !== null) {
                window.clearTimeout(
                    notificationTimer.current
                );
            }
        };
    }, []);

    return (
        <main className="page">
            {notification && (
                <div
                    className="toast-notification"
                    role="status"
                >
                    <span className="toast-icon">✓</span>
                    <span>{notification}</span>
                </div>
            )}

            <header className="page-header">
                <div>
                    <p className="eyebrow">TMB</p>

                    <h1>Gestão de pedidos</h1>

                    <p>
                        Acompanhe o processamento dos pedidos.
                    </p>
                </div>
            </header>

            <section className="content-grid">
                <OrderForm
                    onCreated={async () => {
                        await loadOrders();
                    }}
                />

                <section className="orders-section">
                    <div className="section-header">
                        <div>
                            <h2>Pedidos</h2>

                            <p>
                                {hasOrdersInProgress
                                    ? "Atualizando automaticamente enquanto houver pedidos em processamento."
                                    : "Todos os pedidos estão finalizados."}
                            </p>
                        </div>

                        <button
                            type="button"
                            className="secondary-button"
                            onClick={() => {
                                void loadOrders();
                            }}
                        >
                            Atualizar
                        </button>
                    </div>

                    {loading ? (
                        <div className="loading">
                            Carregando pedidos...
                        </div>
                    ) : (
                        <OrderTable
                            orders={orders}
                            onSelect={handleSelectOrder}
                        />
                    )}
                </section>
            </section>

            {error && (
                <div className="error-message global-error">
                    {error}
                </div>
            )}

            {modalOpen && (
                <OrderDetailsModal
                    order={selectedOrder}
                    loading={detailsLoading}
                    error={detailsError}
                    onClose={handleCloseModal}
                />
            )}
        </main>
    );
}

export default App;
