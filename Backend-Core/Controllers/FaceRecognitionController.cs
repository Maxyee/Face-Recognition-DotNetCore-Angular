using Microsoft.AspNetCore.Mvc;
using Face_Recognition_Demo.Services;


namespace Face_Recognition_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly IFaceRecognitionService _faceRecognitionService;
        private readonly ILogger<FaceRecognitionController> _logger;

        public FaceRecognitionController(
            IFaceRecognitionService faceRecognitionService,
            ILogger<FaceRecognitionController> logger)
        {
            _faceRecognitionService = faceRecognitionService;
            _logger = logger;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyFace([FromBody] FaceVerificationRequest request)
        {
            if (request.ImageData == null || request.ImageData.Length == 0)
            {
                return BadRequest(new { error = "No image data provided" });
            }

            try
            {
                var imageBytes = Convert.FromBase64String(request.ImageData);
                var result = await _faceRecognitionService.VerifyFace(imageBytes);

                return Ok(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    userId = result.UserId,
                    confidence = result.Confidence
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in face verification endpoint");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterFace([FromBody] FaceRegistrationRequest request)
        {
            if (request.ImageData == null || string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { error = "Invalid request data" });
            }

            try
            {
                var imageBytes = Convert.FromBase64String(request.ImageData);
                var service = _faceRecognitionService as FaceRecognitionService;

                if (service == null)
                {
                    return StatusCode(500, new { error = "Service not available" });
                }

                var result = await service.RegisterFace(request.UserId, imageBytes);

                if (result)
                {
                    return Ok(new { success = true, message = $"Face registered for user {request.UserId}" });
                }

                return BadRequest(new { error = "Failed to register face. No face detected." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }
    }

    public class FaceVerificationRequest
    {
        public string ImageData { get; set; } = string.Empty;
    }

    public class FaceRegistrationRequest
    {
        public string ImageData { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
