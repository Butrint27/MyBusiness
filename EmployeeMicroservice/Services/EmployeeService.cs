
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.EmployeeMicroservice.DTO;
using MyBusiness.EmployeeMicroservice.Models;
using MyBusiness.RelationData;

namespace MyBusiness.EmployeeMicroservice.Services
{
    public class EmployeeService : IEmployeeService
    {
    private readonly MySQLDataContext _mysqlcontext;
    private readonly MongoDBDataContext _mongodbcontext;
    private readonly IMongoCollection<BsonDocument> _mongoCollection;

    public EmployeeService(MySQLDataContext mysqlcontext, MongoDBDataContext mongodbcontext, IMongoCollection<BsonDocument> mongoCollection) 
    {
        _mysqlcontext = mysqlcontext;
        _mongodbcontext = mongodbcontext;
        _mongoCollection = mongoCollection;
    }

    public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDTO)
    {
    // Check if the provided department ID exists
    var departmentExists = await _mysqlcontext.Departments.AnyAsync(d => d.DepartmentId == employeeDTO.DepartmentId);
    if (!departmentExists)
    {
        throw new InvalidOperationException("Invalid department ID");
    }

    var employee = new Employee
    {
        Name = employeeDTO.Name,
        Surname = employeeDTO.Surname,
        Email = employeeDTO.Email,
        PhoneNumber = employeeDTO.PhoneNumber,
        HireDate = employeeDTO.HireDate,
        DepartmentId = employeeDTO.DepartmentId
    };

    // Add employee to MySQL
    _mysqlcontext.Employees.Add(employee);
    await _mysqlcontext.SaveChangesAsync();

    // Also add to MongoDB
    await _mongoCollection.InsertOneAsync(employee.ToBsonDocument());

    // Return the created employee DTO
    return new EmployeeDTO
    {
        EmployeeId = employee.EmployeeId,
        Name = employee.Name,
        Surname = employee.Surname,
        Email = employee.Email,
        PhoneNumber = employee.PhoneNumber,
        HireDate = employee.HireDate,
        DepartmentId = employee.DepartmentId
    };
   }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
      var employee = await _mysqlcontext.Employees.FindAsync(employeeId);
      if (employee == null)
      {
        return false; // Employee not found
      }

      _mysqlcontext.Employees.Remove(employee);
      await _mysqlcontext.SaveChangesAsync();

      var filter = Builders<BsonDocument>.Filter.Eq("_id", employeeId);
      await _mongoCollection.DeleteOneAsync(filter);

     return true;
    }

    public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
    {
      var employees = await _mysqlcontext.Employees.ToListAsync();

    // Map employees to DTOs
      var employeeDTOs = employees.Select(employee => new EmployeeDTO
      {
        EmployeeId = employee.EmployeeId,
        Name = employee.Name,
        Surname = employee.Surname,
        Email = employee.Email,
        PhoneNumber = employee.PhoneNumber,
        HireDate = employee.HireDate,
        DepartmentId = employee.DepartmentId
      });

    return employeeDTOs;
    }

    public async Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId)
    {
      var employee = await _mysqlcontext.Employees.FindAsync(employeeId);
      if (employee == null)
      {
        return null; // Employee not found
      }

    // Map employee to DTO
      var employeeDTO = new EmployeeDTO
      {
        EmployeeId = employee.EmployeeId,
        Name = employee.Name,
        Surname = employee.Surname,
        Email = employee.Email,
        PhoneNumber = employee.PhoneNumber,
        HireDate = employee.HireDate,
        DepartmentId = employee.DepartmentId
      };

      return employeeDTO;
    }

    public async Task<EmployeeDTO> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDTO)
    {
       var existingEmployee = await _mysqlcontext.Employees.FindAsync(employeeId);
       if (existingEmployee == null)
       {
        return null; // Employee not found
       }

       existingEmployee.Name = employeeDTO.Name;
       existingEmployee.Surname = employeeDTO.Surname;
       existingEmployee.Email = employeeDTO.Email;
       existingEmployee.PhoneNumber = employeeDTO.PhoneNumber;
       existingEmployee.HireDate = employeeDTO.HireDate;
       existingEmployee.DepartmentId = employeeDTO.DepartmentId;

       await _mysqlcontext.SaveChangesAsync();

    // Also update in MongoDB
       var filter = Builders<BsonDocument>.Filter.Eq("_id", employeeId);
       var update = Builders<BsonDocument>.Update
        .Set("name", employeeDTO.Name)
        .Set("surname", employeeDTO.Surname)
        .Set("email", employeeDTO.Email)
        .Set("phoneNumber", employeeDTO.PhoneNumber)
        .Set("hireDate", employeeDTO.HireDate)
        .Set("departmentId", employeeDTO.DepartmentId);

       await _mongoCollection.UpdateOneAsync(filter, update);

       return employeeDTO;
    }
  }
}