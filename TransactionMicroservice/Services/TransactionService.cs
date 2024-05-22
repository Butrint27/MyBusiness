using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MyBusiness.RelationData;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Models;

namespace MyBusiness.TransactionMicroservice.Services
{
    public class TransactionService : ITransactionService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;

    public TransactionService(MySQLDataContext mysqlContext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper) 
    {
        _mysqlContext = mysqlContext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }
      public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync()
    {
        var mySqlTransactions = await _mysqlContext.Transactions.Include(t => t.Reports).ToListAsync();
        var mongoTransactions = await _mongoCollection.Find(new BsonDocument()).ToListAsync();

        var mySqlTransactionDtos = _mapper.Map<IEnumerable<TransactionDTO>>(mySqlTransactions);
        var mongoTransactionDtos = mongoTransactions.Select(t => _mapper.Map<TransactionDTO>(BsonSerializer.Deserialize<Transaction>(t)));

        // Combine data from both databases if needed

        return mySqlTransactionDtos;
    }

   public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
{
    var mySqlTransaction = await _mysqlContext.Transactions.Include(t => t.Reports).FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    
    TransactionDTO mySqlTransactionDto = null;
    if (mySqlTransaction != null)
    {
        mySqlTransactionDto = _mapper.Map<TransactionDTO>(mySqlTransaction);
    }

    TransactionDTO mongoTransactionDto = null;
    var mongoTransaction = await _mongoCollection.Find(Builders<BsonDocument>.Filter.Eq("transactionId", transactionId)).FirstOrDefaultAsync();
    if (mongoTransaction != null)
    {
        mongoTransactionDto = _mapper.Map<TransactionDTO>(BsonSerializer.Deserialize<Transaction>(mongoTransaction));
    }

    // Combine data from both databases if needed

    return mySqlTransactionDto ?? mongoTransactionDto;
}

    public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
    {
        var transaction = _mapper.Map<Transaction>(transactionDTO);
        await _mysqlContext.Transactions.AddAsync(transaction);
        await _mysqlContext.SaveChangesAsync();

        var bsonDocument = transaction.ToBsonDocument();
        bsonDocument.Remove("_id"); // Remove the MongoDB ObjectId if it exists
        await _mongoCollection.InsertOneAsync(bsonDocument);

        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO)
    {
    var existingTransaction = await _mysqlContext.Transactions.FindAsync(transactionDTO.TransactionId);
    if (existingTransaction == null)
    {
        return null; // Transaction not found
    }

    _mapper.Map(transactionDTO, existingTransaction);
    await _mysqlContext.SaveChangesAsync();

    var filter = Builders<BsonDocument>.Filter.Eq("transactionId", transactionDTO.TransactionId);
    var update = Builders<BsonDocument>.Update
        .Set("productId", transactionDTO.ProductId)
        .Set("quantity", transactionDTO.Quantity)
        .Set("transactionDate", transactionDTO.TransactionDate);
    // Add more fields to update if needed

    await _mongoCollection.UpdateOneAsync(filter, update);

    return _mapper.Map<TransactionDTO>(existingTransaction);
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var transaction = await _mysqlContext.Transactions.FindAsync(transactionId);
        if (transaction == null)
        {
            return false; // Transaction not found
        }

        _mysqlContext.Transactions.Remove(transaction);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("transactionId", transactionId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
    }
}