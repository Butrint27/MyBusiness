using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.Mapping
{
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Define your mappings here
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<Supplier, SupplierDTO>().ReverseMap();
        CreateMap<Transaction, TransactionDTO>().ReverseMap();
        CreateMap<Report, ReportDTO>().ReverseMap();
    }
}
}