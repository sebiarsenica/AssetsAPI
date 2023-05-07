namespace AssetsAPI.DTOs
{
    public class AssetDTO
    {
        public string name { get; set; }
        public string category { get; set; }
        public int quantity { get; set; }
        public string addedBy { get; set; }
        public byte[] image { get; set; }    
    }
}
