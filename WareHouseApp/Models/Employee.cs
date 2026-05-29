namespace WareHouseApp.Models
{
    /// <summary>
    /// A member of warehouse staff.
    /// </summary>
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }
    }
}
