using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.RelationData;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ProductMicroservice.Services
{
    public class ProductService : IProductService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public ProductService(MySQLDataContext mySqlContext, IMongoCollection<BsonDocument> mongoCollection)
    {
        _mysqlContext = mySqlContext;
        _mongoCollection = mongoCollection;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var productsMySQL = await _mysqlContext.Products
            .Include(p => p.Transactions)
            .Include(p => p.Suppliers)
            .ToListAsync();

        var productsDTO = productsMySQL.Select(p => new ProductDTO
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            LastUpdated = p.LastUpdated,
            Category = p.Category,
            IsActive = p.IsActive,
            Transactions = p.Transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Type = t.Type,
                PaymentMethod = t.PaymentMethod,
                IsPaid = t.IsPaid
            }).ToList(),
            Suppliers = p.Suppliers.Select(s => new SupplierDTO
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                ContactInfo = s.ContactInfo,
                Address = s.Address,
                City = s.City,
                Country = s.Country,
                Website = s.Website,
                IsActive = s.IsActive
            }).ToList()
        });

        return productsDTO;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
        var productMySQL = await _mysqlContext.Products
            .Include(p => p.Transactions)
            .Include(p => p.Suppliers)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (productMySQL == null)
            return null;

        var productDTO = new ProductDTO
        {
            ProductId = productMySQL.ProductId,
            Name = productMySQL.Name,
            Description = productMySQL.Description,
            Price = productMySQL.Price,
            StockQuantity = productMySQL.StockQuantity,
            LastUpdated = productMySQL.LastUpdated,
            Category = productMySQL.Category,
            IsActive = productMySQL.IsActive,
            Transactions = productMySQL.Transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Type = t.Type,
                PaymentMethod = t.PaymentMethod,
                IsPaid = t.IsPaid
            }).ToList(),
            Suppliers = productMySQL.Suppliers.Select(s => new SupplierDTO
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                ContactInfo = s.ContactInfo,
                Address = s.Address,
                City = s.City,
                Country = s.Country,
                Website = s.Website,
                IsActive = s.IsActive
            }).ToList()
        };

        return productDTO;
    }

    public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
    {
        var productMySQL = new Product
        {
            Name = productDTO.Name,
            Description = productDTO.Description,
            Price = productDTO.Price,
            StockQuantity = productDTO.StockQuantity,
            LastUpdated = DateTime.UtcNow,
            Category = productDTO.Category,
            IsActive = productDTO.IsActive
        };

        await _mysqlContext.Products.AddAsync(productMySQL);
        await _mysqlContext.SaveChangesAsync();

        var document = productDTO.ToBsonDocument();
        await _mongoCollection.InsertOneAsync(document);

        productDTO.ProductId = productMySQL.ProductId;
        return productDTO;
    }

    public async Task<ProductDTO> UpdateProductAsync(ProductDTO productDTO)
    {
        var productMySQL = await _mysqlContext.Products.FindAsync(productDTO.ProductId);
        if (productMySQL == null)
            return null;

        productMySQL.Name = productDTO.Name;
        productMySQL.Description = productDTO.Description;
        productMySQL.Price = productDTO.Price;
        productMySQL.StockQuantity = productDTO.StockQuantity;
        productMySQL.LastUpdated = DateTime.UtcNow;
        productMySQL.Category = productDTO.Category;
        productMySQL.IsActive = productDTO.IsActive;

        _mysqlContext.Products.Update(productMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("ProductId", productDTO.ProductId);
        var update = Builders<BsonDocument>.Update
            .Set("Name", productDTO.Name)
            .Set("Description", productDTO.Description)
            .Set("Price", productDTO.Price)
            .Set("StockQuantity", productDTO.StockQuantity)
            .Set("LastUpdated", DateTime.UtcNow)
            .Set("Category", productDTO.Category)
            .Set("IsActive", productDTO.IsActive);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return productDTO;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var productMySQL = await _mysqlContext.Products.FindAsync(productId);
        if (productMySQL == null)
            return false;

        _mysqlContext.Products.Remove(productMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("ProductId", productId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
}
}