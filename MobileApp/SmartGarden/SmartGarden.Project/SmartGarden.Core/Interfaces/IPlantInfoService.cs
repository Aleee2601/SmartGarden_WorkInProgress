using SmartGarden.Core.DTOs;

namespace SmartGarden.Core.Interfaces;

/// <summary>
/// Service for searching plant information from external plant database APIs
/// </summary>
public interface IPlantInfoService
{
    /// <summary>
    /// Search for plants by query string (e.g., "basil", "tomato")
    /// </summary>
    /// <param name="query">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of plant search results with auto-suggested thresholds</returns>
    Task<IEnumerable<PlantSearchResponseDto>> SearchPlantsAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get detailed information about a specific plant by external API ID
    /// </summary>
    /// <param name="externalApiId">Plant ID from external API</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed plant information</returns>
    Task<PlantSearchResponseDto?> GetPlantDetailsAsync(int externalApiId, CancellationToken cancellationToken = default);
}
