using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.RelationData;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.TransactionMicroservice.Services
{
    public class TransactionService : ITransactionService
    {
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public TransactionService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection) 
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

     public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
    {
        var transactionEntity = new Transaction
        {
            Date = transactionDTO.Date,
            Amount = transactionDTO.Amount,
            Type = transactionDTO.Type,
            PaymentMethod = transactionDTO.PaymentMethod,
            IsPaid = transactionDTO.IsPaid,
            ProductId = transactionDTO.ProductId
        };

        _mysqlcontext.Transactions.Add(transactionEntity);
        await _mysqlcontext.SaveChangesAsync();

        // Also add to MongoDB
        await _mongoCollection.InsertOneAsync(transactionEntity.ToBsonDocument());

        // Return the created transaction DTO
        return transactionDTO;
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var transactionEntity = await _mysqlcontext.Transactions.FindAsync(transactionId);
        if (transactionEntity == null)
        {
            return false; // Transaction not found
        }

        _mysqlcontext.Transactions.Remove(transactionEntity);
        await _mysqlcontext.SaveChangesAsync();

        // Remove from MongoDB
        var filter = Builders<BsonDocument>.Filter.Eq("_id", transactionId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }

    public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync()
    {
        var transactions = await _mysqlcontext.Transactions.ToListAsync();
        return transactions.Select(transaction => new TransactionDTO
        {
            TransactionId = transaction.TransactionId,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Type = transaction.Type,
            PaymentMethod = transaction.PaymentMethod,
            IsPaid = transaction.IsPaid,
            ProductId = transaction.ProductId
        });
    }

    public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
    {
        var transactionEntity = await _mysqlcontext.Transactions.FindAsync(transactionId);
        if (transactionEntity == null)
        {
            return null; // Transaction not found
        }

        // Map transaction entity to DTO
        return new TransactionDTO
        {
            TransactionId = transactionEntity.TransactionId,
            Date = transactionEntity.Date,
            Amount = transactionEntity.Amount,
            Type = transactionEntity.Type,
            PaymentMethod = transactionEntity.PaymentMethod,
            IsPaid = transactionEntity.IsPaid,
            ProductId = transactionEntity.ProductId
        };
    }

    public async Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO)
    {
        if (transactionDTO == null)
        {
            throw new ArgumentNullException(nameof(transactionDTO), "Transaction DTO is null");
        }

        var existingTransactionEntity = await _mysqlcontext.Transactions.FindAsync(transactionDTO.TransactionId);
        if (existingTransactionEntity == null)
        {
            return null; // Transaction not found
        }

        // Update existing transaction entity
        existingTransactionEntity.Date = transactionDTO.Date;
        existingTransactionEntity.Amount = transactionDTO.Amount;
        existingTransactionEntity.Type = transactionDTO.Type;
        existingTransactionEntity.PaymentMethod = transactionDTO.PaymentMethod;
        existingTransactionEntity.IsPaid = transactionDTO.IsPaid;
        existingTransactionEntity.ProductId = transactionDTO.ProductId;

        await _mysqlcontext.SaveChangesAsync();

        // Update transaction in MongoDB
        var filter = Builders<BsonDocument>.Filter.Eq("_id", transactionDTO.TransactionId);
        var update = Builders<BsonDocument>.Update
            .Set("date", transactionDTO.Date)
            .Set("amount", transactionDTO.Amount)
            .Set("type", transactionDTO.Type)
            .Set("paymentMethod", transactionDTO.PaymentMethod)
            .Set("isPaid", transactionDTO.IsPaid)
            .Set("productId", transactionDTO.ProductId);
        // Update other attributes...

        await _mongoCollection.UpdateOneAsync(filter, update);

        // Return updated transaction DTO
        return transactionDTO;
    }
    }
}