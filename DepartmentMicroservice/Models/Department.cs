using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.EmployeeMicroservice.Models;

namespace MyBusiness.DepartmentMicroservice.Models
{
    [Table("department")]
    public class Department
    {
        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Column("department_name")]
        public string DepartmentName { get; set; }

        public ICollection<Employee> Employees { get; set; }

    }
}