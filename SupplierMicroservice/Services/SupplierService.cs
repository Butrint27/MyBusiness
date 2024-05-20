using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.RelationData;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.SupplierMicroservice.Services
{
    public class SupplierService : ISupplierService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;


    public SupplierService(MySQLDataContext mysqlContext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection)
    {
        _mysqlContext = mysqlContext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

     public async Task<IEnumerable<SupplierDTO>> GetAllSuppliersAsync()
    {
        var suppliersMySQL = await _mysqlContext.Suppliers
            .Include(s => s.Products)
            .Include(s => s.Reports)
            .ToListAsync();

        var suppliersDTO = suppliersMySQL.Select(s => new SupplierDTO
        {
            SupplierId = s.SupplierId,
            Name = s.Name,
            ContactInfo = s.ContactInfo,
            Address = s.Address,
            City = s.City,
            Country = s.Country,
            Website = s.Website,
            IsActive = s.IsActive,
            Products = s.Products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                LastUpdated = p.LastUpdated,
                Category = p.Category,
                IsActive = p.IsActive
            }).ToList(),
            Reports = s.Reports.Select(r => new ReportDTO
            {
                ReportId = r.ReportId,
                Title = r.Title,
                DateGenerated = r.DateGenerated,
                Content = r.Content,
                Author = r.Author
            }).ToList()
        });

        return suppliersDTO;
    }

    public async Task<SupplierDTO> GetSupplierByIdAsync(int supplierId)
    {
        var supplierMySQL = await _mysqlContext.Suppliers
            .Include(s => s.Products)
            .Include(s => s.Reports)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

        if (supplierMySQL == null)
            return null;

        var supplierDTO = new SupplierDTO
        {
            SupplierId = supplierMySQL.SupplierId,
            Name = supplierMySQL.Name,
            ContactInfo = supplierMySQL.ContactInfo,
            Address = supplierMySQL.Address,
            City = supplierMySQL.City,
            Country = supplierMySQL.Country,
            Website = supplierMySQL.Website,
            IsActive = supplierMySQL.IsActive,
            Products = supplierMySQL.Products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                LastUpdated = p.LastUpdated,
                Category = p.Category,
                IsActive = p.IsActive
            }).ToList(),
            Reports = supplierMySQL.Reports.Select(r => new ReportDTO
            {
                ReportId = r.ReportId,
                Title = r.Title,
                DateGenerated = r.DateGenerated,
                Content = r.Content,
                Author = r.Author
            }).ToList()
        };

        return supplierDTO;
    }

    public async Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDTO)
    {
        var supplierMySQL = new Supplier
        {
            Name = supplierDTO.Name,
            ContactInfo = supplierDTO.ContactInfo,
            Address = supplierDTO.Address,
            City = supplierDTO.City,
            Country = supplierDTO.Country,
            Website = supplierDTO.Website,
            IsActive = supplierDTO.IsActive
        };

        await _mysqlContext.Suppliers.AddAsync(supplierMySQL);
        await _mysqlContext.SaveChangesAsync();

        var document = supplierDTO.ToBsonDocument();
        await _mongoCollection.InsertOneAsync(document);

        supplierDTO.SupplierId = supplierMySQL.SupplierId;
        return supplierDTO;
    }

    public async Task<SupplierDTO> UpdateSupplierAsync(SupplierDTO supplierDTO)
    {
        var supplierMySQL = await _mysqlContext.Suppliers.FindAsync(supplierDTO.SupplierId);
        if (supplierMySQL == null)
            return null;

        supplierMySQL.Name = supplierDTO.Name;
        supplierMySQL.ContactInfo = supplierDTO.ContactInfo;
        supplierMySQL.Address = supplierDTO.Address;
        supplierMySQL.City = supplierDTO.City;
        supplierMySQL.Country = supplierDTO.Country;
        supplierMySQL.Website = supplierDTO.Website;
        supplierMySQL.IsActive = supplierDTO.IsActive;

        _mysqlContext.Suppliers.Update(supplierMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("SupplierId", supplierDTO.SupplierId);
        var update = Builders<BsonDocument>.Update
            .Set("Name", supplierDTO.Name)
            .Set("ContactInfo", supplierDTO.ContactInfo)
            .Set("Address", supplierDTO.Address)
            .Set("City", supplierDTO.City)
            .Set("Country", supplierDTO.Country)
            .Set("Website", supplierDTO.Website)
            .Set("IsActive", supplierDTO.IsActive);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return supplierDTO;
    }

    // Delete a supplier by its ID from MySQL and MongoDB
    public async Task<bool> DeleteSupplierAsync(int supplierId)
    {
        var supplierMySQL = await _mysqlContext.Suppliers.FindAsync(supplierId);
        if (supplierMySQL == null)
            return false;

        _mysqlContext.Suppliers.Remove(supplierMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("SupplierId", supplierId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
  }
}