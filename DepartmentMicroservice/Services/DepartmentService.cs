using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.DepartmentMicroservice.DTO;
using MyBusiness.DepartmentMicroservice.Models;
using MyBusiness.RelationData;

namespace MyBusiness.DepartmentMicroservice.Services
{
    public class DepartmentService : IDepartmentService
    {
    private readonly MySQLDataContext _mysqlContext;
    private readonly MongoDBDataContext _mongoDBContext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public DepartmentService(MySQLDataContext mySQLContext, MongoDBDataContext mongoDBContext, IMongoCollection<BsonDocument> mongoCollection)
    {
        _mysqlContext = mySQLContext;
        _mongoDBContext = mongoDBContext;
        _mongoCollection = mongoCollection;
    }

    public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
    {
        var departments = await _mysqlContext.Departments.ToListAsync();

        // Map departments to DTOs
        var departmentDTOs = departments.Select(department => new DepartmentDTO
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName
        });

        return departmentDTOs;
    }

    public async Task<DepartmentDTO> GetDepartmentByIdAsync(int departmentId)
    {
        var department = await _mysqlContext.Departments.FindAsync(departmentId);
        if (department == null)
        {
            return null; // Department not found
        }

        // Map department to DTO
        var departmentDTO = new DepartmentDTO
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName
        };

        return departmentDTO;
    }

    public async Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDTO)
    {
        var department = new Department
        {
            DepartmentName = departmentDTO.DepartmentName
        };

        // Add department to MySQL
        _mysqlContext.Departments.Add(department);
        await _mysqlContext.SaveChangesAsync();

        // Return the created department DTO
        return new DepartmentDTO
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName
        };
    }

    public async Task<bool> DeleteDepartmentAsync(int departmentId)
    {
        var department = await _mysqlContext.Departments.FindAsync(departmentId);
        if (department == null)
        {
            return false; // Department not found
        }

        _mysqlContext.Departments.Remove(department);
        await _mysqlContext.SaveChangesAsync();

        return true;
    }

    public async Task<DepartmentDTO> UpdateDepartmentAsync(int departmentId, DepartmentDTO departmentDTO)
    {
        var existingDepartment = await _mysqlContext.Departments.FindAsync(departmentId);
        if (existingDepartment == null)
        {
            return null; // Department not found
        }

        // Update existing department entity
        existingDepartment.DepartmentName = departmentDTO.DepartmentName;

        await _mysqlContext.SaveChangesAsync();

        // Return updated department DTO
        return departmentDTO;
    }
    }
}