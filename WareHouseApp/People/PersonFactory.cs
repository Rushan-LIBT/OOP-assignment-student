using WareHouseApp.Models;

namespace WareHouseApp.People
{
    /// <summary>
    /// Creates the correct concrete <see cref="Person"/> subtype for a stored user.
    /// Centralising this keeps the role-to-type mapping in one place (polymorphism).
    /// </summary>
    public static class PersonFactory
    {
        public static Person FromUser(User user)
        {
            Person person;
            if (user.Role == "Admin")
            {
                person = new Admin();
            }
            else
            {
                person = new ShippingOperator();
            }

            person.EmpID = user.UserID;
            person.UserName = user.Username;
            person.FullName = user.FullName;
            return person;
        }
    }
}
