using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// A login account stored in the Users table.
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
