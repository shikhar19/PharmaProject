namespace PharmacyApp.Models
{
    public class SaleRecord
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MedicineId { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public string CustomerName { get; set; } = string.Empty;
    }
}
