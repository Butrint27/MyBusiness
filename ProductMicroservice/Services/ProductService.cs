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
    private readonly IMapper _mapper;

    public ProductService(MySQLDataContext mySqlContext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper)
    {
        _mysqlContext = mySqlContext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

     public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var mySqlProducts = await _mysqlContext.Products.Include(p => p.Supplier).ToListAsync();
        var mongoProducts = await _mongoCollection.Find(new BsonDocument()).ToListAsync();

        var mySqlProductDtos = _mapper.Map<IEnumerable<ProductDTO>>(mySqlProducts);
        var mongoProductDtos = mongoProducts.Select(p => _mapper.Map<ProductDTO>(BsonSerializer.Deserialize<Product>(p)));

        // Combine data from both databases if needed

        return mySqlProductDtos;
    }

    public async Task<ProductDTO> GetProductByIdAsync(int productId)
    {
    var mySqlProduct = await _mysqlContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
    var mongoProduct = await _mongoCollection.Find(Builders<BsonDocument>.Filter.Eq("productId", productId)).FirstOrDefaultAsync();

    if (mySqlProduct == null && mongoProduct == null)
    {
        return null;
    }

    var combinedProductDto = new ProductDTO();

    if (mySqlProduct != null)
    {
        combinedProductDto = _mapper.Map<ProductDTO>(mySqlProduct);
    }

    if (mongoProduct != null)
    {
        try
        {
            var mongoProductEntity = BsonSerializer.Deserialize<Product>(mongoProduct);
            var mongoProductDto = _mapper.Map<ProductDTO>(mongoProductEntity);

            combinedProductDto.Name = mongoProductDto.Name ?? combinedProductDto.Name;
            combinedProductDto.Description = mongoProductDto.Description ?? combinedProductDto.Description;
            combinedProductDto.Price = mongoProductDto.Price != 0 ? mongoProductDto.Price : combinedProductDto.Price;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing MongoDB product: {ex.Message}");
        }
    }
    return combinedProductDto;
    }

    public async Task<ProductDTO> CreateProductAsync(ProductDTO productDTO)
    {
        var product = _mapper.Map<Product>(productDTO);
        await _mysqlContext.Products.AddAsync(product);
        await _mysqlContext.SaveChangesAsync();

        var bsonDocument = product.ToBsonDocument();
        bsonDocument.Remove("_id"); // Remove the MongoDB ObjectId if it exists
        await _mongoCollection.InsertOneAsync(bsonDocument);

        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<ProductDTO> UpdateProductAsync(ProductDTO productDTO)
    {
        var existingProduct = await _mysqlContext.Products.FindAsync(productDTO.ProductId);
        if (existingProduct == null)
        {
            return null; // Product not found
        }

        _mapper.Map(productDTO, existingProduct);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("productId", productDTO.ProductId);
        var update = Builders<BsonDocument>.Update.Set("name", productDTO.Name)
                                                  .Set("description", productDTO.Description)
                                                  .Set("price", productDTO.Price)
                                                  .Set("supplierId", productDTO.SupplierId);
        // Update other fields as needed
        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<ProductDTO>(existingProduct);
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _mysqlContext.Products.FindAsync(productId);
        if (product == null)
        {
            return false; // Product not found
        }

        _mysqlContext.Products.Remove(product);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("productId", productId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
}
}