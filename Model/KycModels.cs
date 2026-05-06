namespace bharathome_api.Model
{
    public class PanVerificationResult
    {
        public bool IsValid { get; set; }
        public string NameOnPan { get; set; } = "";
        public bool NameMatch { get; set; }
    }

    public class SurepassPanResponse
    {
        public SurepassPanData? Data { get; set; }
    }

    public class SurepassPanData
    {
        public bool Valid { get; set; }
        public string Name { get; set; } = "";
    }
}
