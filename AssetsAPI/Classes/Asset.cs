namespace AssetsAPI.Classes
{
    public class Asset
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string category { get; set; }
        public int quantity { get; set; }
        public string addedBy { get; set; }
        public byte[] image { get; set; }
    }
}
