using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MyBusiness.DepartmentMicroservice.Models;
using MyBusiness.EmployeeMicroservice.Models;
using MyBusiness.UserMicroservice.Models;

namespace MyBusiness.RelationData
{
  public class MongoDBDataContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBDataContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Employee> Employees => _database.GetCollection<Employee>("Employees");
        public IMongoCollection<Department> Departments => _database.GetCollection<Department>("Departments");

    }
}