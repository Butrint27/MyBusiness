using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;

namespace MyBusiness.TransactionMicroservice.Models
{
    [Table("transactions")]
    public class Transaction
    {
    
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; } // Changed to int
    public Product Product { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }
    public ICollection<Report> Reports { get; set; }
    
    }
}