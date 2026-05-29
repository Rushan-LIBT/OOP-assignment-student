using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// A single stock-in or stock-out movement recorded against a material.
    /// </summary>
    public class StockMovement
    {
        public int MovementID { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public string MovementType { get; set; }   // "IN" or "OUT"
        public int Quantity { get; set; }
        public string MovedBy { get; set; }
        public DateTime MovedAt { get; set; }
    }
}
