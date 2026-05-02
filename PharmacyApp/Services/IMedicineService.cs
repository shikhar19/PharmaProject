using PharmacyApp.Models;

namespace PharmacyApp.Services
{
    public interface IMedicineService
    {
        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine?> GetMedicineByIdAsync(string id);
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine?> UpdateMedicineAsync(string id, Medicine medicine);
        Task<bool> DeleteMedicineAsync(string id);
        Task<List<Medicine>> SearchMedicinesAsync(string query);
        Task<bool> UpdateQuantityAsync(string id, int quantityChange);
    }
}
