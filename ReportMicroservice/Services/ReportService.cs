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
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ReportMicroservice.Services
{
    public class ReportService : IReportService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;

    public ReportService(MySQLDataContext mysqlContext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper) 
    {
        _mysqlContext = mysqlContext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
    {
        var mySqlReports = await _mysqlContext.Reports.Include(r => r.Transaction).ToListAsync();
        var mongoReports = await _mongoCollection.Find(new BsonDocument()).ToListAsync();

        var mySqlReportDtos = _mapper.Map<IEnumerable<ReportDTO>>(mySqlReports);
        var mongoReportDtos = mongoReports.Select(r => _mapper.Map<ReportDTO>(BsonSerializer.Deserialize<Report>(r)));

        // Combine data from both databases if needed

        return mySqlReportDtos;
    }

    public async Task<ReportDTO> GetReportByIdAsync(int reportId)
{
    // Get the report from MySQL
    var mySqlReport = await _mysqlContext.Reports.FirstOrDefaultAsync(r => r.ReportId == reportId);
    if (mySqlReport != null)
    {
        // Map the report from MySQL to DTO
        return _mapper.Map<ReportDTO>(mySqlReport);
    }

    // Get the report from MongoDB
    var mongoReport = await _mongoCollection.Find(Builders<BsonDocument>.Filter.Eq("reportId", reportId)).FirstOrDefaultAsync();
    if (mongoReport != null)
    {
        // Map the report from MongoDB to DTO
        return _mapper.Map<ReportDTO>(BsonSerializer.Deserialize<Report>(mongoReport));
    }

    // Report not found in either database
    return null;
}

    public async Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO)
    {
        var report = _mapper.Map<Report>(reportDTO);
        await _mysqlContext.Reports.AddAsync(report);
        await _mysqlContext.SaveChangesAsync();

        var bsonDocument = report.ToBsonDocument();
        bsonDocument.Remove("_id"); // Remove the MongoDB ObjectId if it exists
        await _mongoCollection.InsertOneAsync(bsonDocument);

        return _mapper.Map<ReportDTO>(report);
    }

    public async Task<ReportDTO> UpdateReportAsync(ReportDTO reportDTO)
    {
        var existingReport = await _mysqlContext.Reports.FindAsync(reportDTO.ReportId);
        if (existingReport == null)
        {
            return null; // Report not found
        }

        _mapper.Map(reportDTO, existingReport);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("reportId", reportDTO.ReportId);
        var update = Builders<BsonDocument>.Update.Set("transactionId", reportDTO.TransactionId)
                                                  .Set("reportDate", reportDTO.ReportDate)
                                                  .Set("details", reportDTO.Details);
        // Update other fields as needed
        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<ReportDTO>(existingReport);
    }

    public async Task<bool> DeleteReportAsync(int reportId)
    {
        var report = await _mysqlContext.Reports.FindAsync(reportId);
        if (report == null)
        {
            return false; // Report not found
        }

        _mysqlContext.Reports.Remove(report);
        await _mysqlContext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("reportId", reportId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }

     
  }
}