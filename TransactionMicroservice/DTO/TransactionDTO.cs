using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ReportMicroservice.DTO;

namespace MyBusiness.TransactionMicroservice.DTO
{
    public class TransactionDTO
    {
      public int TransactionId { get; set; }
      public DateTime Date { get; set; }
      public decimal Amount { get; set; }
      public string Type { get; set; }
      public string PaymentMethod { get; set; }
      public bool IsPaid { get; set; }
      public int ProductId { get; set; }
      public ProductDTO Product { get; set; }
      public ICollection<ReportDTO> Reports { get; set; }
    }
}