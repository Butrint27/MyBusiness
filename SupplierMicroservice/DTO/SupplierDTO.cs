using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.SupplierMicroservice.DTO
{
    public class SupplierDTO
    {
    public int SupplierId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    }
}