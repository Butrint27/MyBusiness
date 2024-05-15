using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ReportMicroservice.Models
{
    public class Report
    {
      public int ReportId { get; set; }
      public string Title { get; set; }
      public DateTime DateGenerated { get; set; }
      public string Content { get; set; }
      public string Author { get; set; }
      public virtual ICollection<Product> Products { get; set; }
      public virtual ICollection<Supplier> Suppliers { get; set; }
      public virtual ICollection<Transaction> Transactions { get; set; }
    }
}