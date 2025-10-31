using SmartGarden_WorkInProgress.Models;

namespace SmartGarden_WorkInProgress.Services;

public class MockPlantDataService
{
    public List<Plant> GetPlants()
    {
        return new List<Plant>
        {
            new Plant
            {
                Id = "1",
                Nickname = "Ficus lyrata",
                Species = "Ficus lyrata",
                IsIndoor = true,
                Room = "Bedroom",
                Image = "plant_ficus.png",
                Humidity = 19,
                Fertilizer = 86,
                WaterTank = 10,
                Light = 78,
                Temperature = 24,
                SoilMoisture = 46,
                NextWatering = TimeSpan.FromMinutes(36),
                Weeks = 26,
                IsSelected = true
            },
            new Plant
            {
                Id = "2",
                Nickname = "Monstera Deliciosa",
                Species = "Monstera deliciosa",
                IsIndoor = true,
                Room = "Living Room",
                Image = "plant_monstera.png",
                Humidity = 65,
                Fertilizer = 72,
                WaterTank = 45,
                Light = 82,
                Temperature = 22,
                SoilMoisture = 58,
                NextWatering = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(15)),
                Weeks = 18,
                IsSelected = false
            },
            new Plant
            {
                Id = "3",
                Nickname = "Snake Plant",
                Species = "Sansevieria trifasciata",
                IsIndoor = true,
                Room = "Bedroom",
                Image = "plant_ficus.png",
                Humidity = 42,
                Fertilizer = 55,
                WaterTank = 78,
                Light = 45,
                Temperature = 23,
                SoilMoisture = 35,
                NextWatering = TimeSpan.FromHours(5),
                Weeks = 12,
                IsSelected = false
            },
            new Plant
            {
                Id = "4",
                Nickname = "Pothos",
                Species = "Epipremnum aureum",
                IsIndoor = true,
                Room = "Kitchen",
                Image = "plant_monstera.png",
                Humidity = 58,
                Fertilizer = 68,
                WaterTank = 62,
                Light = 70,
                Temperature = 21,
                SoilMoisture = 52,
                NextWatering = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(45)),
                Weeks = 8,
                IsSelected = false
            },
            new Plant
            {
                Id = "5",
                Nickname = "Peace Lily",
                Species = "Spathiphyllum",
                IsIndoor = true,
                Room = "Bathroom",
                Image = "plant_ficus.png",
                Humidity = 75,
                Fertilizer = 48,
                WaterTank = 88,
                Light = 55,
                Temperature = 20,
                SoilMoisture = 68,
                NextWatering = TimeSpan.FromMinutes(50),
                Weeks = 22,
                IsSelected = false
            },
            new Plant
            {
                Id = "6",
                Nickname = "Rosemary",
                Species = "Salvia rosmarinus",
                IsIndoor = false,
                Room = "Outdoor",
                Image = "plant_monstera.png",
                Humidity = 38,
                Fertilizer = 82,
                WaterTank = 52,
                Light = 95,
                Temperature = 26,
                SoilMoisture = 42,
                NextWatering = TimeSpan.FromHours(3),
                Weeks = 15,
                IsSelected = false
            }
        };
    }

    public List<TipCard> GetTips()
    {
        return new List<TipCard>
        {
            new TipCard
            {
                Icon = "eco",
                Title = "Watering Schedule",
                Text = "Most indoor plants thrive with weekly watering. Check soil moisture before watering.",
                Emphasis = true
            },
            new TipCard
            {
                Icon = "wb_sunny",
                Title = "Light Requirements",
                Text = "Place plants near windows for indirect sunlight. Rotate them weekly for even growth.",
                Emphasis = false
            },
            new TipCard
            {
                Icon = "touch_app",
                Title = "Soil Health",
                Text = "Use well-draining soil mix. Add perlite for better aeration and root health.",
                Emphasis = false
            },
            new TipCard
            {
                Icon = "nightlight",
                Title = "Low Light Plants",
                Text = "Snake plants and pothos can survive in low light conditions, perfect for offices.",
                Emphasis = true
            }
        };
    }

    public List<RoomFilter> GetRoomFilters(List<Plant> plants)
    {
        var rooms = new List<RoomFilter>
        {
            new RoomFilter { Name = "All", Count = plants.Count, IsSelected = true }
        };

        var groupedRooms = plants
            .GroupBy(p => p.Room)
            .Select(g => new RoomFilter
            {
                Name = g.Key,
                Count = g.Count(),
                IsSelected = false
            });

        rooms.AddRange(groupedRooms);
        return rooms;
    }

    public List<WeeklyStat> GetWeeklyStats()
    {
        return new List<WeeklyStat>
        {
            new WeeklyStat { Day = "Mon", Value = 45 },
            new WeeklyStat { Day = "Tue", Value = 68 },
            new WeeklyStat { Day = "Wed", Value = 82 },
            new WeeklyStat { Day = "Thu", Value = 55 },
            new WeeklyStat { Day = "Fri", Value = 72 },
            new WeeklyStat { Day = "Sat", Value = 90 },
            new WeeklyStat { Day = "Sun", Value = 65 }
        };
    }
}
