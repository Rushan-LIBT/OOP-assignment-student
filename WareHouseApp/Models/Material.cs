namespace WareHouseApp.Models
{
    /// <summary>
    /// An inventory item held in the warehouse.
    /// </summary>
    public class Material
    {
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalValue
        {
            get { return Quantity * UnitPrice; }
        }
    }
}
