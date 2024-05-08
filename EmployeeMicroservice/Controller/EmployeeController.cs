using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.EmployeeMicroservice.DTO;
using MyBusiness.EmployeeMicroservice.Services;

namespace MyBusiness.EmployeeMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
    private IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService){
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    // GET: api/employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDTO>> GetEmployee(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return employee;
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDTO>> CreateEmployee(EmployeeDTO employeeDTO)
    {
        var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeDTO);
        return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
    }

    // PUT: api/employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeDTO employeeDTO)
    {
        if (id != employeeDTO.EmployeeId)
        {
            return BadRequest();
        }

        var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeDTO);
        if (updatedEmployee == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeService.DeleteEmployeeAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
  }
}