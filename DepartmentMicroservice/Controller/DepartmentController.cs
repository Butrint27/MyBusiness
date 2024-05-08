using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.DepartmentMicroservice.DTO;
using MyBusiness.DepartmentMicroservice.Services;

namespace MyBusiness.DepartmentMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // GET: api/Department
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    // GET: api/Department/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDTO>> GetDepartment(int id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        return department;
    }

    // POST: api/Department
    [HttpPost]
    public async Task<ActionResult<DepartmentDTO>> CreateDepartment(DepartmentDTO departmentDTO)
    {
        var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentDTO);
        return CreatedAtAction(nameof(GetDepartment), new { id = createdDepartment.DepartmentId }, createdDepartment);
    }

    // PUT: api/Department/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentDTO departmentDTO)
    {
        if (id != departmentDTO.DepartmentId)
        {
            return BadRequest();
        }

        var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, departmentDTO);
        if (updatedDepartment == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Department/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var result = await _departmentService.DeleteDepartmentAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    }
}