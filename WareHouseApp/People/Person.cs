using System;

namespace WareHouseApp.People
{
    /// <summary>
    /// Abstract base for every authenticated user of the system.
    /// Demonstrates abstraction and encapsulation (private password field).
    /// </summary>
    public abstract class Person
    {
        public int EmpID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>Role label, overridden by each concrete user type (polymorphism).</summary>
        public abstract string Role { get; }

        /// <summary>True when this user type may manage other employees.</summary>
        public abstract bool CanManageEmployees { get; }

        public virtual string WelcomeMessage()
        {
            return "Welcome, " + (string.IsNullOrEmpty(FullName) ? UserName : FullName) + " (" + Role + ")";
        }

        public override string ToString()
        {
            return UserName + " - " + Role;
        }
    }
}
