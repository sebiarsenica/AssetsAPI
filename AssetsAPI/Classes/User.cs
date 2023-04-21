namespace AssetsAPI.Classes
{
    public class User
    {
        public int id { get; set; }

        public string username { get; set; } = string.Empty;

        public string fullName { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty; 

        public byte[] passwordHash { get; set; } 

        public byte[] passwordSalt { get; set; } 

        public DateTime createDate { get; set; }

        public UserReturn ConvertUserToUserReturn()
        {
            var userReturn = new UserReturn();

            userReturn.id = id;
            userReturn.username = username;
            userReturn.fullName = fullName;
            userReturn.email = email;
            userReturn.createDate = createDate;

            return userReturn;
        }
    }
}
