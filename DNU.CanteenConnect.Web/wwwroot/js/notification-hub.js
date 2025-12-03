// SignalR Connection for Real-Time Order Notifications
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect([0, 0, 3000, 5000, 10000, 15000, 30000])
    .build();

// Connection state event
connection.onreconnecting((error) => {
    console.log(`SignalR reconnecting: ${error}`);
});

connection.onreconnected((connectionId) => {
    console.log(`SignalR reconnected with ID: ${connectionId}`);
    showNotification("Kết nối lại thành công", "success");
});

connection.onclose(async (error) => {
    console.log(`SignalR disconnected: ${error}`);
});

// Listen for order status update notifications
connection.on("ReceiveOrderStatusUpdate", (orderId, newStatus, displayName) => {
    console.log(`Order #${orderId} status updated to: ${newStatus} (${displayName})`);
    showNotification(`Đơn hàng #${orderId} đã được cập nhật thành "${displayName}"`, "info");
    
    // Optional: Trigger page refresh or UI update
    // setTimeout(() => location.reload(), 2000);
});

// Listen for general notifications
connection.on("ReceiveNotification", (message) => {
    console.log(`Notification: ${message}`);
    showNotification(message, "info");
});

// Start the connection
connection.start()
    .catch(err => {
        console.error("SignalR connection failed:", err);
        return new Promise(resolve => setTimeout(() => connection.start().then(resolve).catch(() => resolve()), 5000));
    });

// Toast notification helper function
function showNotification(message, type = "info") {
    // Create a toast container if it doesn't exist
    if (!document.getElementById("toast-container")) {
        const container = document.createElement("div");
        container.id = "toast-container";
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 400px;
        `;
        document.body.appendChild(container);
    }

    // Create toast element
    const toast = document.createElement("div");
    const iconClass = type === "success" ? "bi-check-circle" : type === "error" ? "bi-exclamation-circle" : "bi-info-circle";
    const bgClass = type === "success" ? "bg-success" : type === "error" ? "bg-danger" : "bg-primary";
    
    toast.className = `alert alert-${type === "success" ? "success" : type === "error" ? "danger" : "info"} alert-dismissible fade show`;
    toast.setAttribute("role", "alert");
    toast.style.cssText = `
        margin-bottom: 10px;
        min-width: 300px;
        animation: slideIn 0.3s ease-in-out;
    `;
    
    toast.innerHTML = `
        <i class="bi ${iconClass}" style="margin-right: 10px;"></i>
        <span>${message}</span>
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    document.getElementById("toast-container").appendChild(toast);

    // Auto-remove after 5 seconds
    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 150);
    }, 5000);
}

// Add CSS animation
if (!document.getElementById("notification-styles")) {
    const style = document.createElement("style");
    style.id = "notification-styles";
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);
}
