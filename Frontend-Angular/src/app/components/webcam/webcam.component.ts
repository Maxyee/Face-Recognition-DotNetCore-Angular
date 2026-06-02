import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FaceApiService } from '../../services/face-api.service';
import { FaceVerificationResponse } from '../../models/face-recognition.model';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-webcam',
  standalone: false,
  templateUrl: './webcam.component.html',
  styleUrls: ['./webcam.component.css']
})
export class WebcamComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('video') videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvas') canvasElement!: ElementRef<HTMLCanvasElement>;
  
  public isWebcamActive = false;
  public isProcessing = false;
  public verificationResult: FaceVerificationResponse | null = null;
  public showRegisterDialog = false;
  public capturedImage: string | null = null;
  public userName = '';
  public registrationSuccess = false;
  public healthStatus: string = 'Checking...';
  public lastVerificationTime: Date | null = null;
  
  private mediaStream: MediaStream | null = null;
  private verificationInterval: any;

  constructor(
    private faceApiService: FaceApiService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.checkBackendHealth();
  }

  ngAfterViewInit(): void {
    this.initWebcam();
  }

  private checkBackendHealth(): void {
    this.faceApiService.checkHealth().subscribe({
      next: (response) => {
        this.healthStatus = 'Online';
        this.showNotification('Backend connected successfully!', 'success');
      },
      error: (error) => {
        this.healthStatus = 'Offline';
        this.showNotification('Cannot connect to backend. Please check if server is running.', 'error');
        console.error('Health check failed:', error);
      }
    });
  }

  async initWebcam(): Promise<void> {
    try {
      this.mediaStream = await navigator.mediaDevices.getUserMedia({ 
        video: { 
          width: { ideal: 640 },
          height: { ideal: 480 },
          facingMode: 'user'
        } 
      });
      
      this.videoElement.nativeElement.srcObject = this.mediaStream;
      this.videoElement.nativeElement.onloadedmetadata = () => {
        this.videoElement.nativeElement.play();
        this.isWebcamActive = true;
        this.showNotification('Webcam initialized successfully', 'success');
        this.startPeriodicVerification();
      };
    } catch (err) {
      console.error('Error accessing webcam:', err);
      this.showNotification('Error accessing webcam. Please check permissions.', 'error');
      this.isWebcamActive = false;
    }
  }

  private startPeriodicVerification(): void {
    // Verify face every 2 seconds
    this.verificationInterval = setInterval(() => {
      if (this.isWebcamActive && !this.isProcessing && !this.showRegisterDialog) {
        this.captureAndVerify();
      }
    }, 2000);
  }

  private captureAndVerify(): void {
    const imageData = this.captureImage();
    if (imageData) {
      this.isProcessing = true;
      this.faceApiService.verifyFace(imageData).subscribe({
        next: (result) => {
          this.verificationResult = result;
          this.lastVerificationTime = new Date();
          this.isProcessing = false;
          
          if (result.success && result.userId) {
            this.showNotification(`Welcome ${result.userId}!`, 'success');
          }
        },
        error: (error) => {
          console.error('Verification error:', error);
          this.isProcessing = false;
          this.verificationResult = {
            success: false,
            message: 'Error connecting to server'
          };
        }
      });
    }
  }

  private captureImage(): string | null {
    const video = this.videoElement.nativeElement;
    const canvas = this.canvasElement.nativeElement;
    
    if (video.readyState === video.HAVE_ENOUGH_DATA) {
      canvas.width = video.videoWidth;
      canvas.height = video.videoHeight;
      const context = canvas.getContext('2d');
      
      if (context) {
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        return canvas.toDataURL('image/jpeg', 0.8);
      }
    }
    return null;
  }

  capturePhotoForRegistration(): void {
    const imageData = this.captureImage();
    if (imageData) {
      this.capturedImage = imageData;
      this.showRegisterDialog = true;
      // Pause periodic verification while registering
      if (this.verificationInterval) {
        clearInterval(this.verificationInterval);
      }
    }
  }

  registerFace(): void {
    if (!this.userName.trim()) {
      this.showNotification('Please enter a username', 'error');
      return;
    }
    
    if (!this.capturedImage) {
      this.showNotification('No image captured', 'error');
      return;
    }
    
    this.isProcessing = true;
    this.faceApiService.registerFace(this.userName, this.capturedImage).subscribe({
      next: (response) => {
        this.isProcessing = false;
        if (response.success) {
          this.registrationSuccess = true;
          this.showNotification(response.message, 'success');
          setTimeout(() => {
            this.closeRegisterDialog();
          }, 2000);
        } else {
          this.showNotification(response.message || 'Registration failed', 'error');
        }
      },
      error: (error) => {
        this.isProcessing = false;
        console.error('Registration error:', error);
        this.showNotification('Error during registration', 'error');
      }
    });
  }

  closeRegisterDialog(): void {
    this.showRegisterDialog = false;
    this.capturedImage = null;
    this.userName = '';
    this.registrationSuccess = false;
    // Restart periodic verification
    this.startPeriodicVerification();
  }

  stopWebcam(): void {
    if (this.verificationInterval) {
      clearInterval(this.verificationInterval);
    }
    
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach(track => track.stop());
      this.isWebcamActive = false;
      this.showNotification('Webcam stopped', 'info');
    }
  }

  restartWebcam(): void {
    this.stopWebcam();
    this.initWebcam();
  }

  private showNotification(message: string, type: 'success' | 'error' | 'info' = 'info'): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: type === 'success' ? 'success-snackbar' : type === 'error' ? 'error-snackbar' : 'info-snackbar'
    });
  }

  getConfidenceColor(confidence: number): string {
    if (confidence >= 0.7) return 'high';
    if (confidence >= 0.5) return 'medium';
    return 'low';
  }

  ngOnDestroy(): void {
    this.stopWebcam();
  }
}