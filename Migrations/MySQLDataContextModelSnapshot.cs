﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBusiness.RelationData;

#nullable disable

namespace MyBusiness.Migrations
{
    [DbContext(typeof(MySQLDataContext))]
    partial class MySQLDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MyBusiness.DepartmentMicroservice.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("department_id");

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("department_name");

                    b.HasKey("DepartmentId");

                    b.ToTable("department");
                });

            modelBuilder.Entity("MyBusiness.EmployeeMicroservice.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("emoloyee_id");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("department_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<DateTime>("HireDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("hire_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("phone_number");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("surname");

                    b.HasKey("EmployeeId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("employee");
                });

            modelBuilder.Entity("MyBusiness.ProductMicroservice.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("MyBusiness.ReportMicroservice.Models.Report", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ReportDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.HasKey("ReportId");

                    b.HasIndex("TransactionId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("MyBusiness.SupplierMicroservice.Models.Supplier", b =>
                {
                    b.Property<int>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("SupplierId");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("MyBusiness.TransactionMicroservice.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("TransactionId");

                    b.HasIndex("ProductId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("MyBusiness.UserMicroservice.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("user_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("phone_number");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("surname");

                    b.HasKey("UserId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("MyBusiness.EmployeeMicroservice.Models.Employee", b =>
                {
                    b.HasOne("MyBusiness.DepartmentMicroservice.Models.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("MyBusiness.ProductMicroservice.Models.Product", b =>
                {
                    b.HasOne("MyBusiness.SupplierMicroservice.Models.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("MyBusiness.ReportMicroservice.Models.Report", b =>
                {
                    b.HasOne("MyBusiness.TransactionMicroservice.Models.Transaction", "Transaction")
                        .WithMany("Reports")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("MyBusiness.TransactionMicroservice.Models.Transaction", b =>
                {
                    b.HasOne("MyBusiness.ProductMicroservice.Models.Product", "Product")
                        .WithMany("Transactions")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("MyBusiness.DepartmentMicroservice.Models.Department", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("MyBusiness.ProductMicroservice.Models.Product", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("MyBusiness.SupplierMicroservice.Models.Supplier", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("MyBusiness.TransactionMicroservice.Models.Transaction", b =>
                {
                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
