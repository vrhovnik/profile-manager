using System.ComponentModel.DataAnnotations;

namespace PM.Core;

public class DataOptions
{
    [Required(ErrorMessage = "Connection string to database is required settings")]
    public required string ConnectionString { get; init; }
    public string DatabaseName { get; init; }
    public int PagingSize { get; set; } = 15;
}