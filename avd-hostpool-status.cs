// ============================================================
// Sample: AVD Host Pool Status Checker
// Demonstrates: Azure SDK (ResourceManager), DefaultAzureCredential,
//               host pool enumeration, session host health aggregation
//
// NOTE: Illustrative snippet — requires Azure.ResourceManager.DesktopVirtualization
//       NuGet package, a configured Azure subscription, and appropriate RBAC.
//       Production version includes Blazor UI, SignalR notifications, and Cosmos DB
//       for user preferences. Credentials are never hardcoded.
// ============================================================

using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.DesktopVirtualization;

namespace AvdSamples;

public record HostPoolHealthSummary(
    string HostPoolName,
    string ResourceGroup,
    int AvailableHosts,
    int UnavailableHosts,
    int TotalSessions
);

public static class HostPoolStatusChecker
{
    /// <summary>
    /// Returns a health summary for every AVD host pool in the given subscription.
    /// Uses DefaultAzureCredential — works with managed identity, Azure CLI, or
    /// environment variables. No secrets in code.
    /// </summary>
    public static async Task<List<HostPoolHealthSummary>> GetAllHostPoolStatusAsync(
        string subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var credential = new DefaultAzureCredential();
        var armClient  = new ArmClient(credential);

        var subscription = armClient.GetSubscriptionResource(
            new Azure.Core.ResourceIdentifier($"/subscriptions/{subscriptionId}"));

        var results = new List<HostPoolHealthSummary>();

        await foreach (var hostPool in subscription.GetHostPoolsAsync(cancellationToken))
        {
            int available   = 0;
            int unavailable = 0;
            int sessions    = 0;

            await foreach (var sessionHost in hostPool.GetSessionHosts().GetAllAsync(cancellationToken: cancellationToken))
            {
                var status = sessionHost.Data.Status?.ToString();

                if (status == "Available")
                    available++;
                else
                    unavailable++;

                sessions += sessionHost.Data.Sessions ?? 0;
            }

            var rgName = hostPool.Id.ResourceGroupName ?? "unknown";

            results.Add(new HostPoolHealthSummary(
                HostPoolName:    hostPool.Data.Name,
                ResourceGroup:   rgName,
                AvailableHosts:  available,
                UnavailableHosts: unavailable,
                TotalSessions:   sessions
            ));
        }

        return results;
    }
}

// ---- Example usage (e.g. in a Blazor page or Azure Function) ----
//
// var summaries = await HostPoolStatusChecker.GetAllHostPoolStatusAsync(
//     subscriptionId: Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID")!);
//
// foreach (var hp in summaries)
//     Console.WriteLine($"{hp.HostPoolName}: {hp.AvailableHosts} up / {hp.UnavailableHosts} down | {hp.TotalSessions} sessions");
