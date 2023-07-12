namespace AssetsAPI.DTOs
{
    public class AssignedAssetDTO
    {
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public DateTime expireDate { get; set; }
    }
}
