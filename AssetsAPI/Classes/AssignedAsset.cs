namespace AssetsAPI.Classes
{
    public class AssignedAsset
    {
        public int id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }

        public int AssetId { get; set; }    
        public Asset Asset { get; set; }

        public DateTime assignedDate { get; set; }  
        public DateTime expireDate { get; set; }
        public string status { get; set; } //Pending or completed
    }
}
