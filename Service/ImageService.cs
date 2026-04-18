public class ImageService
{
    private IConfiguration _config;

    public ImageService(IConfiguration config)
    {
        _config = config;
    }
    public string GetImage(string folderName, string blobName)
    {   
        string? storageAccountUrl = _config["StorageAccountName"];
        string? sasToken = _config["SasToken"];
        // https://bharathomes.blob.core.windows.net/production/images/Agents/sunny_paul.jpg
        string returnUrl = $"{storageAccountUrl}/images/{folderName}/{blobName}{sasToken}";
        System.Console.WriteLine($"Return URL : {returnUrl}");
        return returnUrl;
    }
}