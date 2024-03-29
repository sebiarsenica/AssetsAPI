﻿namespace AssetsAPI.Classes
{
    public class UserReturn
    {
        public int id { get; set; }

        public string username { get; set; } = string.Empty;

        public string fullName { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public DateTime createDate { get; set; }
    }
}
