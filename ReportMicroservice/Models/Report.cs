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
    public int TransactionId { get; set; } // Changed to int
    public Transaction Transaction { get; set; }
    public DateTime ReportDate { get; set; }
    public string Details { get; set; }
    }
}