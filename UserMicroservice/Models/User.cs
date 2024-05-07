using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBusiness.UserMicroservice.Models
{
   [Table("user")]
    public class User
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("surname")]
        public string Surname { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
    }
}