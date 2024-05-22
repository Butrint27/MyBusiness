using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;

namespace MyBusiness.TransactionMicroservice.DTO
{
    public class TransactionDTO
    {
    public int TransactionId { get; set; }
    public int ProductId { get; set; } // Changed to int
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }

    }
}