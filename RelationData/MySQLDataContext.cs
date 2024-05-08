using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBusiness.DepartmentMicroservice.Models;
using MyBusiness.EmployeeMicroservice.Models;
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<Employee>()
             .HasOne(e => e.Department) // Each employee belongs to one department
             .WithMany(d => d.Employees) // Each department can have many employees
             .HasForeignKey(e => e.DepartmentId) // Foreign key property
             .IsRequired(); // Department is required for each employee
        }

    }
}