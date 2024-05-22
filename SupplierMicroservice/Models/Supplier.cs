using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.SupplierMicroservice.Models
{
    public class Supplier
    {
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public ICollection<Product> Products { get; set; }

    }
}