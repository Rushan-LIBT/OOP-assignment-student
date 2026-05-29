namespace WareHouseApp.People
{
    /// <summary>
    /// Standard operator user. Can manage inventory and customers,
    /// but is not allowed to manage employees.
    /// </summary>
    public class ShippingOperator : Person
    {
        public override string Role
        {
            get { return "Operator"; }
        }

        public override bool CanManageEmployees
        {
            get { return false; }
        }
    }
}
