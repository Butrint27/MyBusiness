using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.RelationData;

namespace MyBusiness.ProductMicroservice.Services
{
    public class ProductService : IProductService
{
    private readonly MySQLDataContext _mysqlContext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;

    public ProductService(MySQLDataContext mySqlContext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper)
    {
        _mysqlContext = mySqlContext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var productsMySQL = await _mysqlContext.Products.Include(p => p.Transactions).Include(p => p.Reports).ToListAsync();
        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(productsMySQL);

        var filter = Builders<BsonDocument>.Filter.Empty;
        var productsMongoDB = await _mongoCollection.Find(filter).ToListAsync();
        var productsMongoDBDTO = _mapper.Map<IEnumerable<ProductDTO>>(productsMongoDB);

        return productsDTO.Concat(productsMongoDBDTO);
    }

    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
        var productMySQL = await _mysqlContext.Products.Include(p => p.Transactions).Include(p => p.Reports).FirstOrDefaultAsync(p => p.ProductId == productId);
        if (productMySQL != null)
            return _mapper.Map<ProductDTO>(productMySQL);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", productId);
        var productMongoDB = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        if (productMongoDB != null)
            return _mapper.Map<ProductDTO>(productMongoDB);

        return null;
    }

    public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
    {
        var productEntityMySQL = _mapper.Map<Product>(productDTO);
        productEntityMySQL.LastUpdated = DateTime.UtcNow;
        _mysqlContext.Products.Add(productEntityMySQL);
        await _mysqlContext.SaveChangesAsync();

        var productEntityMongoDB = _mapper.Map<BsonDocument>(productDTO);
        await _mongoCollection.InsertOneAsync(productEntityMongoDB);

        return _mapper.Map<ProductDTO>(productEntityMySQL);
    }

    public async Task<ProductDTO> UpdateProductAsync(ProductDTO productDTO)
    {
        var existingProductEntityMySQL = await _mysqlContext.Products.FindAsync(productDTO.ProductId);
        if (existingProductEntityMySQL == null)
            throw new ArgumentException("Product not found in MySQL");

        _mapper.Map(productDTO, existingProductEntityMySQL);
        existingProductEntityMySQL.LastUpdated = DateTime.UtcNow;
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", productDTO.ProductId);
        var update = Builders<BsonDocument>.Update
            .Set("Name", productDTO.Name)
            .Set("Description", productDTO.Description)
            .Set("Price", productDTO.Price)
            .Set("StockQuantity", productDTO.StockQuantity)
            .Set("LastUpdated", DateTime.UtcNow)
            .Set("Category", productDTO.Category)
            .Set("IsActive", productDTO.IsActive);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<ProductDTO>(existingProductEntityMySQL);
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var existingProductEntityMySQL = await _mysqlContext.Products.FindAsync(productId);
        if (existingProductEntityMySQL == null)
            return false;

        _mysqlContext.Products.Remove(existingProductEntityMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", productId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
}
}