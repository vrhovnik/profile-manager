using System.ComponentModel.DataAnnotations;

namespace PM.Core;

public class AzureAdOptions
{
    public string Instance { get; init; } = "https://login.microsoftonline.com/";

    [Required(ErrorMessage = "Domain is required")]
    public required string Domain { get; init; }

    [Required(ErrorMessage = "TenantId is required")]
    public required string TenantId { get; init; }

    [Required(ErrorMessage = "ClientId is required")]
    public required string ClientId { get; init; }

    [Required(ErrorMessage = "CallbackPath is required")]
    public required string CallbackPath { get; init; }

    public required string SignedOutCallbackPath { get; init; }

    [Required(ErrorMessage = "ClientSecret is required")]
    public required string Secret { get; init; }

    [Required(ErrorMessage = "Azure SubscriptionId is required")]
    public required string SubscriptionId { get; init; }
}