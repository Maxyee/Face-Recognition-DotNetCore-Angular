using Microsoft.AspNetCore.SignalR;

namespace Face_Recognition_Demo.Services
{
    public interface IFaceRecognitionService
    {
        Task<FaceVerificationResult> VerifyFace(byte[] imageBytes);
        Task<float[]> ExtractFaceEmbedding(byte[] imageBytes);
        Task<bool> CompareFaces(float[] embedding1, float[] embedding2);
        Task<bool> RegisterFace(string userId, byte[] imageBytes);
    }

    public class FaceVerificationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public float[]? FaceEmbedding { get; set; }
        public string? UserId { get; set; }
    }
}
