using SmartGarden.Core.Models;

namespace SmartGarden.API.Services;

/// <summary>
/// Data export service interface
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Export sensor data to CSV
    /// </summary>
    Task<byte[]> ExportSensorDataToCsvAsync(int plantId, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// Export all plants data to CSV
    /// </summary>
    Task<byte[]> ExportAllPlantsToCsvAsync(int userId);

    /// <summary>
    /// Generate PDF report for a plant
    /// </summary>
    Task<byte[]> GeneratePlantReportPdfAsync(int plantId, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// Generate PDF summary for all plants
    /// </summary>
    Task<byte[]> GenerateUserSummaryPdfAsync(int userId);
}
