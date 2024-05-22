using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.SupplierMicroservice.Models
{
    [Table("suppliers")]
    public class Supplier
    {
    
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("phone")]
    public string Phone { get; set; }
    public ICollection<Product> Products { get; set; }

    }
}