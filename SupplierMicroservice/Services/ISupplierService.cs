using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.SupplierMicroservice.DTO;

namespace MyBusiness.SupplierMicroservice.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDTO>> GetAllSuppliersAsync();
        Task<SupplierDTO> GetSupplierByIdAsync(int supplyId);
        Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDTO);
        Task<SupplierDTO> UpdateSupplierAsync(SupplierDTO supplierDTO);
        Task<bool> DeleteSupplierAsync(int supplyId);
    }
}