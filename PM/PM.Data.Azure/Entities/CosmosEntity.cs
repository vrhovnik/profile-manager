using Newtonsoft.Json;

namespace PM.Data.Azure.Entities;

public abstract class CosmosEntity
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
}