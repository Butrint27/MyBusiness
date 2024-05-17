using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ProductMicroservice.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int productId);
        Task<ProductDTO> CreateProductAsync(ProductDTO productDTO);
        Task<ProductDTO> UpdateProductAsync(ProductDTO productDTO);
        Task<bool> DeleteProductAsync(int productId);

    }
}