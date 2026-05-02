using PharmacyApp.Models;
using System.Text.Json;

namespace PharmacyApp.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public MedicineService(IWebHostEnvironment env)
        {
            // Store JSON in the Data folder inside the project
            var dataFolder = Path.Combine(env.ContentRootPath, "Data");

            // Create directory if it doesn't exist
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _filePath = Path.Combine(dataFolder, "medicines.json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Seed initial data if file doesn't exist
            if (!File.Exists(_filePath))
                SeedDataAsync().Wait();
        }

        // ─── Private Helpers ────────────────────────────────────────────

        private async Task<List<Medicine>> ReadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Medicine>();

            var json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Medicine>();

            return JsonSerializer.Deserialize<List<Medicine>>(json, _jsonOptions)
                   ?? new List<Medicine>();
        }

        private async Task WriteToFileAsync(List<Medicine> medicines)
        {
            var json = JsonSerializer.Serialize(medicines, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }

        private async Task SeedDataAsync()
        {
            var seedData = new List<Medicine>
            {
                new Medicine
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Paracetamol 500mg",
                    Notes = "Common pain reliever and fever reducer",
                    ExpiryDate = DateTime.UtcNow.AddDays(180),
                    Quantity = 150,
                    Price = 5.99m,
                    Brand = "HealthCare Plus",
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Amoxicillin 250mg",
                    Notes = "Antibiotic for bacterial infections",
                    ExpiryDate = DateTime.UtcNow.AddDays(25), // Near expiry - RED
                    Quantity = 45,
                    Price = 12.50m,
                    Brand = "MediCore",
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Ibuprofen 400mg",
                    Notes = "Anti-inflammatory pain reliever",
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    Quantity = 8, // Low stock - YELLOW
                    Price = 7.25m,
                    Brand = "PharmaCo",
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Metformin 500mg",
                    Notes = "Used to treat type 2 diabetes",
                    ExpiryDate = DateTime.UtcNow.AddDays(365),
                    Quantity = 200,
                    Price = 9.99m,
                    Brand = "DiabeCare",
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Cetirizine 10mg",
                    Notes = "Antihistamine for allergy relief",
                    ExpiryDate = DateTime.UtcNow.AddDays(15), // Near expiry - RED
                    Quantity = 5,  // Low stock - YELLOW (both conditions)
                    Price = 4.75m,
                    Brand = "AllerFree",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await WriteToFileAsync(seedData);
        }


        public async Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return await ReadFromFileAsync();
        }

        public async Task<Medicine?> GetMedicineByIdAsync(string id)
        {
            var medicines = await ReadFromFileAsync();
            return medicines.FirstOrDefault(m => m.Id == id);
        }

        public async Task<Medicine> AddMedicineAsync(Medicine medicine)
        {
            var medicines = await ReadFromFileAsync();

            medicine.Id = Guid.NewGuid().ToString();
            medicine.CreatedAt = DateTime.UtcNow;

            medicines.Add(medicine);
            await WriteToFileAsync(medicines);

            return medicine;
        }

        public async Task<Medicine?> UpdateMedicineAsync(string id, Medicine updatedMedicine)
        {
            var medicines = await ReadFromFileAsync();
            var index = medicines.FindIndex(m => m.Id == id);

            if (index == -1)
                return null;

            updatedMedicine.Id = id;
            updatedMedicine.CreatedAt = medicines[index].CreatedAt;
            medicines[index] = updatedMedicine;

            await WriteToFileAsync(medicines);
            return updatedMedicine;
        }

        public async Task<bool> DeleteMedicineAsync(string id)
        {
            var medicines = await ReadFromFileAsync();
            var medicine = medicines.FirstOrDefault(m => m.Id == id);

            if (medicine == null)
                return false;

            medicines.Remove(medicine);
            await WriteToFileAsync(medicines);
            return true;
        }

        public async Task<List<Medicine>> SearchMedicinesAsync(string query)
        {
            var medicines = await ReadFromFileAsync();

            if (string.IsNullOrWhiteSpace(query))
                return medicines;

            return medicines
                .Where(m => m.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<bool> UpdateQuantityAsync(string id, int quantityChange)
        {
            var medicines = await ReadFromFileAsync();
            var medicine = medicines.FirstOrDefault(m => m.Id == id);

            if (medicine == null)
                return false;

            // Ensure quantity doesn't go below zero
            if (medicine.Quantity + quantityChange < 0)
                return false;

            medicine.Quantity += quantityChange;
            await WriteToFileAsync(medicines);
            return true;
        }
    }
}
