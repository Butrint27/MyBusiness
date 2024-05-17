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
       public int StockQuantity { get; set; }
       public DateTime LastUpdated { get; set; }
       public string Category { get; set; }
       public bool IsActive { get; set; }
       public virtual ICollection<Transaction> Transactions { get; set; }
       public virtual ICollection<Report> Reports { get; set; }
       public virtual ICollection<Supplier> Suppliers { get; set; }

    }
}