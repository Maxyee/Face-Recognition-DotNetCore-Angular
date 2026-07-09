## Step 1: Create the Project with Traditional Structure

```bash
dotnet new webapi -n JwtAuthApi --use-program-main
```

## Step 2: Install Required NuGet Packages

```bash

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.EntityFrameworkCore.Design

```

### Step 3: Create the Database Models

Create Models/User.cs:

```cs

check the model folder and its User.cs File

```

Create Models/Role.cs (optional, but we'll use IdentityRole):

```cs
check the model folder and its Role.cs File
```

### Step 4: Create Database Context
Create Data/ApplicationDbContext.cs

```cs
check the Data folder and its ApplicationDbContext.cs File
```

### Step 5: Create DTOs (Data Transfer Objects)
Create DTOs/RegisterDto.cs:

```cs
check the DTOs folder and its RegisterDto.cs File
```
Create DTOs/LoginDto.cs

```cs
check the DTOs  folder and its LoginDto.cs File
```

Create DTOs/TokenResponseDto.cs

```cs
check the DTOs  folder and its TokenResponseDto.cs File
```

Create DTOs/RefreshTokenDto.cs

```cs
check the DTOs  folder and its RefreshTokenDto.cs File
```

### Step 5.5: Create IJwtService Interface
Create Services/IJwtService.cs

```cs
check the Services  folder and its IJwtService.cs File
```

### Step 6: Create JWT Service
Create Services/JwtService.cs:

```cs
check the Services  folder and its JwtService.cs File
```

### Step 6.5: Register IJwtService in Program.cs
Update your Program.cs to register the interface with its implementation:

```cs
// ... existing code ...

// Register services - Use the interface
builder.Services.AddScoped<IJwtService, JwtService>();

// ... rest of the code ...

```

### Step 7: Create the Auth Controller
Create Controllers/AuthController.cs:

```cs
check the Controller folder and its AuthController.cs File
```

### Step 8: Create Protected Controller for Testing
Create Controllers/UserController.cs

```cs
check the Controller folder and its UserController.cs File
```

### Step 9: Configure Program.cs

```cs
check the program.cs File
```

### Step 10: Create Seeding Helper
Create Helper/SeedData.cs

```cs
check the Helper folder and its SeedData.cs File
```

### Step 11: Configure appsettings.json
Update appsettings.json:

```json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JwtAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "DefaultConnection2": "Server=localhost\\SQLEXPRESS;Database=JwtAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatMustBeAtLeast32CharactersLong!",
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "ExpirationInMinutes": 60
  },
  "AllowedHosts": "*"
}

```

### Step 12: Create Migrations and Database

```bash

# Build the project
dotnet build

# Create and apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

```

### Step 13: Run the Application

```bash
dotnet run

```
### Step 14: Testing the API

```text

Swagger UI at  https://localhost:57257/swagger/index.html

```


### Fix HTTPS Certificate Issues (if needed)

```bash

# For Windows
dotnet dev-certs https --trust

# For Mac
dotnet dev-certs https --trust

# For Linux
dotnet dev-certs https
```