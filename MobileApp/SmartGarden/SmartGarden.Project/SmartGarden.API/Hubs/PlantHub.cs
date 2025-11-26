using Microsoft.AspNetCore.SignalR;

namespace SmartGarden.API.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time plant sensor data updates
    /// </summary>
    public class PlantHub : Hub
    {
        private readonly ILogger<PlantHub> _logger;

        public PlantHub(ILogger<PlantHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.FindFirst("userId")?.Value;

            _logger.LogInformation(
                "Client connected to PlantHub. ConnectionId: {ConnectionId}, UserId: {UserId}",
                connectionId, userId ?? "Anonymous");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            if (exception != null)
            {
                _logger.LogWarning(exception,
                    "Client disconnected from PlantHub with error. ConnectionId: {ConnectionId}",
                    connectionId);
            }
            else
            {
                _logger.LogInformation(
                    "Client disconnected from PlantHub. ConnectionId: {ConnectionId}",
                    connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Subscribe to updates for a specific plant
        /// </summary>
        public async Task SubscribeToPlant(int plantId)
        {
            var groupName = $"Plant_{plantId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "Client {ConnectionId} subscribed to plant {PlantId}",
                Context.ConnectionId, plantId);
        }

        /// <summary>
        /// Unsubscribe from updates for a specific plant
        /// </summary>
        public async Task UnsubscribeFromPlant(int plantId)
        {
            var groupName = $"Plant_{plantId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "Client {ConnectionId} unsubscribed from plant {PlantId}",
                Context.ConnectionId, plantId);
        }

        /// <summary>
        /// Subscribe to all plants for a user (optional feature)
        /// </summary>
        public async Task SubscribeToUserPlants(int userId)
        {
            var groupName = $"User_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "Client {ConnectionId} subscribed to all plants for user {UserId}",
                Context.ConnectionId, userId);
        }
    }
}
