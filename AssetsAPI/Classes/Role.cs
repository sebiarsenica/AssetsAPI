namespace AssetsAPI.Classes
{
    public class Role
    {
        public int id { get; set; }

        public string roleName { get; set; } = string.Empty;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Role other = (Role)obj;
            return id == other.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
