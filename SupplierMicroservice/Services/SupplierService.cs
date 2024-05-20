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
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;


    public SupplierService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper)
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

     public async Task<IEnumerable<SupplierDTO>> GetAllSuppliersAsync()
    {
        var suppliersMySQL = await _mysqlcontext.Suppliers.ToListAsync();
        var suppliersDTO = _mapper.Map<IEnumerable<SupplierDTO>>(suppliersMySQL);

        var filter = Builders<BsonDocument>.Filter.Empty;
        var suppliersMongoDB = await _mongoCollection.Find(filter).ToListAsync();
        var suppliersMongoDBDTO = _mapper.Map<IEnumerable<SupplierDTO>>(suppliersMongoDB);

        return suppliersDTO.Concat(suppliersMongoDBDTO);
    }

    public async Task<SupplierDTO> GetSupplierByIdAsync(int supplierId)
    {
        var supplierMySQL = await _mysqlcontext.Suppliers.FindAsync(supplierId);
        if (supplierMySQL != null)
            return _mapper.Map<SupplierDTO>(supplierMySQL);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", supplierId);
        var supplierMongoDB = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        if (supplierMongoDB != null)
            return _mapper.Map<SupplierDTO>(supplierMongoDB);

        return null; // Supplier not found in both MySQL and MongoDB
    }

    public async Task<SupplierDTO> CreateSupplierAsync(SupplierDTO supplierDTO)
    {
        var supplierEntityMySQL = _mapper.Map<Supplier>(supplierDTO);
        _mysqlcontext.Suppliers.Add(supplierEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var supplierEntityMongoDB = _mapper.Map<BsonDocument>(supplierDTO);
        await _mongoCollection.InsertOneAsync(supplierEntityMongoDB);

        return _mapper.Map<SupplierDTO>(supplierEntityMySQL);
    }

    public async Task<SupplierDTO> UpdateSupplierAsync(SupplierDTO supplierDTO)
    {
        var existingSupplierEntityMySQL = await _mysqlcontext.Suppliers.FindAsync(supplierDTO.SupplierId);
        if (existingSupplierEntityMySQL == null)
            throw new ArgumentException("Supplier not found in MySQL");

        _mapper.Map(supplierDTO, existingSupplierEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", supplierDTO.SupplierId);
        var update = Builders<BsonDocument>.Update
            .Set("Name", supplierDTO.Name)
            .Set("ContactInfo", supplierDTO.ContactInfo)
            .Set("Address", supplierDTO.Address)
            .Set("City", supplierDTO.City)
            .Set("Country", supplierDTO.Country)
            .Set("Website", supplierDTO.Website)
            .Set("IsActive", supplierDTO.IsActive);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<SupplierDTO>(existingSupplierEntityMySQL);
    }

    public async Task<bool> DeleteSupplierAsync(int supplierId)
    {
        var existingSupplierEntityMySQL = await _mysqlcontext.Suppliers.FindAsync(supplierId);
        if (existingSupplierEntityMySQL == null)
            return false; // Supplier not found in MySQL

        _mysqlcontext.Suppliers.Remove(existingSupplierEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", supplierId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
  }
}