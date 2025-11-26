namespace SmartGarden.Core.DTOs;

/// <summary>
/// Response DTO for plant search results from external API
/// Uses C# 14 field keyword for property validation
/// </summary>
public class PlantSearchResponseDto
{
    public string CommonName { get; set; } = string.Empty;

    public string ScientificName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Suggested moisture threshold (0-100)
    /// Uses C# 14 field keyword to ensure value is clamped between 0-100
    /// </summary>
    public int SuggestedMoistureThreshold
    {
        get => field;
        set => field = value < 0 ? 0 : value > 100 ? 100 : value;
    }

    public string WateringDescription { get; set; } = string.Empty;

    public string Sunlight { get; set; } = string.Empty;

    public int ExternalApiId { get; set; }
}

/// <summary>
/// Request DTO for creating a plant with smart defaults
/// </summary>
public class CreatePlantFromSearchDto
{
    public string Nickname { get; set; } = string.Empty;

    public string SpeciesName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Moisture threshold with validation using C# 14 field keyword
    /// </summary>
    public int MinMoistureThreshold
    {
        get => field;
        set => field = value < 0 ? 0 : value > 100 ? 100 : value;
    }

    public string RoomName { get; set; } = string.Empty;

    public int? DeviceId { get; set; }

    public bool IsOutdoor { get; set; }
}

/// <summary>
/// Internal DTO for Perenual API response
/// </summary>
public class PerenualPlantDto
{
    public int Id { get; set; }
    public string Common_name { get; set; } = string.Empty;
    public string[] Scientific_name { get; set; } = Array.Empty<string>();
    public string Watering { get; set; } = string.Empty;
    public string[] Sunlight { get; set; } = Array.Empty<string>();
    public PerenualImageDto? Default_image { get; set; }
}

public class PerenualImageDto
{
    public string Regular_url { get; set; } = string.Empty;
    public string Medium_url { get; set; } = string.Empty;
    public string Small_url { get; set; } = string.Empty;
}

public class PerenualApiResponse
{
    public List<PerenualPlantDto> Data { get; set; } = new();
    public int To { get; set; }
    public int Per_page { get; set; }
    public int Current_page { get; set; }
    public int From { get; set; }
    public int Last_page { get; set; }
    public int Total { get; set; }
}
