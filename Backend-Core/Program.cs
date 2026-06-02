using Face_Recognition_Demo.Hubs;
using Face_Recognition_Demo.Services;

namespace Face_Recognition_Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Adding Services for Face Recognition
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add SignalR
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 102400000; // 100MB for video frames
            });

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Angular default port
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Register custom services
            builder.Services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();

            // Add logging
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngularApp");
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<FaceRecognitionHub>("/faceRecognitionHub");
            app.Run();
        }
    }
}
