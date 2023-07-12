namespace AssetsAPI.Classes
{
    public class AssignedAssetReturn
    {
        public int id { get; set; } 
        public UserReturn user { get; set; }
        public Asset asset { get; set; }

        public DateTime assignedDate { get; set; }
        public DateTime expireDate { get; set; }
        public string status { get; set; }
    }
}
