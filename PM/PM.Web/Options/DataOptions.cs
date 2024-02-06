using System.ComponentModel.DataAnnotations;

namespace PM.Web.Options;

public class DataOptions
{
    [Required(ErrorMessage = "The ConnectionString field setting is required.")]
    public string ConnectionString { get; set; }
}