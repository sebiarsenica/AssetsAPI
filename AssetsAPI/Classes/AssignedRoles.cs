namespace AssetsAPI.Classes
{
    public class AssignedRoles
    {
        public int id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
