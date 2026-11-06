import { useEffect } from "react";
import type { OrderDetailsResponse } from "../types/order";
import { OrderStatusBadge } from "./OrderStatusBadge";

interface OrderDetailsModalProps {
    order: OrderDetailsResponse | null;
    loading: boolean;
    error: string;
    onClose: () => void;
}

export function OrderDetailsModal({
                                      order,
                                      loading,
                                      error,
                                      onClose,
                                  }: OrderDetailsModalProps) {
    useEffect(() => {
        function handleKeyDown(event: KeyboardEvent) {
            if (event.key === "Escape") onClose();
        }

        window.addEventListener("keydown", handleKeyDown);
        return () =>
            window.removeEventListener("keydown", handleKeyDown);
    }, [onClose]);

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div
                className="modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="modal-title"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="modal-header">
                    <div>
                        <h2 id="modal-title">Detalhes do pedido</h2>
                        {order && <p className="modal-subtitle">{order.id}</p>}
                    </div>
                    <button
                        className="modal-close"
                        onClick={onClose}
                        aria-label="Fechar"
                    >
                        ×
                    </button>
                </div>

                <div className="modal-body">
                    {loading && <p>Carregando...</p>}

                    {error && <p className="error-message">{error}</p>}

                    {order && !loading && (
                        <>
                            <div className="details-grid">
                                <div>
                                    <strong>Cliente</strong>
                                    <span>{order.customer}</span>
                                </div>

                                <div>
                                    <strong>Produto</strong>
                                    <span>{order.product}</span>
                                </div>

                                <div>
                                    <strong>Valor</strong>
                                    <span>
                                        {order.value.toLocaleString("pt-BR", {
                                            style: "currency",
                                            currency: "BRL",
                                        })}
                                    </span>
                                </div>

                                <div>
                                    <strong>Status atual</strong>
                                    <OrderStatusBadge status={order.orderStatus} />
                                </div>
                            </div>

                            <h3>Histórico de status</h3>

                            <div className="history-list">
                                {order.statusHistory.map((history, index) => (
                                    <div
                                        className="history-item"
                                        key={`${history.changedAt}-${index}`}
                                    >
                                        <div>
                                            <strong>{history.newStatus}</strong>
                                            <span>
                                                {history.previousStatus
                                                    ? `${history.previousStatus} → ${history.newStatus}`
                                                    : "Criação do pedido"}
                                            </span>
                                        </div>

                                        <time>
                                            {new Date(history.changedAt).toLocaleString(
                                                "pt-BR"
                                            )}
                                        </time>
                                    </div>
                                ))}
                            </div>
                        </>
                    )}
                </div>

                <div className="modal-footer">
                    <button className="secondary-button" onClick={onClose}>
                        Fechar
                    </button>
                </div>
            </div>
        </div>
    );
}