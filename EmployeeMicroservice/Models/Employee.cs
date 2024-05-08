using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.DepartmentMicroservice.Models;

namespace MyBusiness.EmployeeMicroservice.Models
{
    [Table("employee")]
    public class Employee
    {
        [Column("emoloyee_id")]
        public int EmployeeId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [Column("department_id")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}