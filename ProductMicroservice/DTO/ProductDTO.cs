using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ProductMicroservice.DTO
{
    public class ProductDTO
    {
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int SupplierId { get; set; }
    
    }
}