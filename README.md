# SoloHash Worker

A .NET 8 background worker service that monitors [ckpool](https://bitbucket.org/ckolivas/ckpool) log files in real-time and persists mining statistics to AWS DynamoDB.

## Overview

SoloHash Worker uses `FileSystemWatcher` to detect changes to ckpool log files and parses three categories of data:

| Log Type | Source | What it tracks |
|---|---|---|
| **Pool** | `/ckpool/logs/pool/*` | Pool runtime status, hashrate, and statistics |
| **User** | `/ckpool/logs/users/*` | Per-user status and 5-minute hashrate |
| **PoolSystem** | `/ckpool/logs/*` | Block solve events (block height + winning address) |

Parsed data is saved to DynamoDB using the AWS SDK.

## Architecture

```
Program.cs          → Builds the .NET Generic Host, wires up DI
App.cs              → Starts a LogWatcherService for each log directory
LogWatcherService   → FileSystemWatcher + JSON/regex parsing
DynamoDbService     → Persists pool stats, user stats, hashrate history, and block records
```

### DynamoDB records written

- **Pool stats** – `PoolStats` item keyed on `"pool"` / `StatsType.Pool`
- **User stats** – `UserStats` item keyed on the user's filename (Bitcoin address)
- **User hashrate** – `UserHashrateStats` time-series record with a 24-hour TTL
- **User blocks** – `UserBlockStats` list of solved blocks per Bitcoin address

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- AWS credentials with read/write access to DynamoDB (region: `eu-west-2`)
- A running ckpool instance writing logs to the expected paths

## Configuration

Settings are read from `appsettings.json` and can be overridden with environment variables.

```json
{
  "LogWatcherOptions": {
    "UserDirectoryPath": "/ckpool/logs/users",
    "UserFilter": "*",
    "PoolDirectoryPath": "/ckpool/logs/pool",
    "PoolFilter": "*",
    "PoolSystemDirectoryPath": "/ckpool/logs",
    "PoolSystemFilter": "*"
  }
}
```

| Key | Default | Description |
|---|---|---|
| `UserDirectoryPath` | `/ckpool/logs/users` | Directory containing per-user JSON stat files |
| `UserFilter` | `*` | File glob filter for user log files |
| `PoolDirectoryPath` | `/ckpool/logs/pool` | Directory containing pool JSON stat files |
| `PoolFilter` | `*` | File glob filter for pool log files |
| `PoolSystemDirectoryPath` | `/ckpool/logs` | Directory containing the ckpool system log |
| `PoolSystemFilter` | `*` | File glob filter for the system log |

AWS credentials are resolved via the standard AWS credential chain (environment variables, `~/.aws/credentials`, IAM role, etc.).

## Running locally

```bash
cd src
dotnet restore
dotnet run
```

## Docker

Build and run with Docker:

```bash
# Build
docker build -t solohash-worker --build-arg ENVIRONMENT=Production .

# Run (mount the ckpool log directory and pass AWS credentials)
docker run --rm \
  -v /path/to/ckpool/logs:/ckpool/logs \
  -e AWS_ACCESS_KEY_ID=<key> \
  -e AWS_SECRET_ACCESS_KEY=<secret> \
  solohash-worker
```

The `ENVIRONMENT` build argument sets `ASPNETCORE_ENVIRONMENT` inside the container, enabling environment-specific `appsettings.<Environment>.json` overrides.

## Project structure

```
SoloHash-Worker.sln
src/
├── Program.cs                   # Host bootstrap & DI registration
├── App.cs                       # Entry point – starts all watchers
├── LogWatcherType.cs            # Enum: User | Pool | PoolSystem
├── CustomConsoleFormatter.cs    # Timestamped console log formatter
├── appsettings.json
├── Options/
│   └── LogWatcherOptions.cs
├── Models/
│   ├── HashrateItem.cs
│   ├── PoolStats.cs
│   ├── StatsItem.cs
│   ├── StatsType.cs
│   ├── Pool/
│   └── User/
└── Services/
    ├── DynamoDbService/
    │   ├── IDynamoDbService.cs
    │   └── DynamoDbService.cs
    └── LogParserService/
        ├── ILogWatcherService.cs
        └── LogWatcherService.cs
```

## Dependencies

| Package | Version |
|---|---|
| `AWSSDK.DynamoDBv2` | 3.7.x |
| `Microsoft.Extensions.DependencyInjection` | 8.0.0 |
| `Microsoft.Extensions.Hosting` | 8.0.0 |
