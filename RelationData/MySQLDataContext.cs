using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBusiness.DepartmentMicroservice.Models;
using MyBusiness.EmployeeMicroservice.Models;
using MyBusiness.ProductMicroservice.Models;
using MyBusiness.ReportMicroservice.Models;
using MyBusiness.SupplierMicroservice.Models;
using MyBusiness.TransactionMicroservice.Models;
using MyBusiness.UserMicroservice.Models;

namespace MyBusiness.RelationData
{
    public class MySQLDataContext : DbContext
    {
    public MySQLDataContext(DbContextOptions<MySQLDataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define relationships between entities

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product-Transaction Relationship (One-to-Many)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Product)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transaction-Report Relationship (One-to-Many)
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Transaction)
            .WithMany(t => t.Reports)
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
            
    }
  }
}