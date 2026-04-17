using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class CosmosDbService
{
    private IConfiguration _config;
    Container _container;
    public CosmosDbService(IConfiguration config, Container container)
    {
        _config = config;
        _container = container;

    }

    public async Task<List<T>> ReadItemsAsync<T>(string modelType)
    {        
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

    public async Task<T> ReadItemAsync<T>(string modelType, string id)
    {
        
        // var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.modeltype = @type")
        // .WithParameter("@type", modelType);

        // var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

        var result = await _container.ReadItemAsync<T>(id, new PartitionKey(id));


        // var results = new List<T>();

        // while (feedIterator.HasMoreResults)
        // {
        //     FeedResponse<T> response = await feedIterator.ReadNextAsync();
        //     results.AddRange(response.ToList());
        // }
        return result;
    }

    public async Task<string> CreateItemAsyc<T>(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        dynamic dynamicItem = item;

        var response = await _container.CreateItemAsync<T>(item, new PartitionKey(dynamicItem.Id));
    
        return $"Response Status : {response.StatusCode}";
    }

    public async Task<string> DeleteItemAsync<T>(string modelType, string id)
    {
        var response = await _container.DeleteItemAsync<T>(id, new PartitionKey(id));

        return $"{modelType} with {id} Deleted status : {response.StatusCode}";
        
    }

}