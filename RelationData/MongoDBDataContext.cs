using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MyBusiness.DepartmentMicroservice.Models;
using MyBusiness.EmployeeMicroservice.Models;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;
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
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Supplier> Suppliers => _database.GetCollection<Supplier>("Suppliers");
        public IMongoCollection<Transaction> Transactions => _database.GetCollection<Transaction>("Transactions");
        public IMongoCollection<Report> Reports => _database.GetCollection<Report>("Reports");

    }
}