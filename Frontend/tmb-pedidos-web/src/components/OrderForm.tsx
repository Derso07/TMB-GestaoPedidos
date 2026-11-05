import { useState } from "react";
import { createOrder } from "../services/api";
import type { OrderRequest } from "../types/order";

interface OrderFormProps {
    onCreated: () => void;
}

const initialForm: OrderRequest = {
    customer: "",
    product: "",
    value: 0,
};

export function OrderForm({
                              onCreated,
                          }: OrderFormProps) {
    const [form, setForm] = useState<OrderRequest>(
        initialForm
    );

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    function updateField(
        field: keyof OrderRequest,
        value: string
    ) {
        setForm((current) => ({
            ...current,
            [field]:
                field === "value"
                    ? Number(value)
                    : value,
        }));
    }

    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>
    ) {
        event.preventDefault();
        setError("");

        if (!form.customer.trim()) {
            setError("Informe o cliente.");
            return;
        }

        if (!form.product.trim()) {
            setError("Informe o produto.");
            return;
        }

        if (form.value <= 0) {
            setError("O valor deve ser maior que zero.");
            return;
        }

        try {
            setLoading(true);

            await createOrder(form);

            setForm(initialForm);
            onCreated();
        } catch (exception) {
            setError(
                exception instanceof Error
                    ? exception.message
                    : "Erro ao criar o pedido."
            );
        } finally {
            setLoading(false);
        }
    }

    return (
        <form
            className="order-form"
            onSubmit={handleSubmit}
        >
            <h2>Novo pedido</h2>

            <label>
                Cliente
                <input
                    type="text"
                    value={form.customer}
                    onChange={(event) =>
                        updateField(
                            "customer",
                            event.target.value
                        )
                    }
                    maxLength={200}
                    required
                />
            </label>

            <label>
                Produto
                <input
                    type="text"
                    value={form.product}
                    onChange={(event) =>
                        updateField(
                            "product",
                            event.target.value
                        )
                    }
                    maxLength={200}
                    required
                />
            </label>

            <label>
                Valor
                <input
                    type="number"
                    value={form.value || ""}
                    onChange={(event) =>
                        updateField(
                            "value",
                            event.target.value
                        )
                    }
                    min="0.01"
                    step="0.01"
                    required
                />
            </label>

            {error && (
                <p className="error-message">
                    {error}
                </p>
            )}

            <button
                type="submit"
                disabled={loading}
            >
                {loading ? "Criando..." : "Criar pedido"}
            </button>
        </form>
    );
}
