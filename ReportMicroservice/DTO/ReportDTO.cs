using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ReportMicroservice.DTO
{
    public class ReportDTO
    {
    public int ReportId { get; set; }
    public string Title { get; set; }
    public DateTime DateGenerated { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public int SupplierId { get; set; }
    public SupplierDTO Supplier { get; set; }
    public List<TransactionDTO> Transactions { get; set; }
    public List<ProductDTO> Products { get; set; }
    }
}