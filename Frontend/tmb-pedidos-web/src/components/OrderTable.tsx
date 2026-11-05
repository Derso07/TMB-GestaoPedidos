import type { OrderResponse } from "../types/order";
import { OrderStatusBadge } from "./OrderStatusBadge";

interface OrderTableProps {
    orders: OrderResponse[];
    onSelect: (id: string) => void;
}

export function OrderTable({
                               orders,
                               onSelect,
                           }: OrderTableProps) {
    if (orders.length === 0) {
        return (
            <div className="empty-state">
                Nenhum pedido encontrado.
            </div>
        );
    }

    return (
        <div className="table-container">
            <table>
                <thead>
                <tr>
                    <th>Cliente</th>
                    <th>Produto</th>
                    <th>Valor</th>
                    <th>Status</th>
                    <th>Data</th>
                    <th>Ação</th>
                </tr>
                </thead>

                <tbody>
                {orders.map((order) => (
                    <tr key={order.id}>
                        <td>{order.customer}</td>
                        <td>{order.product}</td>
                        <td>
                            {order.value.toLocaleString(
                                "pt-BR",
                                {
                                    style: "currency",
                                    currency: "BRL",
                                }
                            )}
                        </td>
                        <td>
                            <OrderStatusBadge
                                status={order.orderStatus}
                            />
                        </td>
                        <td>
                            {new Date(
                                order.creatOn
                            ).toLocaleString("pt-BR")}
                        </td>
                        <td>
                            <button
                                className="secondary-button"
                                onClick={() =>
                                    onSelect(order.id)
                                }
                            >
                                Detalhes
                            </button>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
}
