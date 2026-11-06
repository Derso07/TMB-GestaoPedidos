import { useCallback, useEffect, useState } from "react";
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

function App() {
  const [orders, setOrders] = useState<OrderResponse[]>([]);

  const [selectedOrder, setSelectedOrder] =
      useState<OrderDetailsResponse | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState("");

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