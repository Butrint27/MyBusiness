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
      public DateTime Date { get; set; }
      public decimal Amount { get; set; }
      public string Type { get; set; } // Sale or Purchase
      public string PaymentMethod { get; set; }
      public bool IsPaid { get; set; }
      public int ProductId { get; set; }
      public virtual Product Product { get; set; }
      public virtual ICollection<Report> Reports { get; set; }
    }
}