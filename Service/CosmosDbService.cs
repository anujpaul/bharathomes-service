using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class CosmosDbService
{
    private IConfiguration _config;

    private readonly ILogger<CosmosDbService> _logger;
    Container _container;
    public CosmosDbService(IConfiguration config, Container container, ILogger<CosmosDbService> logger)
    {
        _config = config;
        _container = container;
        _logger = logger;

    }

    public async Task<List<T>> ReadItemsAsync<T>()
    {        
        
        string modelType = typeof(T).GetProperty("ModelType")?.GetValue(null)?.ToString()?? typeof(T).Name;

        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.modeltype = @type")
        .WithParameter("@type", modelType);

        var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

        var results = new List<T>();

        while (feedIterator.HasMoreResults)
        {
            FeedResponse<T> response = await feedIterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<T> ReadItemAsync<T>(string id)
    {
        
        try
        {
            string modelType = typeof(T).GetProperty("ModelType")?.GetValue(null)?.ToString()?? typeof(T).Name;
            var result = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
            return result;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default! ;
        }
    }
    

    public async Task<string> CreateItemAsync<T>(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        dynamic dynamicItem = item;

        var response = await _container.CreateItemAsync<T>(item, new PartitionKey(dynamicItem.Id));
    
        return $"Response Status : {response.StatusCode}";
    }

    public async Task<string> DeleteItemAsync<T>(string id)
    {
         string modelType = typeof(T).GetProperty("ModelType")?.GetValue(null)?.ToString()?? typeof(T).Name;

        var response = await _container.DeleteItemAsync<T>(id, new PartitionKey(id));

        return $"{modelType} with {id} Deleted status : {response.StatusCode}";
        
    }

    public async Task<T?> ReadItemByEmailAsync<T>(string email)
    {

        string modelType = typeof(T).GetProperty("ModelType")?.GetValue(null)?.ToString()?? typeof(T).Name;

        _logger.LogInformation($"Searching for Email: {email}");
        _logger.LogInformation($"Model Type: {modelType}");

        var queryDefinition = new QueryDefinition(
                                        "SELECT * FROM c WHERE c.modeltype = @type AND c.email = @email")
                                        .WithParameter("@type", modelType)
                                        .WithParameter("@email", email);

        var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

        
        _logger.LogInformation($"Reached here");
        if (feedIterator.HasMoreResults)
        {
            var response = await feedIterator.ReadNextAsync();
            _logger.LogInformation($"Response");
            return response.FirstOrDefault(); // Returns null if no item found
            
        }
        _logger.LogInformation($"Returning empty");
        return default; // Returns null for reference types

    }

    internal async Task<T> UpdateItemAsync<T>(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        dynamic dynamicItem = item;

        var response = await _container.ReplaceItemAsync<T>(
            item,
            dynamicItem.Id,
            new PartitionKey(dynamicItem.Id)
        );

        return response.Resource;
    }
}