using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Models;
using SmartGarden.Core.Shared;
using SmartGarden.Data.Persistence;

namespace SmartGarden.API.Services
{
    public class AlertService : IAlertService
    {
        private readonly SmartGardenDbContext _context;

        public AlertService(SmartGardenDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(int userId)
            => await _context.Alerts
                .Where(a => a.UserId == userId)
                .Include(a => a.Plant)
                .Include(a => a.Device)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

        public async Task<IEnumerable<Alert>> GetUnreadAsync(int userId)
            => await _context.Alerts
                .Where(a => a.UserId == userId && !a.IsRead)
                .Include(a => a.Plant)
                .Include(a => a.Device)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

        public async Task<Alert?> GetByIdAsync(int alertId)
            => await _context.Alerts
                .Include(a => a.Plant)
                .Include(a => a.Device)
                .FirstOrDefaultAsync(a => a.AlertId == alertId);

        public async Task<Alert> CreateAsync(CreateAlertDto dto)
        {
            var alert = new Alert
            {
                UserId = dto.UserId,
                PlantId = dto.PlantId,
                DeviceId = dto.DeviceId,
                AlertType = dto.AlertType,
                Severity = dto.Severity,
                Title = dto.Title,
                Message = dto.Message,
                TriggeredAt = DateTime.UtcNow,
                IsRead = false,
                IsDismissed = false,
                IsResolved = false
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<bool> MarkAsReadAsync(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null) return false;

            alert.IsRead = true;
            alert.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DismissAsync(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null) return false;

            alert.IsDismissed = true;
            alert.DismissedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResolveAsync(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null) return false;

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
            => await _context.Alerts
                .Where(a => a.UserId == userId && !a.IsRead)
                .CountAsync();

        public async Task<IEnumerable<Alert>> GetBySeverityAsync(int userId, AlertSeverity severity)
            => await _context.Alerts
                .Where(a => a.UserId == userId && a.Severity == severity)
                .Include(a => a.Plant)
                .Include(a => a.Device)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();
    }
}
