import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  FaceVerificationRequest, 
  FaceVerificationResponse, 
  FaceRegistrationRequest,
  FaceRegistrationResponse,
  HealthCheckResponse 
} from '../models/face-recognition.model';

@Injectable({
  providedIn: 'root'
})
export class FaceApiService {
  // Update this port if your backend runs on a different port
  private baseUrl = 'https://localhost:7069/api/FaceRecognition';
  // Alternative ports if needed:
  // private baseUrl = 'https://localhost:5001/api/FaceRecognition';
  // private baseUrl = 'http://localhost:5000/api/FaceRecognition';

  constructor(private http: HttpClient) { }

  // Health check endpoint
  checkHealth(): Observable<HealthCheckResponse> {
    return this.http.get<HealthCheckResponse>(`${this.baseUrl}/health`);
  }

  // Verify face endpoint
  verifyFace(imageData: string): Observable<FaceVerificationResponse> {
    const request: FaceVerificationRequest = { imageData };
    return this.http.post<FaceVerificationResponse>(`${this.baseUrl}/verify`, request);
  }

  // Register face endpoint
  registerFace(userId: string, imageData: string): Observable<FaceRegistrationResponse> {
    const request: FaceRegistrationRequest = { imageData, userId };
    return this.http.post<FaceRegistrationResponse>(`${this.baseUrl}/register`, request);
  }
}