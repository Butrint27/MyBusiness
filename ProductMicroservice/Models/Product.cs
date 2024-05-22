using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ProductMicroservice.Models
{
    [Table("products")]
    public class Product
    {

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; } // Changed to int
    public Supplier Supplier { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    }
}