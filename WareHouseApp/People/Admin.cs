namespace WareHouseApp.People
{
    /// <summary>
    /// Administrator user. Has full access including employee management.
    /// </summary>
    public class Admin : Person
    {
        public override string Role
        {
            get { return "Admin"; }
        }

        public override bool CanManageEmployees
        {
            get { return true; }
        }
    }
}
