using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ReportMicroservice.Models
{
    [Table("reports")]
    public class Report
    {
    
    [Column("report_id")]
    public int ReportId { get; set; }

    [Column("transaction_id")]
    public int TransactionId { get; set; } // Changed to int
    public Transaction Transaction { get; set; }

    [Column("report_date")]
    public DateTime ReportDate { get; set; }

    [Column("details")]
    public string Details { get; set; }
    }
}