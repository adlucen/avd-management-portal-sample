# AVD Management Portal — Sample

A C# (.NET 8) snippet demonstrating how to enumerate Azure Virtual Desktop host pools across a subscription and aggregate session host health using the Azure Resource Manager SDK.

## What it shows
- Authenticating to Azure with `DefaultAzureCredential` (managed identity / Azure CLI / env vars — no secrets in code)
- Listing all AVD host pools in a subscription
- Counting available vs. unavailable session hosts and active sessions per pool

## Part of a larger solution
This snippet is extracted from a full **Blazor Server portal** for AVD operations, which includes host pool management, image lifecycle orchestration via Azure Image Builder, actionable telemetry from Log Analytics, and user preferences stored in Cosmos DB — all secured with Azure AD.

## Stack
`.NET 8` · `Blazor Server` · `Azure SDK (ResourceManager)` · `DefaultAzureCredential` · `Azure Virtual Desktop` · `Azure Monitor` · `Cosmos DB` · `Azure Functions` · `Bicep/ARM`
