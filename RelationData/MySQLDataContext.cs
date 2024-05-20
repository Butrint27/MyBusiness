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

    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.Product)
        .WithMany(p => p.Transactions)
        .HasForeignKey(t => t.ProductId)
        .IsRequired();

    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.Supplier)
        .WithMany(s => s.Transactions)
        .HasForeignKey(t => t.SupplierId)
        .IsRequired();

    modelBuilder.Entity<Report>()
        .HasOne(r => r.Supplier)
        .WithMany(s => s.Reports)
        .HasForeignKey(r => r.SupplierId)
        .IsRequired();
        
    modelBuilder.Entity<Report>()
        .HasMany(r => r.Transactions)
        .WithMany(t => t.Reports)
        .UsingEntity(j => j.ToTable("TransactionReports"));

    modelBuilder.Entity<Report>()
        .HasMany(r => r.Products)
        .WithMany(p => p.Reports)
        .UsingEntity(j => j.ToTable("ProductReports"));
       }
    }
}