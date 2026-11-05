interface OrderStatusBadgeProps {
    status: string;
}

function normalizeStatus(status: string): string {
    return status
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

export function OrderStatusBadge({
                                     status,
                                 }: OrderStatusBadgeProps) {
    const normalizedStatus = normalizeStatus(status);

    let className = "status-badge status-pending";

    if (
        normalizedStatus === "processing" ||
        normalizedStatus === "processando"
    ) {
        className = "status-badge status-processing";
    }

    if (
        normalizedStatus === "completed" ||
        normalizedStatus === "finalizado"
    ) {
        className = "status-badge status-completed";
    }

    return (
        <span className={className}>
      {status}
    </span>
    );
}
