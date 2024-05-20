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
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.DTO;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.ReportMicroservice.Services
{
    public class ReportService : IReportService
    {
     private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;
    private readonly IMapper _mapper;


    public ReportService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection, IMapper mapper) 
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
    {
        var reportsMySQL = await _mysqlcontext.Reports.Include(r => r.Supplier).ToListAsync();
        var reportsDTO = _mapper.Map<IEnumerable<ReportDTO>>(reportsMySQL);

        var filter = Builders<BsonDocument>.Filter.Empty;
        var reportsMongoDB = await _mongoCollection.Find(filter).ToListAsync();
        var reportsMongoDBDTO = _mapper.Map<IEnumerable<ReportDTO>>(reportsMongoDB);

        return reportsDTO.Concat(reportsMongoDBDTO);
    }

    public async Task<ReportDTO> GetReportByIdAsync(int reportId)
    {
        var reportMySQL = await _mysqlcontext.Reports.Include(r => r.Supplier).FirstOrDefaultAsync(r => r.ReportId == reportId);
        if (reportMySQL != null)
            return _mapper.Map<ReportDTO>(reportMySQL);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", reportId);
        var reportMongoDB = await _mongoCollection.Find(filter).FirstOrDefaultAsync();
        if (reportMongoDB != null)
            return _mapper.Map<ReportDTO>(reportMongoDB);

        return null; // Report not found in both MySQL and MongoDB
    }

    public async Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO)
    {
        var reportEntityMySQL = _mapper.Map<Report>(reportDTO);
        _mysqlcontext.Reports.Add(reportEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var reportEntityMongoDB = _mapper.Map<BsonDocument>(reportDTO);
        await _mongoCollection.InsertOneAsync(reportEntityMongoDB);

        return _mapper.Map<ReportDTO>(reportEntityMySQL);
    }

    public async Task<ReportDTO> UpdateReportAsync(ReportDTO reportDTO)
    {
        var existingReportEntityMySQL = await _mysqlcontext.Reports.FindAsync(reportDTO.ReportId);
        if (existingReportEntityMySQL == null)
            throw new ArgumentException("Report not found in MySQL");

        _mapper.Map(reportDTO, existingReportEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", reportDTO.ReportId);
        var update = Builders<BsonDocument>.Update
            .Set("Title", reportDTO.Title)
            .Set("DateGenerated", reportDTO.DateGenerated)
            .Set("Content", reportDTO.Content)
            .Set("Author", reportDTO.Author);

        await _mongoCollection.UpdateOneAsync(filter, update);

        return _mapper.Map<ReportDTO>(existingReportEntityMySQL);
    }

    public async Task<bool> DeleteReportAsync(int reportId)
    {
        var existingReportEntityMySQL = await _mysqlcontext.Reports.FindAsync(reportId);
        if (existingReportEntityMySQL == null)
            return false; // Report not found in MySQL

        _mysqlcontext.Reports.Remove(existingReportEntityMySQL);
        await _mysqlcontext.SaveChangesAsync();

        var filter = Builders<BsonDocument>.Filter.Eq("_id", reportId);
        await _mongoCollection.DeleteOneAsync(filter);

        return true;
    }
    }
}