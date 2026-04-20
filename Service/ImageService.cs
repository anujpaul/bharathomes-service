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
}