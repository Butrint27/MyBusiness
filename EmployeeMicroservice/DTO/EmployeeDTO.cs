using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.DepartmentMicroservice.DTO;
using MyBusiness.DepartmentMicroservice.Models;

namespace MyBusiness.EmployeeMicroservice.DTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }

    }
}