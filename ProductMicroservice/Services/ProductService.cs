using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.RelationData;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.ProductMicroservice.Services
{
    public class ProductService : IProductService
    {
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public ProductService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection) 
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

     public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
    {
        var productEntity = new Product
        {
            Name = productDTO.Name,
            Description = productDTO.Description,
            Price = productDTO.Price,
            StockQuantity = productDTO.StockQuantity,
            LastUpdated = productDTO.LastUpdated,
            Category = productDTO.Category,
            IsActive = productDTO.IsActive
        };

        // Save product to MySQL
        _mysqlcontext.Products.Add(productEntity);
        await _mysqlcontext.SaveChangesAsync();

        // Also add to MongoDB
        await _mongoCollection.InsertOneAsync(productEntity.ToBsonDocument());

        // Return the created product DTO
        return new ProductDTO
        {
            ProductId = productEntity.ProductId,
            Name = productEntity.Name,
            Description = productEntity.Description,
            Price = productEntity.Price,
            StockQuantity = productDTO.StockQuantity,
            LastUpdated = productDTO.LastUpdated,
            Category = productDTO.Category,
            IsActive = productDTO.IsActive
        };
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var productEntity = await _mysqlcontext.Products.FindAsync(productId);
        if (productEntity == null)
        {
            return false; // Product not found
        }

        _mysqlcontext.Products.Remove(productEntity);
        await _mysqlcontext.SaveChangesAsync();

        // Remove from MongoDB
        var filter = Builders<BsonDocument>.Filter.Eq("_id", productId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var products = await _mysqlcontext.Products.ToListAsync();
        return products.Select(product => new ProductDTO
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            LastUpdated = product.LastUpdated,
            Category = product.Category,
            IsActive = product.IsActive
        });
    }

    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
        var productEntity = await _mysqlcontext.Products.FindAsync(productId);
        if (productEntity == null)
        {
            return null; // Product not found
        }

        // Map product entity to DTO
        return new ProductDTO
        {
            ProductId = productEntity.ProductId,
            Name = productEntity.Name,
            Description = productEntity.Description,
            Price = productEntity.Price,
            StockQuantity = productEntity.StockQuantity,
            LastUpdated = productEntity.LastUpdated,
            Category = productEntity.Category,
            IsActive = productEntity.IsActive
        };
    }

    public async Task<ProductDTO> UpdateProductAsync(ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            throw new ArgumentNullException(nameof(productDTO), "Product DTO is null");
        }

        var existingProductEntity = await _mysqlcontext.Products.FindAsync(productDTO.ProductId);
        if (existingProductEntity == null)
        {
            return null; // Product not found
        }

        // Update existing product entity
        existingProductEntity.Name = productDTO.Name;
        existingProductEntity.Description = productDTO.Description;
        existingProductEntity.Price = productDTO.Price;
        existingProductEntity.StockQuantity = productDTO.StockQuantity;
        existingProductEntity.LastUpdated = productDTO.LastUpdated;
        existingProductEntity.Category = productDTO.Category;
        existingProductEntity.IsActive = productDTO.IsActive;

        await _mysqlcontext.SaveChangesAsync();

        // Update product in MongoDB
        var filter = Builders<BsonDocument>.Filter.Eq("_id", productDTO.ProductId);
        var update = Builders<BsonDocument>.Update
            .Set("name", productDTO.Name)
            .Set("description", productDTO.Description)
            .Set("price", productDTO.Price)
            .Set("stockQuantity", productDTO.StockQuantity)
            .Set("lastUpdated", productDTO.LastUpdated)
            .Set("category", productDTO.Category)
            .Set("isActive", productDTO.IsActive);
        // Update other attributes...

        await _mongoCollection.UpdateOneAsync(filter, update);

        // Return updated product DTO
        return productDTO;
    }

    private TransactionDTO MapToTransactionDTO(Transaction transaction)
    {
        return new TransactionDTO
        {
            TransactionId = transaction.TransactionId,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Type = transaction.Type,
            PaymentMethod = transaction.PaymentMethod,
            IsPaid = transaction.IsPaid
        };
    }

    private ReportDTO MapToReportDTO(Report report)
    {
        return new ReportDTO
        {
            ReportId = report.ReportId,
            Title = report.Title,
            DateGenerated = report.DateGenerated,
            Content = report.Content,
            Author = report.Author
        };
    }

    private SupplierDTO MapToSupplierDTO(Supplier supplier)
    {
        return new SupplierDTO
        {
            SupplierId = supplier.SupplierId,
            Name = supplier.Name,
            ContactInfo = supplier.ContactInfo,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            Website = supplier.Website,
            IsActive = supplier.IsActive
        };
    }

    }
}