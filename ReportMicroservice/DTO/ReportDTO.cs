using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ReportMicroservice.DTO
{
    public class ReportDTO
    {
    public int ReportId { get; set; }
    public string Title { get; set; }
    public DateTime DateGenerated { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public ICollection<TransactionDTO> Transactions { get; set; }
    public ICollection<SupplierDTO> Suppliers { get; set; }
    
    }
}