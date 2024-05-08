using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.EmployeeMicroservice.DTO;

namespace MyBusiness.EmployeeMicroservice.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDTO);
        Task<EmployeeDTO> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDTO);
        Task<bool> DeleteEmployeeAsync(int employeeId);
    }
}