# ⚖️ TCP Load Balancer (.NET)

A lightweight, software-based Layer 4 TCP load balancer built in C# using .NET. This load balancer distributes incoming TCP traffic across multiple backend services, monitors their health, and removes unhealthy backends from the pool.

---

## 🚀 Features

### ✅ Accepts traffic from multiple clients
- Uses an asynchronous `TcpListener` to accept and handle **concurrent TCP connections**.
- Each connection is handled in a **non-blocking Task**, allowing the load balancer to scale efficiently with multiple simultaneous clients.

### ✅ Balances traffic across multiple backend services
- Implements a **pluggable backend selection strategy** via `IBackendSelector`.
- The default strategy uses **Least Connections** — routes traffic to the backend with the fewest active connections.

### ✅ Automatically removes offline services
- Background health checks (via `TcpHealthChecker`) periodically check the availability of each backend.
- Unhealthy backends are **excluded from routing decisions** until they recover.

### ✅ Exponential backoff for retrying unhealthy backends
- When a backend is marked unhealthy, it is retried with an **exponential backoff delay** before it's reconsidered for health checks.
- Prevents hammering services that are temporarily down and promotes graceful recovery.

### ✅ Console logging for observability
- Logs major events such as:
  - Client connections
  - Backend selection
  - Health check results (up/down)
  - Errors or exceptions
- Allows you to observe system behavior in real-time.

---

## 🛠️ How to Run Locally

### ✅ Requirements

- [.NET 6 SDK or newer](https://dotnet.microsoft.com/download)

### 📦 1. Clone the Repository

```bash
git clone https://github.com/your-username/TcpLoadBalancer.git
cd TcpLoadBalancer

### ⚙️ 2. Configure the Backend Servers
In Program.cs, define the backend servers that will receive traffic.
You can test with mock backends or create your own TCP services

### ▶️ 3. Run the Load Balancer
From the solution root:
dotnet run --project TcpLoadBalancer

This will:

Start the TCP listener (default port: 5000)

Start health checks on all backends

Begin accepting client traffic and load balancing across available, healthy backends