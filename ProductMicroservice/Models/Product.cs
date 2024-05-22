using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ProductMicroservice.Models
{
    public class Product
    {
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int SupplierId { get; set; } // Changed to int
    public Supplier Supplier { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    }
}