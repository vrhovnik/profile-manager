using System.Diagnostics;
using System.Net;
using Microsoft.Azure.Cosmos;
using PM.Core;
using PM.Data.Azure.Entities;
using PM.Interfaces;

namespace PM.Data.Azure;

public class DataService<T>(DataOptions dataOptions) : IDataRepository<T>
    where T : CosmosEntity
{
    protected CosmosClient Client =>
        new(dataOptions.ConnectionString, new CosmosClientOptions()
        {
            ApplicationName = "PM"
        });

    public virtual Task<PaginatedList<T>> SearchAsync(int page, int pageSize, string query = "")
    {
        throw new NotImplementedException();
    }

    public virtual Task<PaginatedList<T>> GetAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<T>> GetAsync()
    {
        var data = Client.GetContainer(dataOptions.DatabaseName, nameof(T))
            .GetItemLinqQueryable<T>();
        return Task.FromResult(data.ToList());
    }

    public virtual async Task<bool> DeleteAsync(string entityId)
    {
        try
        {
            await Client.GetContainer(dataOptions.DatabaseName, nameof(T))
                .DeleteItemAsync<T>(entityId, new PartitionKey("partitionkey"));
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return false;
        }
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            await Client.GetContainer(dataOptions.DatabaseName, nameof(T))
                .ReplaceItemAsync(entity, entity.Id);
            return true;
        }
        catch (CosmosException e)
        {
            Debug.WriteLine(e);
            return false;
        }
    }

    public virtual async Task<T> InsertAsync(T entity)
    {
        await Client.GetContainer(dataOptions.DatabaseName, nameof(T))
            .CreateItemAsync(entity);
        return entity;
    }

    public virtual async Task<T> DetailsAsync(string entityId)
    {
        return await Client.GetContainer(dataOptions.DatabaseName, nameof(T))
            .ReadItemAsync<T>(entityId, new PartitionKey("partitionkey"));
    }

    public virtual async Task<int> GetCountAsync() => 
        (await GetAsync()).Count;

    public virtual async Task<bool> IsAlive()
    {
        try
        {
            var db = Client.GetDatabase(dataOptions.DatabaseName);
            var response = await db.ReadAsync();
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (CosmosException error)
        {
            Debug.WriteLine(error);
            return false;
        }
    }
}