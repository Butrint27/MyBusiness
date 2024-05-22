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
    public int TransactionId { get; set; } // Changed to int
    public DateTime ReportDate { get; set; }
    public string Details { get; set; }
    
    }
}