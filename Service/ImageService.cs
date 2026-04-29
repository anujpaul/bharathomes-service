using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class ImageService
{
    private readonly IConfiguration _config;
    private readonly ILogger<ImageService> _logger;  
    public ImageService(IConfiguration config, ILogger<ImageService> logger)
    {
        _config = config;
        _logger = logger;
    }
    public string GetImage(string folderName, string blobName)
    {   
        string? storageAccountUrl = _config["StorageAccountName"];
        string? sasToken = _config["SasToken"];
        // https://bharathomes.blob.core.windows.net/production/images/Agents/sunny_paul.jpg
        string returnUrl = $"{storageAccountUrl}/images/{folderName}/{blobName}{sasToken}";
        _logger.LogInformation($"Return URL : {returnUrl}");
        return returnUrl;
    }

     public async Task<string> UploadImageAsync(Stream fileStream, string folderName, string fileName, string contentType)
    {
        string? connectionString = _config["StorageConnectionString"];
        string? containerName = _config["StorageContainer"];

        var containerClient = new BlobContainerClient(connectionString, containerName);
        var blobPath = $"images/{folderName}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
        {
            ContentType = contentType
        }); 

        _logger.LogInformation($"Uploaded image to: {blobPath}");

        // Return using GetImage so SAS token is included
        return GetImage(folderName, fileName);
    }
}