# DotaStats Microservices Ecosystem
![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![Redis](https://img.shields.io/badge/Redis-Enabled-red)
![Status](https://img.shields.io/badge/Project-Active-success)
## Project Overview  
DotaStats is a scalable microservices-based ecosystem for aggregating and serving Dota 2 statistics (Heroes, Players, Teams) using the OpenDota API. The system is built on modern .NET 9 with containerization, caching, structured logging and a Telegram Bot interface.

## Technology Stack  
- C#  
- .NET 9  
- ASP.NET Core Web API  
- YARP (Yet Another Reverse Proxy) as API Gateway  
- Docker & Docker Compose  
- Redis (distributed caching)  
- Entity Framework Core  
- Serilog (structured logging)  
- Telegram.Bot API  
- Swagger / OpenAPI  

## Architecture  
- Independent microservices for **Heroes**, **ProPlayers**, and **ProTeams**.  
- A unified **API Gateway** implemented with YARP to route incoming requests to appropriate services.  
- All services are containerized and orchestrated via Docker Compose.  
- Redis is used to cache responses and reduce latency/external API calls.  
- Logging and observability with Serilog: console and file sinks, correlation IDs, request tracing.  
- Telegram Bot client that interacts with the backend ecosystem to serve real-time stats.

## Getting Started  
### Prerequisites  
- [.NET 9 SDK](https://dotnet.microsoft.com)  
- [Docker](https://www.docker.com) & [Docker Compose](https://docs.docker.com/compose/)  
- [Redis](https://redis.io) (if running outside Docker)  

### Running locally  
1. Clone the repository:  
   ```bash
   git clone https://github.com/oket23/DotaStats.git
   cd DotaStats
   ```
2. Build and run the system using Docker Compose
   ```bash
   docker-compose up --build
   ```
3. Open the API Gateway in browser to inspect Swagger UI
   ```bash
   http://localhost:5000/swagger
   ```
4. Interact with the Telegram Bot
   - Register your bot via **BotFather**
   - Update the `appsettings.json` or **environment variables** with your Telegram Bot token and gateway URL
   - Start the bot service:
   
   ```bash
   cd TelegramBot
   dotnet run
   ```

## Docker & Deployment

- The `docker-compose.yml` defines the containers for each microservice, the API Gateway, Redis, and the Telegram Bot.
- Services are built from their respective Dockerfiles.
- Use environment variables or a `.env` file to configure:
  - connection strings
  - API keys
  - Redis endpoints
  - Telegram token
- For production, use orchestration platforms (Kubernetes, AWS ECS, Azure AKS) & secret management.

## Caching & Performance

- Redis is used to store frequently accessed results from the OpenDota API to minimise latency.
- Cache policies:
  - hero stats cached for X minutes  
  - player/team stats cached for Y minutes
- Gateway implements request throttling and routing logic via YARP configuration.

## Logging & Observability

- Structured logging using **Serilog**, including:
  - timestamps  
  - correlation IDs  
  - request path  
  - user-agent
- File rolling sinks and console output.
- Metrics and tracing (can be extended using **Prometheus + Grafana**).

## Telegram Bot Integration

- The bot connects to the gateway’s endpoints to fetch data on-demand  
  *(e.g., `/hero Zeus`, `/team OG`)*  
- Designed for rapid responses and fallback logic if the external API is unavailable.

## Key Features

- Microservices architecture with clearly defined bounded contexts.
- API Gateway using **YARP** for routing, versioning and unified surface.
- Containerised deployment via **Docker Compose**.
- Distributed caching with **Redis** for high performance.
- End-to-end logging and observability.
- Telegram Bot client for real-time user interaction.

## Project Structure (example)

```bash
/DotaStats
  /Gateway
  /HeroesService
  /ProPlayersService
  /ProTeamsService
  /TelegramBot
  /docker-compose.yml
  /README.md
```

## Next Steps / Improvements

- Add automated tests (unit/integration) for each microservice.
- Introduce message queue (e.g., RabbitMQ) for asynchronous processing.
- Deploy to cloud environment (e.g., Azure AKS with Helm charts).
- Extend Telegram Bot with interactive menus and webhooks.
- Add monitoring dashboard (**Prometheus + Grafana + Loki**) for metrics and logs.

