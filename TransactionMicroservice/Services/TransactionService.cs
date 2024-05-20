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
    private readonly MySQLDataContext _mysqlContext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;


    public TransactionService(MySQLDataContext mysqlContext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection) 
    {
        _mysqlContext = mysqlContext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

     public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync()
    {
        var transactionsMySQL = await _mysqlContext.Transactions
            .Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Type = t.Type,
                PaymentMethod = t.PaymentMethod,
                IsPaid = t.IsPaid
            })
            .ToListAsync();

        return transactionsMySQL;
    }

    public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
    {
        var transactionMySQL = await _mysqlContext.Transactions.FindAsync(transactionId);
        if (transactionMySQL != null)
        {
            var transactionDTO = new TransactionDTO
            {
                TransactionId = transactionMySQL.TransactionId,
                Date = transactionMySQL.Date,
                Amount = transactionMySQL.Amount,
                Type = transactionMySQL.Type,
                PaymentMethod = transactionMySQL.PaymentMethod,
                IsPaid = transactionMySQL.IsPaid
            };

            return transactionDTO;
        }

        return null;
    }

    public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
    {
        var transactionMySQL = new Transaction
        {
            Date = transactionDTO.Date,
            Amount = transactionDTO.Amount,
            Type = transactionDTO.Type,
            PaymentMethod = transactionDTO.PaymentMethod,
            IsPaid = transactionDTO.IsPaid
        };

        await _mysqlContext.Transactions.AddAsync(transactionMySQL);
        await _mysqlContext.SaveChangesAsync();

        var document = transactionDTO.ToBsonDocument();
        await _mongoCollection.InsertOneAsync(document);

        transactionDTO.TransactionId = transactionMySQL.TransactionId;
        return transactionDTO;
    }

    public async Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO)
    {
        var transactionMySQL = await _mysqlContext.Transactions.FindAsync(transactionDTO.TransactionId);
        if (transactionMySQL == null)
            return null;

        transactionMySQL.Date = transactionDTO.Date;
        transactionMySQL.Amount = transactionDTO.Amount;
        transactionMySQL.Type = transactionDTO.Type;
        transactionMySQL.PaymentMethod = transactionDTO.PaymentMethod;
        transactionMySQL.IsPaid = transactionDTO.IsPaid;

        _mysqlContext.Transactions.Update(transactionMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("TransactionId", transactionDTO.TransactionId);
        var update = Builders<BsonDocument>.Update
            .Set("Date", transactionDTO.Date)
            .Set("Amount", transactionDTO.Amount)
            .Set("Type", transactionDTO.Type)
            .Set("PaymentMethod", transactionDTO.PaymentMethod)
            .Set("IsPaid", transactionDTO.IsPaid);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return transactionDTO;
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var transactionMySQL = await _mysqlContext.Transactions.FindAsync(transactionId);
        if (transactionMySQL == null)
            return false;

        _mysqlContext.Transactions.Remove(transactionMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("TransactionId", transactionId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
    }
}