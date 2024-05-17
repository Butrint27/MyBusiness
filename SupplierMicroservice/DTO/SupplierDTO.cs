using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.SupplierMicroservice.DTO
{
    public class SupplierDTO
    {
     public int SupplierId { get; set; }
     public string Name { get; set; }
     public string ContactInfo { get; set; }
     public string Address { get; set; }
     public string City { get; set; }
     public string Country { get; set; }
     public string Website { get; set; }
     public bool IsActive { get; set; }
    }
}