import { useCallback, useEffect, useState } from "react";
import { OrderForm } from "./components/OrderForm";
import { OrderTable } from "./components/OrderTable";
import {
  getOrderById,
  getOrders,
} from "./services/api";
import type {
  OrderDetailsResponse,
  OrderResponse,
} from "./types/order";
import {OrderStatusBadge} from "./components/OrderStatusBadge.tsx";

function App() {
  const [orders, setOrders] = useState<OrderResponse[]>(
      []
  );

  const [selectedOrder, setSelectedOrder] =
      useState<OrderDetailsResponse | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadOrders = useCallback(async () => {
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
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadOrders();

    const interval = setInterval(() => {
      loadOrders();
    }, 3000);

    return () => clearInterval(interval);
  }, [loadOrders]);

  async function handleSelectOrder(id: string) {
    try {
      const order = await getOrderById(id);
      setSelectedOrder(order);
    } catch (exception) {
      setError(
          exception instanceof Error
              ? exception.message
              : "Erro ao carregar detalhes."
      );
    }
  }

  return (
      <main className="page">
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
          <OrderForm onCreated={loadOrders} />

          <section className="orders-section">
            <div className="section-header">
              <div>
                <h2>Pedidos</h2>
                <p>
                  Atualização automática a cada 3 segundos.
                </p>
              </div>

              <button
                  className="secondary-button"
                  onClick={loadOrders}
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

        {selectedOrder && (
            <section className="details-card">
              <div className="section-header">
                <div>
                  <h2>Detalhes do pedido</h2>
                  <p>{selectedOrder.id}</p>
                </div>

                <button
                    className="secondary-button"
                    onClick={() =>
                        setSelectedOrder(null)
                    }
                >
                  Fechar
                </button>
              </div>

              <div className="details-grid">
                <div>
                  <strong>Cliente</strong>
                  <span>{selectedOrder.customer}</span>
                </div>

                <div>
                  <strong>Produto</strong>
                  <span>{selectedOrder.product}</span>
                </div>

                <div>
                  <strong>Valor</strong>
                  <span>
                {selectedOrder.value.toLocaleString(
                    "pt-BR",
                    {
                      style: "currency",
                      currency: "BRL",
                    }
                )}
              </span>
                </div>

                <div>
                  <strong>Status atual</strong>
                  <OrderStatusBadge
                      status={selectedOrder.orderStatus}
                  />
                </div>
              </div>

              <h3>Histórico de status</h3>

              <div className="history-list">
                {selectedOrder.statusHistory.map(
                    (history, index) => (
                        <div
                            className="history-item"
                            key={`${history.changedAt}-${index}`}
                        >
                          <div>
                            <strong>
                              {history.newStatus}
                            </strong>
                            <span>
                      {history.previousStatus
                          ? `${history.previousStatus} → ${history.newStatus}`
                          : "Criação do pedido"}
                    </span>
                          </div>

                          <time>
                            {new Date(
                                history.changedAt
                            ).toLocaleString("pt-BR")}
                          </time>
                        </div>
                    )
                )}
              </div>
            </section>
        )}
      </main>
  );
}

export default App;
