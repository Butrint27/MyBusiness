using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.ReportMicroservice.DTO;
using MyBusiness.ReportMicroservice.Services;

namespace MyBusiness.ReportMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpGet("{reportId}")]
    public async Task<IActionResult> GetReportById(int reportId)
    {
        var report = await _reportService.GetReportByIdAsync(reportId);
        if (report == null)
        {
            return NotFound();
        }
        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] ReportDTO reportDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdReport = await _reportService.CreateReportAsync(reportDTO);
        return CreatedAtAction(nameof(GetReportById), new { reportId = createdReport.ReportId }, createdReport);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReport([FromBody] ReportDTO reportDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedReport = await _reportService.UpdateReportAsync(reportDTO);
        if (updatedReport == null)
        {
            return NotFound();
        }

        return Ok(updatedReport);
    }

    [HttpDelete("{reportId}")]
    public async Task<IActionResult> DeleteReport(int reportId)
    {
        var deleted = await _reportService.DeleteReportAsync(reportId);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
    }
}