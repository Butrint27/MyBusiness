using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBusiness.UserMicroservice.Models;

namespace MyBusiness.RelationData
{
    public class MySQLDataContext : DbContext
    {
        public MySQLDataContext(DbContextOptions<MySQLDataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        
    }
}