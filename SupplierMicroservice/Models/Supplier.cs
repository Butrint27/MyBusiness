using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.SupplierMicroservice.Models
{
    public class Supplier
    {
       public int SupplierId { get; set; }
       public string Name { get; set; }
       public string ContactInfo { get; set; }
       public string Address { get; set; }
       public string City { get; set; }
       public string Country { get; set; }
       public string Website { get; set; }
       public bool IsActive { get; set; }
       public virtual ICollection<Product> Products { get; set; }
       public virtual ICollection<Transaction> Transactions { get; set; }
    }
}