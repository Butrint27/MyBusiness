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
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;

    public SupplierService(MySQLDataContext mysqlContext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper)
    {
        _mysqlContext = mysqlContext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SupplierDTO>> GetAllSuppliersAsync()
    {
        var mySqlSuppliers = await _mysqlContext.Suppliers.Include(s => s.Products).ToListAsync();
        var mongoSuppliers = await _mongoCollection.Find(new BsonDocument()).ToListAsync();

        var mySqlSupplierDtos = _mapper.Map<IEnumerable<SupplierDTO>>(mySqlSuppliers);
        var mongoSupplierDtos = mongoSuppliers.Select(s => _mapper.Map<SupplierDTO>(BsonSerializer.Deserialize<Supplier>(s)));

        // Combine data from both databases if needed

        return mySqlSupplierDtos;
    }

    public async Task<SupplierDTO> GetSupplierByIdAsync(int supplierId)
    {
    var mySqlSupplier = await _mysqlContext.Suppliers.Include(s => s.Products).FirstOrDefaultAsync(s => s.SupplierId == supplierId);
    
    var filter = Builders<BsonDocument>.Filter.Eq("supplierId", supplierId);
    var mongoSupplier = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

    if (mySqlSupplier == null && mongoSupplier == null)
    {
        return null;
    }

    var combinedSupplierDto = new SupplierDTO();

    if (mySqlSupplier != null)
    {
        combinedSupplierDto = _mapper.Map<SupplierDTO>(mySqlSupplier);
    }

    if (mongoSupplier != null)
    {
        var mongoSupplierEntity = BsonSerializer.Deserialize<Supplier>(mongoSupplier);
        var mongoSupplierDto = _mapper.Map<SupplierDTO>(mongoSupplierEntity);

        combinedSupplierDto.Name = mongoSupplierDto.Name ?? combinedSupplierDto.Name;
        combinedSupplierDto.Email = mongoSupplierDto.Email ?? combinedSupplierDto.Email;
        combinedSupplierDto.Phone = mongoSupplierDto.Phone ?? combinedSupplierDto.Phone;
    }

    return combinedSupplierDto;
    }

    public async Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDTO)
    {
        var supplier = _mapper.Map<Supplier>(supplierDTO);
        await _mysqlContext.Suppliers.AddAsync(supplier);
        await _mysqlContext.SaveChangesAsync();

        var bsonDocument = supplier.ToBsonDocument();
        bsonDocument.Remove("_id"); // Remove the MongoDB ObjectId if it exists
        await _mongoCollection.InsertOneAsync(bsonDocument);

        return _mapper.Map<SupplierDTO>(supplier);
    }

    public async Task<SupplierDTO> UpdateSupplierAsync(SupplierDTO supplierDTO)
    {
        var existingSupplier = await _mysqlContext.Suppliers.FindAsync(supplierDTO.SupplierId);
        if (existingSupplier == null)
        {
            return null; // Supplier not found
        }

        _mapper.Map(supplierDTO, existingSupplier);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("supplierId", supplierDTO.SupplierId);
        var update = Builders<BsonDocument>.Update.Set("name", supplierDTO.Name)
                                                  .Set("email", supplierDTO.Email)
                                                  .Set("phone", supplierDTO.Phone);
        // Update other fields as needed
        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<SupplierDTO>(existingSupplier);
    }

    public async Task<bool> DeleteSupplierAsync(int supplierId)
    {
        var supplier = await _mysqlContext.Suppliers.FindAsync(supplierId);
        if (supplier == null)
        {
            return false; // Supplier not found
        }

        _mysqlContext.Suppliers.Remove(supplier);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("supplierId", supplierId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }   
  }
}