using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.DepartmentMicroservice.DTO;

namespace MyBusiness.DepartmentMicroservice.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<DepartmentDTO> GetDepartmentByIdAsync(int departmentId);
        Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDTO);
        Task<DepartmentDTO> UpdateDepartmentAsync(DepartmentDTO departmentDTO);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}