using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.ReportMicroservice.DTO;

namespace MyBusiness.ReportMicroservice.Services
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDTO>> GetAllReportsAsync();
        Task<ReportDTO> GetReportByIdAsync(int reportId);
        Task<ReportDTO> CreateReportAsync(ReportDTO reportDTO);
        Task<ReportDTO> UpdateReportAsync(ReportDTO reportDTO);
        Task<bool> DeleteReportAsync(int reportId);
    }
}