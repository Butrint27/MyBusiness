using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Services;

namespace MyBusiness.SupplierMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound(); // Supplier not found
            }
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> CreateSupplier(SupplierDTO supplierDTO)
        {
            var createdSupplier = await _supplierService.CreateSupplierAsync(supplierDTO);
            return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.SupplierId }, createdSupplier);
        }

        [HttpPut]
        public async Task<ActionResult<SupplierDTO>> UpdateSupplier(SupplierDTO supplierDTO)
        {
            
            var updatedSupplier = await _supplierService.UpdateSupplierAsync(supplierDTO);
            if (updatedSupplier == null)
            {
                return NotFound(); // Supplier not found
            }
            return Ok(updatedSupplier);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteSupplier(int id)
        {
            var isDeleted = await _supplierService.DeleteSupplierAsync(id);
            if (!isDeleted)
            {
                return NotFound(); // Supplier not found
            }
            return Ok(true);
        }
    }
}