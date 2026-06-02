export interface FaceVerificationRequest {
  imageData: string;
}

export interface FaceVerificationResponse {
  success: boolean;
  message: string;
  userId?: string;
  confidence?: number;
}

export interface FaceRegistrationRequest {
  imageData: string;
  userId: string;
}

export interface FaceRegistrationResponse {
  success: boolean;
  message: string;
}

export interface HealthCheckResponse {
  status: string;
  timestamp: string;
}