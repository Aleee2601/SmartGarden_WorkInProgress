namespace SmartGarden.Core.Models
{
    public class PlantPhoto
    {
        public int PlantPhotoId { get; set; }
        public int PlantId { get; set; }

        // Photo storage
        public string PhotoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }

        // Photo metadata
        public string? Caption { get; set; }
        public DateTime TakenAt { get; set; }
        public long? FileSize { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Format { get; set; }

        // Navigation properties
        public Plant Plant { get; set; } = null!;
    }
}
