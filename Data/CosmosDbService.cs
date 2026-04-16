using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

public class CosmosDbService
{
    private IConfiguration _config;
    CosmosClient _cosmosClient;
    public CosmosDbService(IConfiguration config, CosmosClient cosmosClient)
    {
        _config = config;
        _cosmosClient = cosmosClient;

    }

    public async Task<List<T>> ReadItemAsync<T>(string databaseName, string containerName, string modelType)
    {
        var containerClient = _cosmosClient.GetContainer(databaseName, containerName);
        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.modeltype = @type")
        .WithParameter("@type", modelType);

        var feedIterator = containerClient.GetItemQueryIterator<T>(queryDefinition);

        var results = new List<T>();

        while (feedIterator.HasMoreResults)
        {
            FeedResponse<T> response = await feedIterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<string> CreateItemAsyc<T>(string databaseName, string containerName,T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        dynamic dynamicItem = item;

        var containerClient = _cosmosClient.GetContainer(databaseName, containerName);
        var response = await containerClient.CreateItemAsync<T>(item, new PartitionKey(dynamicItem.Id));

    
        return $"Response Status : {response.StatusCode}";
    }
}