using System.ComponentModel.DataAnnotations;

namespace PM.Core;

public class StorageOptions
{
    [Required(ErrorMessage = "Connection string to storage is required settings")]
    public required string ConnectionString { get; init; }

    [Required(ErrorMessage = "Container name is required settings")]
    public required string Container { get; init; }

    [Required(ErrorMessage = "Settings container name is required settings")]
    public required string SettingsContainer { get; init; }
}