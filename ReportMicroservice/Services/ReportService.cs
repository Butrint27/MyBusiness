using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.ProductMicroservice.DTO;
using MyBusiness.RelationData;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ReportMicroservice.Services
{
    public class ReportService : IReportService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public ReportService(MySQLDataContext mysqlContext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection) 
    {
        _mysqlContext = mysqlContext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

     public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
    {
        var reportsMySQL = await _mysqlContext.Reports
            .Include(r => r.Transactions)
            .Include(r => r.Suppliers)
            .ToListAsync();

        var reportsDTO = reportsMySQL.Select(r => new ReportDTO
        {
            ReportId = r.ReportId,
            Title = r.Title,
            DateGenerated = r.DateGenerated,
            Content = r.Content,
            Author = r.Author,
            Transactions = r.Transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Type = t.Type,
                PaymentMethod = t.PaymentMethod,
                IsPaid = t.IsPaid
            }).ToList(),
            Suppliers = r.Suppliers.Select(s => new SupplierDTO
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

        return reportsDTO;
    }

    public async Task<ReportDTO> GetReportByIdAsync(int reportId)
    {
        var reportMySQL = await _mysqlContext.Reports
            .Include(r => r.Transactions)
            .Include(r => r.Suppliers)
            .FirstOrDefaultAsync(r => r.ReportId == reportId);

        if (reportMySQL == null)
            return null;

        var reportDTO = new ReportDTO
        {
            ReportId = reportMySQL.ReportId,
            Title = reportMySQL.Title,
            DateGenerated = reportMySQL.DateGenerated,
            Content = reportMySQL.Content,
            Author = reportMySQL.Author,
            Transactions = reportMySQL.Transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Type = t.Type,
                PaymentMethod = t.PaymentMethod,
                IsPaid = t.IsPaid
            }).ToList(),
            Suppliers = reportMySQL.Suppliers.Select(s => new SupplierDTO
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

        return reportDTO;
    }

    public async Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO)
    {
        var reportMySQL = new Report
        {
            Title = reportDTO.Title,
            DateGenerated = reportDTO.DateGenerated,
            Content = reportDTO.Content,
            Author = reportDTO.Author
        };

        await _mysqlContext.Reports.AddAsync(reportMySQL);
        await _mysqlContext.SaveChangesAsync();

        var document = reportDTO.ToBsonDocument();
        await _mongoCollection.InsertOneAsync(document);

        reportDTO.ReportId = reportMySQL.ReportId;
        return reportDTO;
    }

    public async Task<ReportDTO> UpdateReportAsync(ReportDTO reportDTO)
    {
        var reportMySQL = await _mysqlContext.Reports.FindAsync(reportDTO.ReportId);
        if (reportMySQL == null)
            return null;

        reportMySQL.Title = reportDTO.Title;
        reportMySQL.DateGenerated = reportDTO.DateGenerated;
        reportMySQL.Content = reportDTO.Content;
        reportMySQL.Author = reportDTO.Author;

        _mysqlContext.Reports.Update(reportMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("ReportId", reportDTO.ReportId);
        var update = Builders<BsonDocument>.Update
            .Set("Title", reportDTO.Title)
            .Set("DateGenerated", reportDTO.DateGenerated)
            .Set("Content", reportDTO.Content)
            .Set("Author", reportDTO.Author);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return reportDTO;
    }

    public async Task<bool> DeleteReportAsync(int reportId)
    {
        var reportMySQL = await _mysqlContext.Reports.FindAsync(reportId);
        if (reportMySQL == null)
            return false;

        _mysqlContext.Reports.Remove(reportMySQL);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("ReportId", reportId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
  }
}