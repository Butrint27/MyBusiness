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
            .HasMany(p => p.Transactions)
            .WithMany(t => t.Products)
            .UsingEntity(j => j.ToTable("ProductTransaction"));

        // Product-Supplier Relationship (Many-to-Many)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Suppliers)
            .WithMany(s => s.Products)
            .UsingEntity(j => j.ToTable("ProductSupplier"));

        // Transaction-Report Relationship (Many-to-Many)
        modelBuilder.Entity<Transaction>()
            .HasMany(t => t.Reports)
            .WithMany(r => r.Transactions)
            .UsingEntity(j => j.ToTable("TransactionReport"));

        // Supplier-Report Relationship (Many-to-Many)
        modelBuilder.Entity<Supplier>()
            .HasMany(s => s.Reports)
            .WithMany(r => r.Suppliers)
            .UsingEntity(j => j.ToTable("SupplierReport"));
    }
    }
}