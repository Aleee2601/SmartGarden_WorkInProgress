using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.API.Services;
using System.Security.Claims;

namespace SmartGarden.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "UserOnly")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(IExportService exportService, ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Export sensor data for a plant to CSV
    /// </summary>
    [HttpGet("plant/{plantId}/sensors/csv")]
    public async Task<IActionResult> ExportSensorDataCsv(
        int plantId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        try
        {
            var csvData = await _exportService.ExportSensorDataToCsvAsync(plantId, startDate, endDate);
            var fileName = $"sensor-data-plant-{plantId}-{DateTime.Now:yyyyMMdd}.csv";

            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting sensor data for plant {plantId}");
            return StatusCode(500, new { error = "Failed to export sensor data" });
        }
    }

    /// <summary>
    /// Export all plants to CSV
    /// </summary>
    [HttpGet("plants/csv")]
    public async Task<IActionResult> ExportAllPlantsCsv()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var csvData = await _exportService.ExportAllPlantsToCsvAsync(userId);
            var fileName = $"all-plants-{DateTime.Now:yyyyMMdd}.csv";

            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting all plants");
            return StatusCode(500, new { error = "Failed to export plants data" });
        }
    }

    /// <summary>
    /// Generate PDF report for a plant
    /// </summary>
    [HttpGet("plant/{plantId}/report")]
    public async Task<IActionResult> GeneratePlantReport(
        int plantId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        try
        {
            var htmlData = await _exportService.GeneratePlantReportPdfAsync(plantId, startDate, endDate);
            var fileName = $"plant-report-{plantId}-{DateTime.Now:yyyyMMdd}.html";

            return File(htmlData, "text/html", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating report for plant {plantId}");
            return StatusCode(500, new { error = "Failed to generate plant report" });
        }
    }

    /// <summary>
    /// Generate summary report for all user plants
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GenerateUserSummary()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var htmlData = await _exportService.GenerateUserSummaryPdfAsync(userId);
            var fileName = $"garden-summary-{DateTime.Now:yyyyMMdd}.html";

            return File(htmlData, "text/html", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user summary");
            return StatusCode(500, new { error = "Failed to generate summary report" });
        }
    }

    /// <summary>
    /// Export sensor data for a specific date range with format options
    /// </summary>
    [HttpGet("plant/{plantId}/sensors/download")]
    public async Task<IActionResult> DownloadSensorData(
        int plantId,
        [FromQuery] string format = "csv",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            if (format.ToLower() == "csv")
            {
                var csvData = await _exportService.ExportSensorDataToCsvAsync(plantId, startDate, endDate);
                return File(csvData, "text/csv", $"sensor-data-{plantId}.csv");
            }
            else if (format.ToLower() == "html" || format.ToLower() == "pdf")
            {
                var htmlData = await _exportService.GeneratePlantReportPdfAsync(plantId, startDate, endDate);
                return File(htmlData, "text/html", $"plant-report-{plantId}.html");
            }
            else
            {
                return BadRequest(new { error = "Invalid format. Supported formats: csv, html, pdf" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading sensor data for plant {plantId}");
            return StatusCode(500, new { error = "Failed to download sensor data" });
        }
    }
}
