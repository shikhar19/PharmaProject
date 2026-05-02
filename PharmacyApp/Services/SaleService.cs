using PharmacyApp.Models;
using System.Text.Json;

namespace PharmacyApp.Services
{
    public class SaleService : ISaleService
    {
        private readonly string _filePath;
        private readonly IMedicineService _medicineService;
        private readonly JsonSerializerOptions _jsonOptions;

        public SaleService(IWebHostEnvironment env, IMedicineService medicineService)
        {
            _medicineService = medicineService;

            var dataFolder = Path.Combine(env.ContentRootPath, "Data");
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _filePath = Path.Combine(dataFolder, "sales.json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        private async Task<List<SaleRecord>> ReadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new List<SaleRecord>();

            var json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<SaleRecord>();

            return JsonSerializer.Deserialize<List<SaleRecord>>(json, _jsonOptions)
                   ?? new List<SaleRecord>();
        }

        private async Task WriteToFileAsync(List<SaleRecord> sales)
        {
            var json = JsonSerializer.Serialize(sales, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<SaleRecord>> GetAllSalesAsync()
        {
            var sales = await ReadFromFileAsync();
            // Return most recent sales first
            return sales.OrderByDescending(s => s.SaleDate).ToList();
        }

        public async Task<List<SaleRecord>> GetSalesByMedicineIdAsync(string medicineId)
        {
            var sales = await ReadFromFileAsync();
            return sales
                .Where(s => s.MedicineId == medicineId)
                .OrderByDescending(s => s.SaleDate)
                .ToList();
        }

        public async Task<SaleRecord?> AddSaleAsync(SaleRecord sale)
        {
            // Validate medicine exists
            var medicine = await _medicineService.GetMedicineByIdAsync(sale.MedicineId);
            if (medicine == null)
                return null;

            // Check sufficient stock
            if (medicine.Quantity < sale.QuantitySold)
                return null;

            // Set computed fields
            sale.Id = Guid.NewGuid().ToString();
            sale.MedicineName = medicine.FullName;
            sale.PricePerUnit = medicine.Price;
            sale.TotalAmount = medicine.Price * sale.QuantitySold;
            sale.SaleDate = DateTime.UtcNow;

            // Deduct from inventory
            await _medicineService.UpdateQuantityAsync(sale.MedicineId, -sale.QuantitySold);

            // Save sale record
            var sales = await ReadFromFileAsync();
            sales.Add(sale);
            await WriteToFileAsync(sales);

            return sale;
        }
    }
}