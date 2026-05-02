using PharmacyApp.Models;

namespace PharmacyApp.Services
{
    public interface ISaleService
    {
        Task<List<SaleRecord>> GetAllSalesAsync();
        Task<List<SaleRecord>> GetSalesByMedicineIdAsync(string medicineId);
        Task<SaleRecord?> AddSaleAsync(SaleRecord sale);
    }
}
