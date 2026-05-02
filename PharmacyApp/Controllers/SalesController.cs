using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Models;
using PharmacyApp.Services;

namespace PharmacyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: api/sales
        [HttpGet]
        public async Task<ActionResult<List<SaleRecord>>> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        // GET: api/sales/medicine/{medicineId}
        [HttpGet("medicine/{medicineId}")]
        public async Task<ActionResult<List<SaleRecord>>> GetByMedicine(string medicineId)
        {
            var sales = await _saleService.GetSalesByMedicineIdAsync(medicineId);
            return Ok(sales);
        }

        // POST: api/sales
        [HttpPost]
        public async Task<ActionResult<SaleRecord>> Create([FromBody] SaleRecord sale)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _saleService.AddSaleAsync(sale);

            if (created == null)
                return BadRequest(new
                {
                    message = "Sale could not be processed. " +
                              "Medicine not found or insufficient stock."
                });

            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
