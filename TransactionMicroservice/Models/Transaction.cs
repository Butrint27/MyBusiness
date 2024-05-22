using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;

namespace MyBusiness.TransactionMicroservice.Models
{
    public class Transaction
    {
    public int TransactionId { get; set; }
    public int ProductId { get; set; } // Changed to int
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public ICollection<Report> Reports { get; set; }
    
    }
}