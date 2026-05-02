using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Models;
using PharmacyApp.Services;

namespace PharmacyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: api/medicines
        [HttpGet]
        public async Task<ActionResult<List<Medicine>>> GetAll([FromQuery] string? search)
        {
            List<Medicine> medicines;

            if (!string.IsNullOrWhiteSpace(search))
                medicines = await _medicineService.SearchMedicinesAsync(search);
            else
                medicines = await _medicineService.GetAllMedicinesAsync();

            return Ok(medicines);
        }

        // GET: api/medicines/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicine>> GetById(string id)
        {
            var medicine = await _medicineService.GetMedicineByIdAsync(id);

            if (medicine == null)
                return NotFound(new { message = $"Medicine with ID {id} not found" });

            return Ok(medicine);
        }

        // POST: api/medicines
        [HttpPost]
        public async Task<ActionResult<Medicine>> Create([FromBody] Medicine medicine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _medicineService.AddMedicineAsync(medicine);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/medicines/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Medicine>> Update(string id, [FromBody] Medicine medicine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _medicineService.UpdateMedicineAsync(id, medicine);

            if (updated == null)
                return NotFound(new { message = $"Medicine with ID {id} not found" });

            return Ok(updated);
        }

        // DELETE: api/medicines/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await _medicineService.DeleteMedicineAsync(id);

            if (!deleted)
                return NotFound(new { message = $"Medicine with ID {id} not found" });

            return NoContent();
        }
    }
}
