using Microsoft.AspNetCore.SignalR;
using Face_Recognition_Demo.Services;


namespace Face_Recognition_Demo.Hubs
{
    public class FaceRecognitionHub : Hub
    {
        private readonly IFaceRecognitionService _faceRecognitionService;
        private readonly ILogger<FaceRecognitionHub> _logger;

        public FaceRecognitionHub(
            IFaceRecognitionService faceRecognitionService,
            ILogger<FaceRecognitionHub> logger)
        {
            _faceRecognitionService = faceRecognitionService;
            _logger = logger;
        }

        public async Task SendFrame(byte[] frameData)
        {
            try
            {
                _logger.LogInformation($"Received frame from client: {Context.ConnectionId}");

                // Process the frame for face recognition
                var result = await _faceRecognitionService.VerifyFace(frameData);

                // Send result back to the specific client
                await Clients.Caller.SendAsync("RecognitionResult", new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    userId = result.UserId,
                    confidence = result.Confidence,
                    timestamp = DateTime.UtcNow
                });

                // If face is recognized, broadcast to all connected clients
                if (result.IsSuccess && !string.IsNullOrEmpty(result.UserId))
                {
                    await Clients.All.SendAsync("FaceDetected", new
                    {
                        userId = result.UserId,
                        confidence = result.Confidence,
                        connectionId = Context.ConnectionId,
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing frame");
                await Clients.Caller.SendAsync("Error", new
                {
                    message = "Failed to process frame",
                    error = ex.Message
                });
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await Clients.Caller.SendAsync("Connected", new
            {
                connectionId = Context.ConnectionId,
                message = "Connected to face recognition hub",
                timestamp = DateTime.UtcNow
            });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
