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
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.TransactionMicroservice.Services
{
    public class TransactionService : ITransactionService
    {
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;


    public TransactionService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper) 
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync()
    {
        var transactionsMySQL = await _mysqlcontext.Transactions.Include(t => t.Product).Include(t => t.Supplier).ToListAsync();
        var transactionsDTO = _mapper.Map<IEnumerable<TransactionDTO>>(transactionsMySQL);

        var filter = Builders<BsonDocument>.Filter.Empty;
        var transactionsMongoDB = await _mongoCollection.Find(filter).ToListAsync();
        var transactionsMongoDBDTO = _mapper.Map<IEnumerable<TransactionDTO>>(transactionsMongoDB);

        return transactionsDTO.Concat(transactionsMongoDBDTO);
    }

    public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
    {
        var transactionMySQL = await _mysqlcontext.Transactions.Include(t => t.Product).Include(t => t.Supplier).FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        if (transactionMySQL != null)
            return _mapper.Map<TransactionDTO>(transactionMySQL);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", transactionId);
        var transactionMongoDB = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        if (transactionMongoDB != null)
            return _mapper.Map<TransactionDTO>(transactionMongoDB);

        return null; // Transaction not found in both MySQL and MongoDB
    }

    public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
    {
        var transactionEntityMySQL = _mapper.Map<Transaction>(transactionDTO);
        _mysqlcontext.Transactions.Add(transactionEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var transactionEntityMongoDB = _mapper.Map<BsonDocument>(transactionDTO);
        await _mongoCollection.InsertOneAsync(transactionEntityMongoDB);

        return _mapper.Map<TransactionDTO>(transactionEntityMySQL);
    }

    public async Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO)
    {
        var existingTransactionEntityMySQL = await _mysqlcontext.Transactions.FindAsync(transactionDTO.TransactionId);
        if (existingTransactionEntityMySQL == null)
            throw new ArgumentException("Transaction not found in MySQL");

        _mapper.Map(transactionDTO, existingTransactionEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", transactionDTO.TransactionId);
        var update = Builders<BsonDocument>.Update
            .Set("Date", transactionDTO.Date)
            .Set("Amount", transactionDTO.Amount)
            .Set("Type", transactionDTO.Type)
            .Set("PaymentMethod", transactionDTO.PaymentMethod)
            .Set("IsPaid", transactionDTO.IsPaid)
            .Set("ProductId", transactionDTO.ProductId)
            .Set("SupplierId", transactionDTO.SupplierId);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<TransactionDTO>(existingTransactionEntityMySQL);
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var existingTransactionEntityMySQL = await _mysqlcontext.Transactions.FindAsync(transactionId);
        if (existingTransactionEntityMySQL == null)
            return false; // Transaction not found in MySQL

        _mysqlcontext.Transactions.Remove(existingTransactionEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", transactionId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
    }
}