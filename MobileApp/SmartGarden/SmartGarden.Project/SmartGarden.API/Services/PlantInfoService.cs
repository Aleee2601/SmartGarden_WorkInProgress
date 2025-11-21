using System.Text.Json;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Services;

/// <summary>
/// Service for integrating with Perenual Plant API
/// Implements smart moisture threshold mapping
/// </summary>
public class PlantInfoService : IPlantInfoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlantInfoService> _logger;
    private readonly IConfiguration _configuration;
    private const string BaseUrl = "https://perenual.com/api";

    public PlantInfoService(
        HttpClient httpClient,
        ILogger<PlantInfoService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IEnumerable<PlantSearchResponseDto>> SearchPlantsAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<PlantSearchResponseDto>();
        }

        try
        {
            // Get API key from configuration (free tier)
            var apiKey = _configuration["Perenual:ApiKey"] ?? "sk-mhpf67846dca4bcb37848";

            // Build request URL
            var url = $"{BaseUrl}/species-list?key={apiKey}&q={Uri.EscapeDataString(query)}";

            _logger.LogInformation("Searching plants with query: {Query}", query);

            // Make HTTP request
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Perenual API returned status code: {StatusCode}", response.StatusCode);
                return Array.Empty<PlantSearchResponseDto>();
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<PerenualApiResponse>(jsonString, options);

            if (apiResponse?.Data == null || !apiResponse.Data.Any())
            {
                _logger.LogInformation("No plants found for query: {Query}", query);
                return Array.Empty<PlantSearchResponseDto>();
            }

            // Map to our DTOs with smart moisture threshold calculation
            var results = apiResponse.Data.Select(MapToSearchResponse).ToList();

            _logger.LogInformation("Found {Count} plants for query: {Query}", results.Count, query);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching plants with query: {Query}", query);
            return Array.Empty<PlantSearchResponseDto>();
        }
    }

    public async Task<PlantSearchResponseDto?> GetPlantDetailsAsync(
        int externalApiId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = _configuration["Perenual:ApiKey"] ?? "sk-mhpf67846dca4bcb37848";
            var url = $"{BaseUrl}/species/details/{externalApiId}?key={apiKey}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var plant = JsonSerializer.Deserialize<PerenualPlantDto>(jsonString, options);

            return plant != null ? MapToSearchResponse(plant) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant details for ID: {Id}", externalApiId);
            return null;
        }
    }

    /// <summary>
    /// Maps Perenual API response to our DTO with SMART moisture threshold calculation
    /// This is the core "intelligence" of the feature!
    /// </summary>
    private PlantSearchResponseDto MapToSearchResponse(PerenualPlantDto apiPlant)
    {
        var wateringDescription = apiPlant.Watering?.ToLowerInvariant() ?? "average";

        // SMART MAPPING LOGIC: Text → Numeric Threshold
        // This automatically configures optimal moisture levels based on plant species
        var suggestedThreshold = CalculateMoistureThreshold(wateringDescription);

        return new PlantSearchResponseDto
        {
            CommonName = apiPlant.Common_name ?? "Unknown Plant",
            ScientificName = apiPlant.Scientific_name?.FirstOrDefault() ?? "",
            ImageUrl = apiPlant.Default_image?.Regular_url
                      ?? apiPlant.Default_image?.Medium_url
                      ?? apiPlant.Default_image?.Small_url
                      ?? "",
            SuggestedMoistureThreshold = suggestedThreshold,
            WateringDescription = apiPlant.Watering ?? "Average",
            Sunlight = string.Join(", ", apiPlant.Sunlight ?? Array.Empty<string>()),
            ExternalApiId = apiPlant.Id
        };
    }

    /// <summary>
    /// SMART THRESHOLD CALCULATION
    /// Maps natural language watering descriptions to optimal moisture percentages
    /// Based on horticultural best practices
    /// </summary>
    /// <param name="wateringDescription">Text description from API (e.g., "frequent", "minimal")</param>
    /// <returns>Optimal moisture threshold (0-100%)</returns>
    private int CalculateMoistureThreshold(string wateringDescription)
    {
        return wateringDescription switch
        {
            // High water needs (tropical plants, ferns, etc.)
            var w when w.Contains("frequent") => 60,
            var w when w.Contains("abundant") => 65,
            var w when w.Contains("heavy") => 60,

            // Moderate water needs (most houseplants)
            var w when w.Contains("average") => 40,
            var w when w.Contains("moderate") => 40,
            var w when w.Contains("regular") => 45,
            var w when w.Contains("medium") => 40,

            // Low water needs (succulents, cacti, drought-tolerant)
            var w when w.Contains("minimal") => 20,
            var w when w.Contains("low") => 25,
            var w when w.Contains("infrequent") => 20,
            var w when w.Contains("little") => 25,
            var w when w.Contains("rare") => 15,

            // Very low water needs (desert plants)
            var w when w.Contains("very") && w.Contains("low") => 15,
            var w when w.Contains("drought") => 15,

            // Default to moderate watering
            _ => 40
        };
    }
}
