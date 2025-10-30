using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;
using SmartGarden.Core.Shared;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AlertResponseDto>>> GetByUserId(int userId)
        {
            var alerts = await _alertService.GetByUserIdAsync(userId);
            var response = alerts.Select(a => new AlertResponseDto
            {
                AlertId = a.AlertId,
                UserId = a.UserId,
                PlantId = a.PlantId,
                DeviceId = a.DeviceId,
                AlertType = a.AlertType,
                Severity = a.Severity,
                Title = a.Title,
                Message = a.Message,
                TriggeredAt = a.TriggeredAt,
                IsRead = a.IsRead,
                ReadAt = a.ReadAt,
                IsDismissed = a.IsDismissed,
                DismissedAt = a.DismissedAt,
                IsResolved = a.IsResolved,
                ResolvedAt = a.ResolvedAt,
                PlantNickname = a.Plant?.Nickname,
                DeviceName = a.Device?.DeviceName
            });
            return Ok(response);
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<AlertResponseDto>>> GetUnread(int userId)
        {
            var alerts = await _alertService.GetUnreadAsync(userId);
            var response = alerts.Select(a => new AlertResponseDto
            {
                AlertId = a.AlertId,
                UserId = a.UserId,
                PlantId = a.PlantId,
                DeviceId = a.DeviceId,
                AlertType = a.AlertType,
                Severity = a.Severity,
                Title = a.Title,
                Message = a.Message,
                TriggeredAt = a.TriggeredAt,
                IsRead = a.IsRead,
                PlantNickname = a.Plant?.Nickname,
                DeviceName = a.Device?.DeviceName
            });
            return Ok(response);
        }

        [HttpGet("user/{userId}/count")]
        public async Task<ActionResult<int>> GetUnreadCount(int userId)
        {
            var count = await _alertService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlertResponseDto>> GetById(int id)
        {
            var alert = await _alertService.GetByIdAsync(id);
            if (alert == null) return NotFound();

            var response = new AlertResponseDto
            {
                AlertId = alert.AlertId,
                UserId = alert.UserId,
                PlantId = alert.PlantId,
                DeviceId = alert.DeviceId,
                AlertType = alert.AlertType,
                Severity = alert.Severity,
                Title = alert.Title,
                Message = alert.Message,
                TriggeredAt = alert.TriggeredAt,
                IsRead = alert.IsRead,
                ReadAt = alert.ReadAt,
                IsDismissed = alert.IsDismissed,
                DismissedAt = alert.DismissedAt,
                IsResolved = alert.IsResolved,
                ResolvedAt = alert.ResolvedAt,
                PlantNickname = alert.Plant?.Nickname,
                DeviceName = alert.Device?.DeviceName
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<AlertResponseDto>> Create(CreateAlertDto dto)
        {
            var alert = await _alertService.CreateAsync(dto);
            var response = new AlertResponseDto
            {
                AlertId = alert.AlertId,
                UserId = alert.UserId,
                PlantId = alert.PlantId,
                DeviceId = alert.DeviceId,
                AlertType = alert.AlertType,
                Severity = alert.Severity,
                Title = alert.Title,
                Message = alert.Message,
                TriggeredAt = alert.TriggeredAt,
                IsRead = alert.IsRead,
                IsDismissed = alert.IsDismissed,
                IsResolved = alert.IsResolved
            };
            return CreatedAtAction(nameof(GetById), new { id = alert.AlertId }, response);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var updated = await _alertService.MarkAsReadAsync(id);
            if (!updated) return NotFound();
            return Ok(new { message = "Alert marked as read" });
        }

        [HttpPut("{id}/dismiss")]
        public async Task<IActionResult> Dismiss(int id)
        {
            var updated = await _alertService.DismissAsync(id);
            if (!updated) return NotFound();
            return Ok(new { message = "Alert dismissed" });
        }

        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id)
        {
            var updated = await _alertService.ResolveAsync(id);
            if (!updated) return NotFound();
            return Ok(new { message = "Alert resolved" });
        }

        [HttpGet("user/{userId}/severity/{severity}")]
        public async Task<ActionResult<IEnumerable<AlertResponseDto>>> GetBySeverity(int userId, AlertSeverity severity)
        {
            var alerts = await _alertService.GetBySeverityAsync(userId, severity);
            var response = alerts.Select(a => new AlertResponseDto
            {
                AlertId = a.AlertId,
                UserId = a.UserId,
                PlantId = a.PlantId,
                DeviceId = a.DeviceId,
                AlertType = a.AlertType,
                Severity = a.Severity,
                Title = a.Title,
                Message = a.Message,
                TriggeredAt = a.TriggeredAt,
                IsRead = a.IsRead,
                PlantNickname = a.Plant?.Nickname,
                DeviceName = a.Device?.DeviceName
            });
            return Ok(response);
        }
    }
}
