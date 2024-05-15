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
    [HttpPut]
    public async Task<IActionResult> UpdateEmployeeAsync([FromBody] EmployeeDTO employeeDTO)
    {
      if (employeeDTO == null)
      {
        return BadRequest("Employee DTO is null");
      }

    // Call the service method to update the employee
      var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeDTO);

      if (updatedEmployee == null)
      {
        return NotFound("Employee not found");
      }

    // Return the updated employee DTO
      return Ok(updatedEmployee);
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