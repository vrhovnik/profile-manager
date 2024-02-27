using System.ComponentModel.DataAnnotations;

namespace PM.Core;

public class DataOptions
{
    [Required(ErrorMessage = "Connection string to database is required settings")]
    public string ConnectionString { get; set; }
    public int PagingSize { get; set; } = 15;
}