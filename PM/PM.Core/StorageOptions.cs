using System.ComponentModel.DataAnnotations;

namespace PM.Core;

public class StorageOptions
{
    [Required(ErrorMessage = "Connection string to storage is required settings")]
    public string ConnectionString { get; set; }

    [Required(ErrorMessage = "Container name is required settings")]
    public string Container { get; set; }

    [Required(ErrorMessage = "Settings container name is required settings")]
    public string SettingsContainer { get; set; }
}