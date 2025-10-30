using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarden.Core.DTOs;
using SmartGarden.Core.Interfaces;

namespace SmartGarden.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetAll()
        {
            var devices = await _deviceService.GetAllAsync();
            var response = devices.Select(d => new DeviceResponseDto
            {
                DeviceId = d.DeviceId,
                UserId = d.UserId,
                PlantId = d.PlantId,
                DeviceName = d.DeviceName,
                MacAddress = d.MacAddress,
                IpAddress = d.IpAddress,
                FirmwareVersion = d.FirmwareVersion,
                Model = d.Model,
                SerialNumber = d.SerialNumber,
                IsOnline = d.IsOnline,
                LastSeen = d.LastSeen,
                LastHeartbeat = d.LastHeartbeat,
                BatteryLevel = d.BatteryLevel,
                SignalStrength = d.SignalStrength,
                ReadingIntervalSec = d.ReadingIntervalSec,
                IsCalibrated = d.IsCalibrated,
                CalibrationDate = d.CalibrationDate,
                PlantNickname = d.Plant?.Nickname
            });
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetByUserId(int userId)
        {
            var devices = await _deviceService.GetByUserIdAsync(userId);
            var response = devices.Select(d => new DeviceResponseDto
            {
                DeviceId = d.DeviceId,
                UserId = d.UserId,
                PlantId = d.PlantId,
                DeviceName = d.DeviceName,
                MacAddress = d.MacAddress,
                IpAddress = d.IpAddress,
                FirmwareVersion = d.FirmwareVersion,
                Model = d.Model,
                SerialNumber = d.SerialNumber,
                IsOnline = d.IsOnline,
                LastSeen = d.LastSeen,
                LastHeartbeat = d.LastHeartbeat,
                BatteryLevel = d.BatteryLevel,
                SignalStrength = d.SignalStrength,
                ReadingIntervalSec = d.ReadingIntervalSec,
                IsCalibrated = d.IsCalibrated,
                CalibrationDate = d.CalibrationDate,
                PlantNickname = d.Plant?.Nickname
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceResponseDto>> GetById(int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            if (device == null) return NotFound();

            var response = new DeviceResponseDto
            {
                DeviceId = device.DeviceId,
                UserId = device.UserId,
                PlantId = device.PlantId,
                DeviceName = device.DeviceName,
                MacAddress = device.MacAddress,
                IpAddress = device.IpAddress,
                FirmwareVersion = device.FirmwareVersion,
                Model = device.Model,
                SerialNumber = device.SerialNumber,
                IsOnline = device.IsOnline,
                LastSeen = device.LastSeen,
                LastHeartbeat = device.LastHeartbeat,
                BatteryLevel = device.BatteryLevel,
                SignalStrength = device.SignalStrength,
                ReadingIntervalSec = device.ReadingIntervalSec,
                IsCalibrated = device.IsCalibrated,
                CalibrationDate = device.CalibrationDate,
                PlantNickname = device.Plant?.Nickname
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<DeviceResponseDto>> Create(CreateDeviceDto dto)
        {
            var device = await _deviceService.CreateAsync(dto);
            var response = new DeviceResponseDto
            {
                DeviceId = device.DeviceId,
                UserId = device.UserId,
                PlantId = device.PlantId,
                DeviceName = device.DeviceName,
                MacAddress = device.MacAddress,
                Model = device.Model,
                SerialNumber = device.SerialNumber,
                IsOnline = device.IsOnline,
                ReadingIntervalSec = device.ReadingIntervalSec,
                IsCalibrated = device.IsCalibrated
            };
            return CreatedAtAction(nameof(GetById), new { id = device.DeviceId }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DeviceResponseDto>> Update(int id, UpdateDeviceDto dto)
        {
            try
            {
                var device = await _deviceService.UpdateAsync(id, dto);
                var response = new DeviceResponseDto
                {
                    DeviceId = device.DeviceId,
                    UserId = device.UserId,
                    PlantId = device.PlantId,
                    DeviceName = device.DeviceName,
                    MacAddress = device.MacAddress,
                    IpAddress = device.IpAddress,
                    FirmwareVersion = device.FirmwareVersion,
                    Model = device.Model,
                    SerialNumber = device.SerialNumber,
                    IsOnline = device.IsOnline,
                    LastSeen = device.LastSeen,
                    LastHeartbeat = device.LastHeartbeat,
                    BatteryLevel = device.BatteryLevel,
                    SignalStrength = device.SignalStrength,
                    ReadingIntervalSec = device.ReadingIntervalSec,
                    IsCalibrated = device.IsCalibrated,
                    CalibrationDate = device.CalibrationDate
                };
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _deviceService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/heartbeat")]
        [AllowAnonymous]
        public async Task<IActionResult> Heartbeat(string deviceToken)
        {
            var updated = await _deviceService.UpdateHeartbeatAsync(deviceToken);
            if (!updated) return NotFound();
            return Ok(new { message = "Heartbeat received" });
        }

        [HttpGet("offline")]
        public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetOfflineDevices([FromQuery] int thresholdMinutes = 30)
        {
            var threshold = TimeSpan.FromMinutes(thresholdMinutes);
            var devices = await _deviceService.GetOfflineDevicesAsync(threshold);
            var response = devices.Select(d => new DeviceResponseDto
            {
                DeviceId = d.DeviceId,
                UserId = d.UserId,
                PlantId = d.PlantId,
                DeviceName = d.DeviceName,
                IsOnline = d.IsOnline,
                LastHeartbeat = d.LastHeartbeat,
                PlantNickname = d.Plant?.Nickname
            });
            return Ok(response);
        }
    }
}
